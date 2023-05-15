using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Linql.Core
{
    /// <summary>
    /// A LinqlSearch is the container for LinqlExpressions.  It defines the top-most entrypoint of the query.  
    /// </summary>
    public class LinqlSearch
    {
        /// <summary>
        /// The type of search.  This is the entry point of the query.
        /// </summary>
        public LinqlType Type { get; set; }

        /// <summary>
        /// The expressions of the search.
        /// </summary>
        public List<LinqlExpression> Expressions { get; set; } = null;

        /// <summary>
        /// Creates a new LinqlSearch
        /// </summary>
        /// <param name="Type">The entry point type</param>
        public LinqlSearch(LinqlType Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Creates a new LinqlSearch
        /// </summary>
        /// <param name="Type">The entry point type</param>
        public LinqlSearch(Type Type) : this(new LinqlType(Type)) { }

        /// <summary>
        /// This constructor is required for Json serialization/deserialization.  Should probably not use this.
        /// </summary>
        public LinqlSearch()
        {

        }

        public override string ToString()
        {
            if(this.Type == null)
            {
                return "LinqlSearch";
            }
            return $"LinqlSearch<{this.Type.ToString()}>";
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == this.GetType() && obj is LinqlSearch search)
            {
                return search.Type.Equals(this.Type)
                    && this.Expressions.Zip(search.Expressions, (left, right) => left.Equals(right)).All(r => r);
            }
            return false;
        }

        /// <summary>
        /// Tries to find the FindSearch Expression chain within the source LinqlSearch.  This will only search each Expression in the Expressions list in isolation.
        /// </summary>
        /// <param name="FindSearch">The LinqlSearch to find within the source.</param>
        /// <returns>A List of found LinqlExpressions that match the FindSearch</returns>
        public List<LinqlFindResult> Find(LinqlSearch FindSearch)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            this.Expressions.ForEach(r =>
            {
                LinqlExpression findExpression = FindSearch.Expressions.FirstOrDefault();

                if(findExpression != null)
                {
                    List<LinqlFindResult> result = r.Find(findExpression);
                    results.AddRange(result);
                }
            });

            return results;
        }
    }
}
