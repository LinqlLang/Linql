using Linql.Core;

namespace Linql.Core.Test
{
    public class ConstructorTest
    {
        //[Test]
        //public void LinqlExpresion()
        //{
        //    Assert.DoesNotThrow(() =>
        //    {
        //        LinqlExpression exp = new LinqlExpression();
        //    });
        //}

        [Test]
        public void LinqlObject()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlObject exp = new LinqlObject();
            });
        }

        [Test]
        public void LinqlProperty()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlProperty exp = new LinqlProperty();
            });
        }

        [Test]
        public void LinqlUnary()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlUnary exp = new LinqlUnary();
            });
        }

        [Test]
        public void LinqlBinary()
        {
            Assert.DoesNotThrow(() =>
            {
                LinqlBinary exp = new LinqlBinary();
            });
        }
    }

}