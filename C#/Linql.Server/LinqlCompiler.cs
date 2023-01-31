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

            if(MethodCache != null)
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

            Search.Expressions.ForEach(exp =>
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

            MethodInfo foundMethod = this.FindMethod(queryableType, Function);

            List<object> methodArgs = new List<object>() { Queryable };

            List<LambdaExpression> argExpressions = Function.Arguments.Select(r => this.VisitLambda(r as LinqlLambda, genericType, typeof(Boolean))).ToList();

            if (foundMethod.GetParameters().Any(r => r.ParameterType.IsFunc()))
            {
                methodArgs.AddRange(argExpressions.Select(r => r.Compile()));
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
            return (T) result;
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
            return this.ValidAssemblies.SelectMany(s => s.GetTypes()).FirstOrDefault(r => r.Name == Type.TypeName);
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

        protected MethodInfo FindMethod(Type QueryableType, LinqlFunction function)
        {
            List<MethodInfo> candidates = this.GetMethodsForType(QueryableType).ToList();

            List<MethodInfo> trimmedMethods = candidates.Where(r => r.Name.Contains(function.FunctionName)).ToList();

            List<MethodInfo> argMatchFunctions = trimmedMethods.Where(r =>
            {
                IEnumerable<ParameterInfo> args = r.GetParameters();

                IEnumerable<LinqlExpression> argTypes = args.Skip(1).Select(s =>
                {
                    Type parameterType = s.ParameterType.GetGenericTypeDefinitionSafe();
                  
                    if (typeof(Func<,>).IsAssignableFrom(parameterType))
                    {
                        return this.StaticLambdaInstance as LinqlExpression;
                    }
                    else if (typeof(Expression).IsAssignableFrom(parameterType))
                    {
                        return this.StaticLambdaInstance as LinqlExpression;
                    }

                    return null;
                });

                bool foundMethod = function.Arguments.Count() == argTypes.Count() 
                && function.Arguments.Zip(argTypes, (userArg, convertedArg) =>
                {
                    return new { left = userArg, right = convertedArg };
                })
                .All(s => 
                s.left != null 
                && 
                s.right != null 
                && 
                s.left.GetType() == s.right.GetType()
                );

                return foundMethod;

            }).ToList();


            IEnumerable<MethodInfo> foundSingleMethods = argMatchFunctions
                  .GroupBy(r => r.Name)
                  .Where(r => r.Count() == 1)
                  .SelectMany(r => r);
            IEnumerable<MethodInfo> extensionOverrides;

            if (typeof(IQueryable).IsAssignableFrom(QueryableType))
            {

                extensionOverrides = argMatchFunctions
                    .GroupBy(r => r.Name)
                    .Where(r => r.Count() > 1)
                    .SelectMany(r => r)
                    .Where(r => typeof(Expression).IsAssignableFrom(r.GetParameters().Skip(1).FirstOrDefault()?.ParameterType.GetGenericTypeDefinitionSafe()));
            }
            else
            {
                extensionOverrides = argMatchFunctions
                   .GroupBy(r => r.Name)
                   .Where(r => r.Count() > 1)
                   .SelectMany(r => r)
                   .Where(r => typeof(Func<,>).IsAssignableFrom(r.GetParameters().Skip(1).FirstOrDefault()?.ParameterType.GetGenericTypeDefinitionSafe()));
            }

            List<MethodInfo> allmethods = foundSingleMethods.ToList();
            allmethods.AddRange(extensionOverrides);
            MethodInfo found = allmethods.FirstOrDefault();

            if (found == null)
            {
                throw new Exception($"Unable to find function {function.FunctionName} on type {QueryableType.FullName}.");
            }

            return found;

        }

    }
}
