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

        public override List<LinqlFindResult> Find(LinqlExpression ExpressionToFind, LinqlFindResult CurrentResult = null)
        {
            List<LinqlFindResult> results = new List<LinqlFindResult>();

            return results;
        }


    }
}
