using Linql.Client.Internal;
using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linql.Client
{
    public class LinqlObject<T> : LinqlObject
    {
        public T TypedValue
        {
            get
            {
                return (T)this.Value;
            }
        }

        public LinqlObject(T Value) : base(typeof(T), Value)
        {

        }
    }
}
