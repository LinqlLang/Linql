using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Linql.Server
{
    /// <summary>
    /// Server side LinqlCompiler.  Takes a LinqlSearch/LinqlExpressions and turns them into CSharp expressions that can be executed.  Mimics an ExpressionTreevisitor. 
    /// </summary>
    public partial class LinqlCompiler
    {
        /// <summary>
        /// The Assemblies that Linql will search for functionality.  
        /// </summary>
        public HashSet<Assembly> ValidAssemblies { get; set; } = new HashSet<Assembly>();


        private readonly LinqlLambda StaticLambdaInstance = new LinqlLambda();

        /// <summary>
        /// Whether or not the LinqlCompiler should cache found Methods/Types
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        /// Mapping of Methods that are Cached for a Type.
        /// </summary>
        protected Dictionary<Type, List<MethodInfo>> MethodCache { get; set; } = new Dictionary<Type, List<MethodInfo>>();

        /// <summary>
        /// Local instance of known Parameters.  Parameters can be used anywhere down its expression tree, so we must be able to get reference to them.
        /// </summary>
        protected Dictionary<string, ParameterExpression> Parameters { get; set; } = new Dictionary<string, ParameterExpression>();

        /// <summary>
        /// The Json serialization/deserialization options
        /// </summary>
        protected JsonSerializerOptions JsonOptions { get; set; }


        /// <summary>
        /// Defines before compiler hooks.  
        /// </summary>
        protected List<LinqlBeforeExecutionHook> BeforeHooks { get; set; } = new List<LinqlBeforeExecutionHook>();

        /// <summary>
        /// Defines after compiler hooks.  
        /// </summary>
        protected List<LinqlAfterExecutionHook> AfterHooks { get; set; } = new List<LinqlAfterExecutionHook>();

        /// <summary>
        /// Default LinqlCompile Constructor
        /// </summary>
        /// <param name="extensionAssemblies">Assemblies that Linql should search Extension Methods and Types in</param>
        /// <param name="JsonOptions">Json Serialization/deserializaiton options</param>
        /// <param name="UseCache">Whether or not Linql should use Cache for Methods and Types</param>
        /// <param name="MethodCache">An existing instance of cached methods</param>
        public LinqlCompiler(HashSet<Assembly> extensionAssemblies = null, JsonSerializerOptions JsonOptions = null, bool UseCache = true, Dictionary<Type, List<MethodInfo>> MethodCache = null)
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

            if(JsonOptions == null)
            {
                this.JsonOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true
                };
            }
            else
            {
                this.JsonOptions = JsonOptions;
            }
        }

        /// <summary>
        /// Internal constructor that's used when a nested LInqlCompile is created.  
        /// </summary>
        /// <param name="Parent">The Parent Linql Compiler.</param>
        /// <param name="ParameterExpressions">Parameter expressions available to the child compiler.</param>
        protected LinqlCompiler(LinqlCompiler Parent, Dictionary<string, ParameterExpression> ParameterExpressions) : this(Parent.ValidAssemblies, Parent.JsonOptions, Parent.UseCache, Parent.MethodCache)
        {
            this.Parameters = ParameterExpressions;
            this.BeforeHooks = Parent.BeforeHooks;
            this.AfterHooks = Parent.AfterHooks;
        }

        /// <summary>
        /// Executes a LinqlSearch on an IEnumerable
        /// </summary>
        /// <param name="Search">The LinqlSearch to execute</param>
        /// <param name="Queryable">The Datastore to query</param>
        /// <returns>A result as an object</returns>
        /// <exception cref="Exception">Throws when a LinqlSearch does not start with a Function or a Constant of type LinqlSearch</exception>
        public async Task<object> ExecuteAsync(LinqlSearch Search, IEnumerable Queryable)
        {
            object result = Queryable;

            foreach(LinqlExpression exp in Search.Expressions)
            {
                if (exp is LinqlConstant constant && constant.ConstantType.TypeName == nameof(LinqlSearch))
                {
                    result = await this.TopLevelFunction(constant.Next as LinqlFunction, Queryable);
                }
                else if (exp is LinqlFunction function)
                {
                    result = await this.TopLevelFunction(function, Queryable);
                }
                else
                {
                    throw new Exception($"Linql Search did not start with a function, or a LinqlSearch, but started with {exp.GetType().Name}");
                }
                result = await result.UnwrapTaskAsync();
            }

            return result;
        }

        /// <summary>
        /// Registers a LinqlHook into this LinqlCompiler
        /// </summary>
        /// <param name="Hook">The LinqlCompilerHook</param>
        public void AddHook(LinqlCompilerHook Hook)
        {
            if(Hook is LinqlBeforeExecutionHook before)
            {
                this.BeforeHooks.Add(before);
            }
            else if(Hook is LinqlAfterExecutionHook after)
            {
                this.AfterHooks.Add(after);
            }
        }

        /// <summary>
        /// Unregisters a LinqlHook into this LinqlCompiler
        /// </summary>
        /// <param name="Hook">The LinqlCompilerHook</param>
        public void RemoveHook(LinqlCompilerHook Hook)
        {
            if (Hook is LinqlBeforeExecutionHook before)
            {
                this.BeforeHooks.Remove(before);
            }
            else if (Hook is LinqlAfterExecutionHook after)
            {
                this.AfterHooks.Remove(after);
            }
        }

        /// <summary>
        /// Clears the Method Cache of this compiler.
        /// </summary>
        public void ClearMethodCache()
        {
            this.MethodCache.Clear();
        }

        /// <summary>
        /// Clears the Method Cache for a particular type
        /// </summary>
        /// <param name="Type">The Type to clear cache for</param>
        public void ClearMethodCacheForType(Type Type)
        {
            this.MethodCache.Remove(Type);
        }

        /// <summary>
        /// The top most expression of a LinqlSearch.  This must be a Function.
        /// </summary>
        /// <param name="Function">The Starting Function.</param>
        /// <param name="Queryable">The datastore to query</param>
        /// <returns>The result as an object</returns>
        protected async Task<object> TopLevelFunction(LinqlFunction Function, IEnumerable Queryable)
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

            List<LinqlBeforeExecutionHook> beforeHooks = this.BeforeHooks.Where(r => r.IsValid).ToList();
            List<LinqlAfterExecutionHook> afterHooks = this.AfterHooks.Where(r => r.IsValid).ToList();

            await Task.WhenAll(beforeHooks.Select(r => r.Hook(Function, Queryable, genericType, madeMethod, methodArgs)));

            object result = madeMethod.Invoke(null, methodArgs.ToArray());

            await Task.WhenAll(afterHooks.Select(r => r.Hook(Function, Queryable, genericType, madeMethod, methodArgs, result)));

            if (Function.Next != null)
            {
                result = this.TopLevelFunction(Function.Next as LinqlFunction, result as IEnumerable);
            }
            return result;
        }

        /// <summary>
        /// Given an input and a method, turns the arguments of the method into the correct expression.  When the body is not an expression type, we compile the expression (into a Func).  Otherwise, we use an Expression.  
        /// </summary>
        /// <param name="InputObject">The Input object to operate on</param>
        /// <param name="foundMethod">The Method to call on the input object</param>
        /// <param name="ArgExpressions">The arguments, as expressions, to the method.</param>
        /// <returns>A list of converted arguments.</returns>
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
                            Delegate compiledDeligate = convertedLambda.Compile();
                            
                            methodArgs.Add(compiledDeligate);
                        }
                        else
                        {
                            methodArgs.Add(convertedLambda);
                        }
                    }
                    else if(r.arg is ConstantExpression exp)
                    {
                        methodArgs.Add(exp.Value);
                    }
                    else
                    {
                        methodArgs.Add(r.arg);

                    }

                });

            return methodArgs;
        }

        /// <summary>
        /// Converted a LambdaExpression to the given Type
        /// </summary>
        /// <param name="MethodBodyType">The type to convert the lambda expression to</param>
        /// <param name="Lambda">The original lambda expression</param>
        /// <returns>A LambdaExpression of type MethodBodyType</returns>
        private LambdaExpression ConvertBodyType(Type MethodBodyType, LambdaExpression Lambda)
        {
            Type bodyType = MethodBodyType.GetGenericArguments().FirstOrDefault()?.GetGenericArguments()?.LastOrDefault();

            if (bodyType != null && bodyType != Lambda.Body.Type)
            {
                return Expression.Lambda(Expression.Convert(Lambda.Body, bodyType), Lambda.Parameters);
            }

            return Lambda;
        }

        /// <summary>
        /// Turns a generic method into a constructed generic method based on the SourceType, and the Arguments to the Method
        /// </summary>
        /// <param name="GenericMethod">A Generic Method definition</param>
        /// <param name="SourceType">The datastore source type</param>
        /// <param name="MethodArgs">The arguments to the method</param>
        /// <returns></returns>
        protected MethodInfo CompileGenericMethod(MethodInfo GenericMethod, Type SourceType, IEnumerable<Expression> MethodArgs)
        {
            MethodInfo madeMethod;
            if (!GenericMethod.IsGenericMethod)
            {
                madeMethod = GenericMethod;
            }
            else 
            {
                madeMethod = GenericMethod;
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
            }
            return madeMethod;
        }

        /// <summary>
        /// I think this method matches a generic argument with the type of the supplied argument.  TODO: research this
        /// </summary>
        /// <param name="ParameterType"></param>
        /// <param name="ArgumentType"></param>
        /// <param name="GenericArgMapping"></param>
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

        /// <summary>
        /// Executes a LinqlSearch on a Queryable.
        /// </summary>
        /// <typeparam name="T">The result type of the execution</typeparam>
        /// <param name="Search">The LinqlSearch.</param>
        /// <param name="Queryable">The datasource</param>
        /// <returns>A value of type T</returns>
        public T Execute<T>(LinqlSearch Search, IEnumerable Queryable)
        {
            Task<T> task = this.ExecuteAsync<T>(Search, Queryable);
            task.Wait();
            return task.Result;
        }

        public object Execute(LinqlSearch Search, IEnumerable Queryable)
        {
            Task<object> task = this.ExecuteAsync<object>(Search, Queryable);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Executes a LinqlSearch on a Queryable.
        /// </summary>
        /// <typeparam name="T">The result type of the execution</typeparam>
        /// <param name="Search">The LinqlSearch.</param>
        /// <param name="Queryable">The datasource</param>
        /// <returns>A value of type T</returns>
        public async Task<T> ExecuteAsync<T>(LinqlSearch Search, IEnumerable Queryable)
        {
            object result = await this.ExecuteAsync(Search, Queryable);
            return (T)result;
        }

        /// <summary>
        /// Gets all methods for a Type.  This includes extension methods, and instance methods.  This uses the internal Cache, as well as the UseCache value.
        /// </summary>
        /// <param name="Type">The Type to get methods from.</param>
        /// <returns>A List of methods for this type.</returns>
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

        /// <summary>
        /// Turns a LinqlType into a C# Type
        /// </summary>
        /// <param name="Type">The LinqlType to Convert</param>
        /// <returns>A C# Type</returns>
        protected Type GetTypeFromLinqlObject(LinqlType Type)
        {
            string typeName = Type.TypeName;
            if (Type.TypeName == "List")
            {
                typeName = typeof(List<>).Name;
            }
            else if(Type.TypeName == "object")
            {
                typeName = typeof(object).Name;
            }

            return this.ValidAssemblies.SelectMany(s => s.GetTypes()).FirstOrDefault(r => r.Name == typeName);
        }

        /// <summary>
        /// Gets all methods for a Type.  This includes extension methods, and instance methods.  This method does not use the internal cache. Call GetMethodsForType if you wish to use the cache.
        /// </summary>
        /// <param name="Type">The Type to get methods from.</param>
        /// <returns>A List of methods for this type.</returns>
        protected List<MethodInfo> GetMethods(Type Type)
        {
            List<MethodInfo> allMethods = Type.GetMethods().ToList();
            allMethods.AddRange(this.GetExtensionMethods(Type));
            return allMethods;
        }

        /// <summary>
        /// Gets all extension methods on the Type.  Linql searches its ValidAssemblies for all Extension Methods.  If you think your extension method should be found, and it isn't, make sure you add the Assembly to this.ValidAssemblies.
        /// </summary>
        /// <param name="extendedType">The type to get extension methods for.</param>
        /// <returns>A List of MethodInfo</returns>
        protected IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType)
        {
            IEnumerable<Type> types = this.ValidAssemblies.SelectMany(r => r.GetTypes()).Where(r => r.IsSealed && !r.IsGenericType && !r.IsNested);
            Type extendTypeDef = extendedType.GetGenericTypeDefinitionSafe();

            IEnumerable<MethodInfo> allMethods = types.SelectMany(r => r.GetMethods()).Where(s => s.GetParameters().Count() > 0);

            IEnumerable<MethodInfo> assignableMethods = allMethods.Where(r =>
            {
                string methodName = r.Name;
                IEnumerable<ParameterInfo> parameters = r.GetParameters();
                ParameterInfo parameter = parameters.FirstOrDefault();
                Type genericParameterType = parameter.ParameterType.GetGenericTypeDefinitionSafe();

                if (genericParameterType.IsGenericParameter)
                {
                    return genericParameterType.GetInterfaces().Any(s => s.IsAssignableFrom(extendTypeDef)) 
                    || genericParameterType.BaseType.GetGenericTypeDefinitionSafe().IsAssignableFrom(extendedType);
                }
                else
                {
                    return genericParameterType.IsAssignableFrom(extendTypeDef)
                    ||
                    extendedType.GetInterface(genericParameterType.Name) != null;
                }
            });

            return assignableMethods.ToList();
        }

        /// <summary>
        /// Finds a method on a Type from a LinqlFunction and its Arguments.
        /// </summary>
        /// <param name="FunctionObjectType">The C# Type to Search</param>
        /// <param name="function">The LinqlFunction to try and find</param>
        /// <param name="Args">The Arguments to the LinqlFunction</param>
        /// <returns>A MethodInfo that matches the supplied arguments, or null.</returns>
        protected MethodInfo FindMethod(Type FunctionObjectType, LinqlFunction function, List<Expression> Args)
        {
            IEnumerable<Type> argTypes = Args.Select(r => r.Type);
            return this.FindMethod(FunctionObjectType, function, argTypes);

        }

        /// <summary>
        /// Finds a method on a Type from a LinqlFunction and its Arguments.
        /// </summary>
        /// <param name="FunctionObjectType">The C# Type to Search</param>
        /// <param name="function">The LinqlFunction to try and find</param>
        /// <param name="Args">The Arguments to the LinqlFunction</param>
        /// <returns>A MethodInfo that matches the supplied arguments, or null.</returns>
        protected MethodInfo FindMethod(Type FunctionObjectType, LinqlFunction function, IEnumerable<Type> ArgTypes)
        {
            IEnumerable<MethodInfo> candidates = this.GetMethodsForType(FunctionObjectType);

            IEnumerable<MethodInfo> trimmedMethods = candidates.Where(r => r.Name == function.FunctionName);

            if(trimmedMethods.Any() == false)
            {
                trimmedMethods = candidates.Where(r => r.Name.Contains(function.FunctionName));
            }

            if (trimmedMethods.Any() == false && function.FunctionName.Contains("Async"))
            {
                trimmedMethods = candidates.Where(r => r.Name.Contains(function.FunctionName.Replace("Async", "")));
            }

            bool argTypesSeemsStatic = false;

            Type firstArgType = ArgTypes.FirstOrDefault();
            
            if(firstArgType != null && firstArgType != typeof(string))
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
                    bool generic = s.left.IsGenericParameter;

                    Type leftType = s.left.GetEnumerableType();
                    Type rightType = s.right.GetEnumerableType();
                    bool internalTypesMatch = true;

                    if (s.left.IsEnumerable() && !leftType.IsGenericParameter && s.right.IsEnumerable())
                    {
                        internalTypesMatch = leftType.IsAssignableFrom(rightType);
                    }

                    return (implements || reverseImplements || genericsMatch || isExpressionType || generic) && internalTypesMatch;
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
                found = argMatchFunctions.OrderBy(r => r.GetParameters().Any(s => s.ParameterType == typeof(object))).FirstOrDefault();
            }

            if (found == null)
            {
                throw new Exception($"Unable to find function {function.FunctionName} on type {FunctionObjectType.FullName} with args of type {ArgTypes}.");
            }

            return found;
        }

    }
}
