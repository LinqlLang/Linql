using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linql.Core
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type Type)
        {
            return Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

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

        public static bool IsEnumerable(this Type Type)
        {
            return typeof(IEnumerable).IsAssignableFrom(Type);
        }

        public static Type GetEnumerableType(this Type Type)
        {
            if (Type.IsEnumerable())
            {
                return Type.GetGenericArguments().First();
            }
            else
            {
                return Type;
            }
        }

        public static bool IsFunc(this Type Type)
        {
            return
                typeof(Func<>).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe())
            ||
                typeof(Func<,>).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe());
        }

        public static bool IsExpression(this Type Type)
        {
            return typeof(Expression).IsAssignableFrom(Type.GetGenericTypeDefinitionSafe());
        }

        public static LinqlType ToLinqlType(this Type Type)
        {
            return new LinqlType(Type);
        }

        public static bool IsAssignableFromOrImplements(this Type Type, Type TypeToCompare)
        {
            return Type.IsAssignableFrom(TypeToCompare) || TypeToCompare.GetInterface(Type.Name) != null;
        }
    }
}
