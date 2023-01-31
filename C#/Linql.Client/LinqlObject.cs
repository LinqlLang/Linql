using Linql.Core;

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
