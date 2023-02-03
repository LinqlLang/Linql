using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Linql.Server
{
    public partial class LinqlCompiler
    {

        public HashSet<Assembly> ValidAssemblies { get; set; } = new HashSet<Assembly>();

        private readonly LinqlLambda StaticLambdaInstance = new LinqlLambda();

        public bool UseCache { get; set; }

        protected Dictionary<Type, List<MethodInfo>> MethodCache { get; set; } = new Dictionary<Type, List<MethodInfo>>();

        protected Dictionary<string, ParameterExpression> Parameters { get; set; } = new Dictionary<string, ParameterExpression>();

        public LinqlCompiler(HashSet<Assembly> extensionAssemblies = null, bool UseCache = true, Dictionary<Type, List<MethodInfo>> MethodCache = null)
        {
            if (extensionAssemblies != null)
            {
                ValidAssemblies = extensionAssemblies;
            }
            this.UseCache = UseCache;

            if (MethodCache != null)
            {
                this.MethodCache = MethodCache;
            }
        }

        //Used for recursive lambda generation
        protected LinqlCompiler(LinqlCompiler Parent, Dictionary<string, ParameterExpression> ParameterExpressions) : this(Parent.ValidAssemblies, Parent.UseCache, Parent.MethodCache)
        {
            this.Parameters = ParameterExpressions;
        }

        public object Execute(LinqlSearch Search, IEnumerable Queryable)
        {
            object result = Queryable;

            Search.Expressions?.ForEach(exp =>
            {

                if (exp is LinqlFunction function)
                {
                    result = this.TopLevelFunction(function, Queryable);
                }
                else
                {
                    throw new Exception($"Linql Search did not start with a function, but started with {exp.GetType().Name}");
                }
            });
            return result;
        }

        public void ClearMethodCache()
        {
            this.MethodCache.Clear();
        }

        public void ClearMethodCacheForType(Type Type)
        {
            this.MethodCache.Remove(Type);
        }

        protected object TopLevelFunction(LinqlFunction Function, IEnumerable Queryable)
        {
            this.Parameters.Clear();

            Type queryableType = Queryable.GetType();
            Type genericType = Queryable.GetType().GetEnumerableType();

            List<Expression> argExpressions = Function.Arguments.Select(r =>
            {
                if (r is LinqlLambda lambda)
                {
                    return this.VisitLambda(r as LinqlLambda, genericType);
                }
                else
                {
                    return this.Visit(r, genericType);
                }
            }).ToList();

            List<Type> argTypes = new List<Type>();
            argTypes.Add(queryableType);
            argTypes.AddRange(argExpressions.Select(r => r.Type));

            MethodInfo foundMethod = this.FindMethod(queryableType, Function, argTypes);

            List<object> methodArgs = new List<object>() { Queryable };

            if (foundMethod.GetParameters().Any(r => r.ParameterType.IsFunc()))
            {
                methodArgs.AddRange(argExpressions.Select<Expression, object>(r =>
                {
                    if (r is LambdaExpression lam)
                    {
                        return lam.Compile();
                    }
                    else
                    {
                        return r;
                    }
                }));
            }
            else
            {
                methodArgs.AddRange(argExpressions);
            }

            object result = foundMethod.MakeGenericMethod(genericType).Invoke(null, methodArgs.ToArray());

            if (Function.Next != null)
            {
                result = this.TopLevelFunction(Function.Next as LinqlFunction, result as IEnumerable);
            }
            return result;
        }

        public T Execute<T>(LinqlSearch Search, IEnumerable Queryable)
        {
            object result = this.Execute(Search, Queryable);
            return (T)result;
        }

        protected List<MethodInfo> GetMethodsForType(Type Type)
        {
            List<MethodInfo> methods;
            if (this.UseCache && !this.MethodCache.ContainsKey(Type))
            {
                methods = this.GetMethods(Type);
                this.MethodCache.Add(Type, methods);
            }
            else if (this.UseCache && this.MethodCache.ContainsKey(Type))
            {
                methods = this.MethodCache[Type];
            }
            else
            {
                methods = this.GetMethods(Type);
            }

            return methods;
        }

        protected Type GetTypeFromLinqlObject(LinqlType Type)
        {
            string typeName = Type.TypeName;
            if (Type.TypeName == "List")
            {
                typeName = typeof(List<>).Name;
            }

            return this.ValidAssemblies.SelectMany(s => s.GetTypes()).FirstOrDefault(r => r.Name == typeName);
        }

        protected List<MethodInfo> GetMethods(Type Type)
        {
            List<MethodInfo> allMethods = Type.GetMethods().ToList();
            allMethods.AddRange(this.GetExtensionMethods(Type));
            return allMethods;
        }

        protected IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType)
        {
            IEnumerable<Type> types = this.ValidAssemblies.SelectMany(r => r.GetTypes()).Where(r => r.IsSealed && !r.IsGenericType && !r.IsNested);

            List<MethodInfo> methods = types
                .SelectMany(r => r.GetMethods())
                .Where(s =>
                s.GetParameters().Count() > 0
                &&
                (s.GetParameters().FirstOrDefault().ParameterType.GetGenericTypeDefinitionSafe().IsAssignableFrom(extendedType.GetGenericTypeDefinitionSafe())
                ||
                extendedType.GetInterface(s.GetParameters().FirstOrDefault().ParameterType.GetGenericTypeDefinitionSafe().Name) != null
                )
                )
                .ToList();
            return methods;
        }

        protected MethodInfo FindMethod(Type FunctionObjectType, LinqlFunction function, List<Expression> Args)
        {
            IEnumerable<Type> argTypes = Args.Select(r => r.Type);
            return this.FindMethod(FunctionObjectType, function, argTypes);

        }

        protected MethodInfo FindMethod(Type FunctionObjectType, LinqlFunction function, IEnumerable<Type> ArgTypes)
        {
            IEnumerable<MethodInfo> candidates = this.GetMethodsForType(FunctionObjectType);

            IEnumerable<MethodInfo> trimmedMethods = candidates.Where(r => r.Name.Contains(function.FunctionName));

            IEnumerable<MethodInfo> argMatchFunctions = trimmedMethods.Where(r =>
            {
                IEnumerable<Type> parameterTypes = r.GetParameters().Select(s => s.ParameterType);

                if (parameterTypes.Count() != ArgTypes.Count())
                {
                    return false;
                }
    
                return parameterTypes
                .Zip(ArgTypes, (left, right) => new { left = left, right = right })
                .All(s =>
                s.left.IsAssignableFromOrImplements(s.right)
                ||
                s.left.GetGenericTypeDefinitionSafe() == s.right.GetGenericTypeDefinitionSafe()
                ||
                (s.left.IsExpression() && s.right.IsFunc())
                );

            });

            IEnumerable<MethodInfo> expressionMethods = argMatchFunctions.Where(r => r.GetParameters().Any(s => s.ParameterType.IsExpression()));
            MethodInfo found;

            if (expressionMethods.Any())
            {
                found = expressionMethods.FirstOrDefault();
            }
            else
            {
                found = argMatchFunctions.FirstOrDefault();
            }

            if (found == null)
            {
                throw new Exception($"Unable to find function {function.FunctionName} on type {FunctionObjectType.FullName} with args of type {ArgTypes}.");
            }

            return found;
        }

    }
}
