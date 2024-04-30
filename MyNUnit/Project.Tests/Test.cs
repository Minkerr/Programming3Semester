using MyNUnit;
using MyNUnit.Attributes;

namespace Project.Tests;

public class Test
{
    [BeforeClass]
    public static void BeforeClass()
    {
        Console.WriteLine("BeforeClass is running");
    }

    [Before]
    public void Before()
    {
        Console.WriteLine("Before is running");
    }
    
    [After]
    public void AfterDown()
    {
        Console.WriteLine("After is running");
    }

    [AfterClass]
    public static void AfterClass()
    {
        Console.WriteLine("AfterClass is running");
    }
    
    [MyTest]
    public void PassedTest()
    {
        Console.WriteLine("PassedTest is running");
    }

    [MyTest]
    public void AssertFalseTest()
    {
        Console.WriteLine("AssertFalseTest is running");
        Assert.That(false);
    }
    
    [MyTest]
    public void UnexpectedExceptionTest()
    {
        Console.WriteLine("UnexpectedExceptionTest is running");
        throw new Exception();
    }

    [MyTest(Expected = typeof(SystemException))]
    public void ExpectedExceptionTest()
    {
        Console.WriteLine("ExpectedExceptionTest is running");
        throw new SystemException("Expected exception");
    }

    [MyTest(Ignore = "Test message for ignored test")]
    public void IgnoredTest()
    {
        Console.WriteLine("IgnoredTest is running");
    }

    [MyTest]
    public void TestWithArguments(int number)
    {
        Console.WriteLine("TestWithArguments is running");
    }

    [MyTest]
    public int TestWithReturnValue()
    {
        Console.WriteLine("TestWithReturnValue is running");
        return 0;
    }
}