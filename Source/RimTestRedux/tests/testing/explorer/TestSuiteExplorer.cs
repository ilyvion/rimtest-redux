/*

namespace RimTestRedux.tests
{
    [TestSuite]
    public static class TestSuiteExplorer
    {
        private static class MockTestSuite
        {
            [Test]
            public static void correctTest1() { return; }
            [Test]
            public static void correctTest2() { return; }
            [Test]
            public static bool incorrectTest1() { return true; }
            [Test]
            internal static void incorrectTest2() { return; }
        }
    }

    [TestSuite]
    static class ExampleSkippedTestSuite
    {
        [Test]
        public static void UnranTest() { }
    }

    [TestSuite]
    public static class ExampleErroringTestSuite
    {
        [Test]
        public static void ErroringTest() { Assertion.Assert(true).To.Be.False(); }
        [Test]
        public static void ValidTest() { Assertion.Assert(false).To.Be.False(); }
    }

    [TestSuite]
    public static class ExampleWarningTestSuite
    {
        [Test]
        public static bool SkippedTest() { return true; }
        [Test]
        public static void ValidTest() { Assertion.Assert(false).To.Be.False(); }
    }

}


*/
