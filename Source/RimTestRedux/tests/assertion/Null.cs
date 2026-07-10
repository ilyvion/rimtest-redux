using System;
using static RimTestRedux.Assertion;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace RimTestRedux.tests.assertion
{
    [TestSuite]
    public static class Null
    {
        [Test]
        public static void PassWhenNull()
        {
            Assert(null).To.Be.Null();
        }
        [Test]
        public static void ThrowWhenNotNull()
        {
            try
            {
                Assert(1).To.Be.Null();
            }
            catch (Exception)
            {
                return;
            }
            throw new Exception("Should have thrown an exception.");
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member