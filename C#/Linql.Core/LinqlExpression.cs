using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Linql.Core
{
    [JsonDerivedType(typeof(LinqlConstant))]
    [JsonDerivedType(typeof(LinqlFunction))]
    [JsonDerivedType(typeof(LinqlLambda))]
    [JsonDerivedType(typeof(LinqlParameter))]
    [JsonDerivedType(typeof(LinqlProperty))]
    [JsonDerivedType(typeof(LinqlBinary))]
    [JsonDerivedType(typeof(LinqlUnary))]
    [JsonDerivedType(typeof(LinqlObject))]

    public class LinqlExpression
    {
        public LinqlExpression Next { get; set; }

        public LinqlExpression() { }
    }
}
