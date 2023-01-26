using System;
using System.Collections.Generic;
using System.Text;

namespace Linql.Core
{
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
    }
}
