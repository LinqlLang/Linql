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

        public override bool IsMatch(LinqlExpression ExprssionToCompare)
        {
            if (ExprssionToCompare is LinqlBinary bin)
            {
                return
                    this.BinaryName == bin.BinaryName
                    && this.Left.IsMatch(bin.Left)
                    && this.Right.IsMatch(bin.Right);
            }

            return false;
        }

        protected override List<LinqlExpression> ContinueFind(LinqlExpression ExpressionToFind)
        {
            List<LinqlExpression> results = new List<LinqlExpression>();


            List<LinqlExpression> leftMatch = this.Left.Find(ExpressionToFind);
            List<LinqlExpression> rightMatch = this.Right.Find(ExpressionToFind);

            results.AddRange(leftMatch);
            results.AddRange(rightMatch);


            List<LinqlExpression> baseMatch = base.ContinueFind(ExpressionToFind);
            results.AddRange(baseMatch);

            return results;
        }

    }
}
