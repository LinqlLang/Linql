using Linql.Core;

namespace Linql.Client
{
    /// <summary>
    /// A Typed version of a LinqlObject
    /// </summary>
    /// <typeparam name="T">The type of the IQueryable</typeparam>
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
