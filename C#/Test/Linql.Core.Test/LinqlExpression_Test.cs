using Linql.Core;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Linql.Core.Test
{
    public class LinqlExpression_Test
    {
        [Test]
        public void LinqlBinary()
        {
            LinqlConstant left = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            LinqlConstant right = new LinqlConstant(typeof(bool).ToLinqlType(), false);

            LinqlBinary binary = new LinqlBinary("test", left, right);

            Assert.That(binary.BinaryName, Is.EqualTo("test"));
            Assert.That(binary.Left, Is.EqualTo(left));
            Assert.That(binary.Right, Is.EqualTo(right));

        }

        [Test]
        public void LinqlConstant()
        {
            LinqlConstant left = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            Assert.That(left.Next, Is.EqualTo(null));
            Assert.That(left.Value, Is.EqualTo(false));
            Assert.That(left.ConstantType.TypeName, Is.EqualTo(typeof(bool).ToLinqlType().TypeName));
         
        }

        [Test]
        public void LinqlConstant_Empty()
        {
            LinqlConstant left = new LinqlConstant();
            Assert.That(left.Next, Is.EqualTo(null));
            Assert.That(left.Value, Is.EqualTo(null));
            Assert.That(left.ConstantType, Is.EqualTo(null));

        }


        [Test]
        public void LinqlFunction()
        {
            LinqlConstant constant = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            LinqlFunction fun = new LinqlFunction("Test", new List<LinqlExpression> { constant });
            fun.Object = constant;

            Assert.That(fun.FunctionName, Is.EqualTo("Test"));
            Assert.That(fun.Arguments.Count, Is.EqualTo(1));
            Assert.That(fun.Object, Is.EqualTo(constant));
            Assert.True(fun.Arguments.Contains(constant));

        }


        [Test]
        public void LinqlFunction_Empty()
        {
            LinqlFunction fun = new LinqlFunction();
          
            Assert.That(fun.FunctionName, Is.EqualTo(null));
            Assert.That(fun.Arguments, Is.EqualTo(null));
            Assert.That(fun.Object, Is.EqualTo(null));
            Assert.That(fun.Arguments, Is.EqualTo(null));

        }

        [Test]
        public void LinqlLambda()
        {
            LinqlConstant constant = new LinqlConstant(typeof(bool).ToLinqlType(), false);

            LinqlLambda lam = new LinqlLambda();
            lam.Parameters = new List<LinqlExpression>();
            lam.Parameters.Add(constant);
            lam.Body = constant;
            lam.Next = constant;
            Assert.That(lam.Body, Is.EqualTo(constant));
            Assert.That(lam.Parameters.Count, Is.EqualTo(1));
            Assert.That(lam.Next, Is.EqualTo(constant));
            Assert.True(lam.Parameters.Contains(constant));

        }

        [Test]
        public void LinqlObject()
        {
            LinqlObject obj = new LinqlObject(typeof(bool).ToLinqlType(), false);
            Assert.That(obj.Value, Is.EqualTo(false));
            Assert.That(obj.Type.TypeName, Is.EqualTo(typeof(bool).ToLinqlType().TypeName));

        }

        [Test]
        public void LinqlObject_Type()
        {
            LinqlObject obj = new LinqlObject(typeof(bool), false);
            Assert.That(obj.Value, Is.EqualTo(false));
            Assert.That(obj.Type.TypeName, Is.EqualTo(typeof(bool).ToLinqlType().TypeName));

        }

        [Test]
        public void LinqlObject_Empty()
        {
            LinqlObject obj = new LinqlObject();
            Assert.That(obj.Value, Is.EqualTo(null));
            Assert.That(obj.Type, Is.EqualTo(null));

        }

        [Test]
        public void LinqlParameter()
        {
            LinqlParameter parameter = new LinqlParameter("test");
            Assert.That(parameter.ParameterName, Is.EqualTo("test"));

        }

        [Test]
        public void LinqlParameter_Empty()
        {
            LinqlParameter parameter = new LinqlParameter();
            Assert.That(parameter.ParameterName, Is.EqualTo(null));

        }

        [Test]
        public void LinqlProperty()
        {
            LinqlProperty prop = new LinqlProperty("test");
            Assert.That(prop.PropertyName, Is.EqualTo("test"));
            Assert.That(prop.Next, Is.EqualTo(null));

        }

        [Test]
        public void LinqlType()
        {
            LinqlType type = typeof(List<DataModel>).ToLinqlType();
            Assert.That(type.TypeName, Is.EqualTo("List"));
            Assert.That(type.GenericParameters.Count, Is.EqualTo(1));
            Assert.That(type.GenericParameters.First().TypeName, Is.EqualTo("DataModel"));

        }

        [Test]
        public void LinqlType_Empty()
        {
            LinqlType type = new LinqlType();
            Assert.That(type.TypeName, Is.EqualTo(null));
            Assert.That(type.GenericParameters, Is.EqualTo(null));

        }

        [Test]
        public void LinqlType_LinqlSearch()
        {
            LinqlType type = new LinqlType(typeof(LinqlSearch));
            Assert.That(type.TypeName, Is.EqualTo("LinqlSearch"));

        }

        [Test]
        public void LinqlUnary()
        {
            LinqlConstant constant = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            LinqlUnary unary = new LinqlUnary("test", new List<LinqlExpression>() { constant });
            Assert.That(unary.UnaryName, Is.EqualTo("test"));
            Assert.That(unary.Arguments.Count, Is.EqualTo(1));
            Assert.That(unary.Arguments.First(), Is.EqualTo(constant));

        }

        [Test]
        public void LinqlExpression_GetLastExpressionInNextChain()
        {
            LinqlConstant constant = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            LinqlConstant constant2 = new LinqlConstant(typeof(bool).ToLinqlType(), false);
            LinqlConstant constant3 = new LinqlConstant(typeof(bool).ToLinqlType(), false);

            constant.Next = constant2;
            constant2.Next = constant3;

            LinqlExpression lastInChain = constant.GetLastExpressionInNextChain();

            Assert.That(lastInChain, Is.EqualTo(constant3));


        }

        [Test]
        public void LinqlSearch()
        {
            LinqlSearch search = new LinqlSearch();

            Assert.That(search.Type, Is.EqualTo(null));
            Assert.That(search.Expressions, Is.EqualTo(null));
        }

        [Test]
        public void LinqlSearch_WithType()
        {
            LinqlSearch search = new LinqlSearch(typeof(DataModel));

            Assert.That(search.Type.TypeName, Is.EqualTo(typeof(DataModel).ToLinqlType().TypeName));
            Assert.That(search.Expressions, Is.EqualTo(null));
        }

        [Test]
        public void LinqlSearch_WithLinqlType()
        {
            LinqlSearch search = new LinqlSearch(typeof(DataModel).ToLinqlType());
            search.Expressions = new List<LinqlExpression>();
            Assert.That(search.Type.TypeName, Is.EqualTo(typeof(DataModel).ToLinqlType().TypeName));
            Assert.That(search.Expressions, Is.Not.EqualTo(null));
        }
    }

}