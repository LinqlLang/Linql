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

        public List<Assembly> ValidAssemblies { get; set; } = new List<Assembly>();

        public bool UseCache { get; set; }

        protected Dictionary<Type, List<MethodInfo>> MethodCache { get; set; } = new Dictionary<Type, List<MethodInfo>>();

        public LinqlCompiler(List<Assembly> extensionAssemblies = null, bool UseCache = true, Dictionary<Type, List<MethodInfo>> MethodCache = null)
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

       
        public void Execute(LinqlSearch Search, IQueryable Queryable)
        {
            Search.Expressions.ForEach(exp =>
            {
                Type queryableType = Queryable.GetType().GetEnumerableType();

                if (exp is LinqlFunction function)
                {
                    List<MethodInfo> methods = this.GetMethods(queryableType).ToList();
                    MethodInfo foundMethod = this.FindMethod(queryableType, function, methods);

                    List<object> methodArgs = new List<object>() { Queryable };

                    List<LambdaExpression> argExpressionx = function.Arguments.Select(r => this.VisitLambda(r as LinqlLambda, queryableType, typeof(Boolean))).ToList();

                    if (foundMethod.GetParameters().Any(r => r.ParameterType.IsFunc()))
                    {
                        methodArgs.AddRange(argExpressionx.Select(r => r.Compile()));
                    }

                    object intermediateValue = foundMethod.MakeGenericMethod(queryableType).Invoke(null, methodArgs.ToArray());
                }
                else
                {
                    throw new Exception($"Linql Search did not start with a function, but started with {exp.GetType().Name}");
                }
            });
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

        protected List<MethodInfo> GetMethods(Type Type)
        {
            List<MethodInfo> allMethods = Type.GetMethods().ToList();
            allMethods.AddRange(this.GetExtensionMethods(Type));
            return allMethods;
        }

        protected IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType)
        {
            IEnumerable<Type> types = this.ValidAssemblies.SelectMany(r => r.GetTypes()).Where(r => r.IsSealed && !r.IsGenericType && !r.IsNested);
            List<MethodInfo> methods = types.SelectMany(r => r.GetMethods()).ToList();
            return methods;
        }

        protected MethodInfo FindMethod(Type QueryableType, LinqlFunction function, List<MethodInfo> Candidates)
        {
            List<MethodInfo> trimmedMethods = Candidates
                .Where(r => r.Name.Contains(function.FunctionName))
                .Where(r =>
            (r.IsStatic && r.GetParameters().Count() == function.Arguments.Count() + 1)
            ||
            (!r.IsStatic && r.GetParameters().Count() == function.Arguments.Count())).ToList();

            MethodInfo found = trimmedMethods.FirstOrDefault(r =>
            {
                IEnumerable<ParameterInfo> args = r.GetParameters();

                List<LinqlExpression> argTypes = args.Skip(1).Select(s =>
                {
                    Type parameterType = s.ParameterType.GetGenericTypeDefinitionSafe();
                  
                    if (typeof(Func<,>).IsAssignableFrom(parameterType))
                    {
                        return new LinqlLambda() as LinqlExpression;
                    }

                    return null;
                }).ToList();


                bool foundMethod = function.Arguments.Zip(argTypes, (userArg, convertedArg) =>
                {
                    return new { left = userArg, right = convertedArg };
                }).All(s => s.left != null && s.right != null && s.left.GetType() == s.right.GetType());

                return foundMethod;

            });

            if(found == null)
            {
                throw new Exception($"Unable to find function {function.FunctionName} on type {QueryableType.FullName}.");
            }

            return found;

        }

        



    }
}
