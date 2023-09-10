using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Core
{
    /// <summary>
    /// Helpful Extension methods to inspect types
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns whether or not a type is a Nullable type
        /// </summary>
        /// <param name="Type">The CSharpType</param>
        /// <returns>true or false</returns>
        public static bool IsNullable(this Type Type)
        {
            return Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// A safe method for getting a generic type definition. 
        /// </summary>
        /// <param name="Type">The type to get the definition from</param>
        /// <returns>Either the generic definition, or the original definition if it isn't generic</returns>
        public static Type GetGenericTypeDefinitionSafe(this Type Type)
        {
            if (Type.IsConstructedGenericType)
            {
                return Type.GetGenericTypeDefinition();
            }
            else
            {
                return Type;
            }
        }

        /// <summary>
        /// Returns whether a type is an Enumerable
        /// </summary>
        /// <param name="Type">The type to inspect</param>
        /// <returns>true or false</returns>
        public static bool IsEnumerable(this Type Type)
        {
            return typeof(IEnumerable).IsAssignableFrom(Type);
        }

        /// <summary>
        /// Returns whether a type is an Dictionary
        /// </summary>
        /// <param name="Type">The type to inspect</param>
        /// <returns>true or false</returns>

        public static bool IsDictionary(this Type Type)
        {
            return typeof(IDictionary).IsAssignableFrom(Type);
        }

        /// <summary>
        /// Returns the inner type of a list.  For example, List<int> will return int
        /// </summary>
        /// <param name="Type">The type to inspect</param>
        /// <returns>The type of the list</returns>
        public static Type GetEnumerableType(this Type Type)
        {
            if (Type.IsGenericType && Type.IsEnumerable())
            {
                return Type.GetGenericArguments().First();
            }
            else
            {
                return Type;
            }
        }

        /// <summary>
        /// Returns whether a type is a Func<> or a Func<,>.  To support more Func parameters, just need to add more Func<> defs to this list
        /// </summary>
        /// <param name="Type">The type to inspect</param>
        /// <returns>true or false</returns>
        public static bool IsFunc(this Type Type)
        {
            return
                typeof(Func<>).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe())
            ||
                typeof(Func<,>).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe());
        }

        /// <summary>
        /// Returns whether a type is an expression
        /// </summary>
        /// <param name="Type">The type to inspect</param>
        /// <returns>true or false</returns>
        public static bool IsExpression(this Type Type)
        {
            return typeof(Expression).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe());
        }

        /// <summary>
        /// Turns a CSharp type into  LinqlType
        /// </summary>
        /// <param name="Type">The type to convert</param>
        /// <returns>A LinqlType</returns>
        public static LinqlType ToLinqlType(this Type Type)
        {
            return new LinqlType(Type);
        }

        /// <summary>
        /// Extracts a result from a generic Task
        /// </summary>
        /// <param name="Task">The task to extract the result from</param>
        /// <returns>The Result of the task, or null if the Task was of type void</returns>
        public static async Task<object> GetGenericTaskResultAsync(this Task Task)
        {
            await Task;
            PropertyInfo resultProperty = Task.GetType().GetProperty(nameof(Task<object>.Result));

            if(resultProperty != null)
            {
                return resultProperty.GetValue(Task);
            }
            return null;
        }

        /// <summary>
        /// Extracts a result from a generic Task
        /// </summary>
        /// <param name="Task">The task to extract the result from</param>
        /// <returns>The Result of the task, or null if the Task was of type void</returns>
        public static object GetGenericTaskResult(this Task Task)
        {
            Task.Wait();
            PropertyInfo resultProperty = Task.GetType().GetProperty(nameof(Task<object>.Result));

            if (resultProperty != null)
            {
                return resultProperty.GetValue(Task);
            }
            return null;
        }

        /// <summary>
        /// Unwraps a task until a materialized result is available.
        /// </summary>
        /// <param name="Object">The object to unwrap</param>
        /// <returns>The Result of the task if the Object is a task, otherwise, the object</returns>
        public static async Task<object> UnwrapTaskAsync(this object Object)
        {
            if(Object is Task Task)
            {
                await Task;
                Object = await Task.GetGenericTaskResultAsync();
                return await Object.UnwrapTaskAsync();
            }

            return Object;
        }

        /// <summary>
        /// Unwraps a task until a materialized result is available.
        /// </summary>
        /// <param name="Object">The object to unwrap</param>
        /// <returns>The Result of the task if the Object is a task, otherwise, the object</returns>
        public static object UnwrapTask(this object Object)
        {
            if (Object is Task Task)
            {
                Task.Wait();
                Object = Task.GetGenericTaskResult();
                return Object.UnwrapTask();
            }

            return Object;
        }

        /// <summary>
        /// Returns whether a Type is Assignable from or Implements another type
        /// </summary>
        /// <param name="Type">The type</param>
        /// <param name="TypeToCompare">The type to compare</param>
        /// <returns>true if Type is assignable from TypeToCompare or implements TypeToCompare</returns>
        public static bool IsAssignableFromOrImplements(this Type Type, Type TypeToCompare)
        {
            return Type.IsAssignableFrom(TypeToCompare) || TypeToCompare.GetInterface(Type.Name) != null;
        }
    }
}
