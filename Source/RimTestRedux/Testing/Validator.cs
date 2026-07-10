namespace RimTestRedux.Testing;

internal static class Validator
{
    public static void IsValidTestSuite(Type testSuite)
    {
        if (testSuite == null)
        {
            throw new ArgumentNullException(nameof(testSuite));
        }
        if (!CheckTestSuiteIsStatic(testSuite))
        {
            throw new InvalidTestSuiteException($"{testSuite.Name}: test suite must be static.");
        }
        if (!CheckTestSuiteIsPublic(testSuite))
        {
            throw new InvalidTestSuiteException($"{testSuite.Name}: test suite must be public.");
        }
    }

    public static bool CheckTestSuiteIsPublic(Type testSuite) =>
        testSuite != null && testSuite.IsPublic;

    public static bool CheckTestSuiteIsStatic(Type testSuite) =>
        testSuite != null && testSuite.IsClass && testSuite.IsSealed && testSuite.IsAbstract;

    public static void IsValidTest(MethodInfo method)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        if (!CheckTestReturnsVoid(method))
        {
            throw new InvalidTestException($"{method.Name}: tests must have a void return type.");
        }
        if (!CheckTestIsParameterFree(method))
        {
            throw new InvalidTestException($"{method.Name}: tests must need no parameters.");
        }
        if (!CheckTestIsStatic(method))
        {
            throw new InvalidTestException($"{method.Name}: tests must be static.");
        }
        if (!CheckTestIsPublic(method))
        {
            throw new InvalidTestException($"{method.Name}: tests must be public.");
        }
    }

    public static bool CheckTestIsPublic(MethodInfo method) => method != null && method.IsPublic;

    public static bool CheckTestIsStatic(MethodInfo method) => method != null && method.IsStatic;

    public static bool CheckTestReturnsVoid(MethodInfo method) =>
        method != null && method.ReturnType == typeof(void);

    public static bool CheckTestIsParameterFree(MethodInfo method) =>
        method != null && method.GetParameters().Length == 0;
}
