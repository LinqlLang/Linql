using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
    /// <summary>
    /// Represents 
    /// </summary>
    public class LinqlBinary : LinqlExpression
    {
        public string BinaryName { get; set; }

        public LinqlExpression Left { get; set; }

        public LinqlExpression Right { get; set; }

        public LinqlBinary() { }

        public LinqlBinary(string BinaryName, LinqlExpression Left = null, LinqlExpression Right = null)
        {
            this.BinaryName = BinaryName;
            this.Left = Left;
            this.Right = Right;
        }

        public override string ToString()
        {
            return this.BinaryName;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinqlBinary bin)
            {
                return
                    bin.BinaryName == this.BinaryName
                    && bin.Left.Equals(this.Right)
                    && base.Equals(bin);
            }
            return false;
        }

        public override bool IsMatch(LinqlExpression ExprssionToCompare, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            if (ExprssionToCompare is LinqlBinary bin)
            {
                return
                    this.BinaryName == bin.BinaryName
                    && this.Left.IsMatch(bin.Left, FindOption)
                    && this.Right.IsMatch(bin.Right, FindOption);
            }

            return false;
        }

        protected override List<LinqlExpression> ContinueFind(LinqlExpression ExpressionToFind, LinqlFindOption FindOption = LinqlFindOption.Exact)
        {
            List<LinqlExpression> results = new List<LinqlExpression>();


            List<LinqlExpression> leftMatch = this.Left.Find(ExpressionToFind, FindOption);
            List<LinqlExpression> rightMatch = this.Right.Find(ExpressionToFind, FindOption);

            results.AddRange(leftMatch);
            results.AddRange(rightMatch);


            List<LinqlExpression> baseMatch = base.ContinueFind(ExpressionToFind, FindOption);
            results.AddRange(baseMatch);

            return results;
        }

    }
}
