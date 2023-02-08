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
                if(exp is LinqlConstant constant && constant.ConstantType.TypeName == nameof(LinqlSearch))
                {
                    result = this.TopLevelFunction(constant.Next as LinqlFunction, Queryable);
                }
                else if (exp is LinqlFunction function)
                {
                    result = this.TopLevelFunction(function, Queryable);
                }
                else
                {
                    throw new Exception($"Linql Search did not start with a function, or a LinqlSearch, but started with {exp.GetType().Name}");
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

            if(Function == null)
            {
                return Queryable;
            }

            Type queryableType = Queryable.GetType();
            Type genericType = Queryable.GetType().GetEnumerableType();

            List<Expression> argExpressions = new List<Expression>();

            if (Function.Arguments != null)
            {
                argExpressions = Function.Arguments.Select(r =>
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
            }

            List<Type> argTypes = new List<Type>
            {
                queryableType
            };
            argTypes.AddRange(argExpressions.Select(r => r.Type));

            MethodInfo foundMethod = this.FindMethod(queryableType, Function, argTypes);

            MethodInfo madeMethod = this.CompileGenericMethod(foundMethod, genericType, argExpressions);

            List<object> methodArgs = this.CompileArguments(Queryable, madeMethod, argExpressions);

            object result = madeMethod.Invoke(null, methodArgs.ToArray());

            if (Function.Next != null)
            {
                result = this.TopLevelFunction(Function.Next as LinqlFunction, result as IEnumerable);
            }
            return result;
        }

        protected List<object> CompileArguments(object InputObject, MethodInfo foundMethod, IEnumerable<Expression> ArgExpressions)
        {
            List<object> methodArgs = new List<object>() { InputObject };
            IEnumerable<ParameterInfo> parameters = foundMethod.GetParameters().Skip(1);

            parameters.Zip(ArgExpressions, (left, right) => new { parameter = left, arg = right })
                .ToList()
                .ForEach(r =>
                {
                    if (r.arg is LambdaExpression lam)
                    {
                        LambdaExpression convertedLambda = this.ConvertBodyType(r.parameter.ParameterType, lam);

                        if (r.parameter.ParameterType.IsFunc())
                        {
                            methodArgs.Add(convertedLambda.Compile());
                        }
                        else
                        {
                            methodArgs.Add(convertedLambda);
                        }
                    }
                    else
                    {
                        methodArgs.Add(r.arg);
                    }

                });

            return methodArgs;
        }

        private LambdaExpression ConvertBodyType(Type MethodBodyType, LambdaExpression Lambda)
        {
            Type bodyType = MethodBodyType.GetGenericArguments().FirstOrDefault()?.GetGenericArguments()?.LastOrDefault();

            if (bodyType != null && bodyType != Lambda.Body.Type)
            {
                return Expression.Lambda(Expression.Convert(Lambda.Body, bodyType), Lambda.Parameters);
            }

            return Lambda;
        }

        protected MethodInfo CompileGenericMethod(MethodInfo GenericMethod, Type SourceType, IEnumerable<Expression> MethodArgs)
        {
            MethodInfo madeMethod = GenericMethod;
            IEnumerable<Type> funGenericArgs = GenericMethod.GetGenericArguments();
            int genericArgCount = funGenericArgs.Count();
            IEnumerable<ParameterInfo> funParameters = GenericMethod.GetParameters().Skip(1);
            IEnumerable<Type> methodArgTypes = MethodArgs.Select(r => typeof(Expression<>).MakeGenericType(r.Type));

            Dictionary<string, Type> genericArgMapping = new Dictionary<string, Type>();
            genericArgMapping.Add(funGenericArgs.FirstOrDefault().Name, SourceType);

            funParameters
               .Zip(methodArgTypes, (left, right) => new { parameter = left, argumentType = right })
               .ToList().ForEach(r => this.LoadGenericTypesFromArguments(r.parameter.ParameterType, r.argumentType, genericArgMapping));

            List<Type> genericArgs = genericArgMapping.Select(r => r.Value).ToList();

            madeMethod = madeMethod.MakeGenericMethod(genericArgs.Take(genericArgCount).ToArray());
            return madeMethod;
        }

        protected void LoadGenericTypesFromArguments(Type ParameterType, Type ArgumentType, Dictionary<string, Type> GenericArgMapping)
        {

            List<Type> GenericTypes = new List<Type>();
            bool areExpressions = (ParameterType.IsExpression() || ParameterType.IsFunc()) && (ArgumentType.IsExpression() || ArgumentType.IsFunc());

            bool implements = ParameterType.GetGenericTypeDefinitionSafe().IsAssignableFromOrImplements(ArgumentType.GetGenericTypeDefinitionSafe());

            if (areExpressions || implements)
            {
                Type argumentType = ArgumentType;

                if (ParameterType.IsFunc() && argumentType.IsExpression())
                {
                    argumentType = argumentType.GetGenericArguments().FirstOrDefault();
                }

                IEnumerable<Type> parameterArgs = ParameterType.GetGenericArguments();
                IEnumerable<Type> argumentTypeArgs = argumentType.GetGenericArguments();
                parameterArgs
                               .Zip(argumentTypeArgs, (left, right) => new { parameter = left, argumentType = right })
                               .ToList().ForEach(r => this.LoadGenericTypesFromArguments(r.parameter, r.argumentType, GenericArgMapping));

            }
            else if (ParameterType.IsConstructedGenericType == false && !GenericArgMapping.ContainsKey(ParameterType.Name))
            {
                GenericArgMapping.Add(ParameterType.Name, ArgumentType);
            }
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

            if (trimmedMethods.Any() == false && function.FunctionName.Contains("Async"))
            {
                trimmedMethods = candidates.Where(r => r.Name.Contains(function.FunctionName.Replace("Async", "")));
            }

            bool argTypesSeemsStatic = false;

            Type firstArgType = ArgTypes.FirstOrDefault();
            
            if(firstArgType != null)
            {
                argTypesSeemsStatic = FunctionObjectType.IsAssignableFromOrImplements(firstArgType) 
                    || firstArgType.IsAssignableFromOrImplements(FunctionObjectType);
            }

            IEnumerable<MethodInfo> argMatchFunctions = trimmedMethods.Where(r =>
            {
                IEnumerable<Type> parameterTypes = r.GetParameters().Select(s => s.ParameterType);

                List<Type> argTypes = new List<Type>();

                if (r.IsStatic && !argTypesSeemsStatic)
                {
                    argTypes.Add(FunctionObjectType);
                }

                argTypes.AddRange(ArgTypes);

                if (parameterTypes.Count() != argTypes.Count())
                {
                    return false;
                }

                return parameterTypes
                .Zip(argTypes, (left, right) => new { left = left, right = right })
                .All(s =>
                {
                    bool implements = s.left.IsAssignableFromOrImplements(s.right.GetGenericTypeDefinitionSafe());
                    bool reverseImplements = s.right.IsAssignableFromOrImplements(s.left.GetGenericTypeDefinitionSafe());
                    bool genericsMatch = s.left.GetGenericTypeDefinitionSafe() == s.right.GetGenericTypeDefinitionSafe();
                    bool isExpressionType = s.left.IsExpression() && s.right.IsFunc();

                    return implements || reverseImplements || genericsMatch || isExpressionType;
                });

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
