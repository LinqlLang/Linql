using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Linql.Client.Json
{
    [JsonDerivedType(typeof(LinqlConstant))]
    [JsonDerivedType(typeof(LinqlFunction))]
    [JsonDerivedType(typeof(LinqlLambda))]
    [JsonDerivedType(typeof(LinqlParameter))]
    [JsonDerivedType(typeof(LinqlProperty))]
    [JsonDerivedType(typeof(LinqlBinary))]
    [JsonDerivedType(typeof(LinqlUnary))]

    public abstract class LinqlExpression
    {
        public LinqlExpression Next { get; set; }
    }
}
