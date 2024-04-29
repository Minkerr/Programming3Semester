using System.Reflection;
using MyNUnit;
using Assert = NUnit.Framework.Assert;

namespace Project.Tests;

public class Tests
{
    [Test]
    public void TestRunner_shouldRunAllTests()
    {
        // arrange
        const string path = @"..\..\..\..\Project.Tests\bin\Debug\net8.0\Project.Tests.dll";
        var assembly = Assembly.LoadFrom(path);
        // act
        var act = TestRunner.RunTestsInAssembly(assembly);
        act.Sort(new TestResultComparer());
        // assert
        Assert.Multiple(() =>
        {
            Assert.That(act[0].TestName, Is.EqualTo("AssertFalseTest"));
            Assert.That(act[0].IsPassed, Is.False);
            Assert.That(act[1].TestName, Is.EqualTo("ExpectedExceptionTest"));
            Assert.That(act[1].IsPassed, Is.True);
            Assert.That(act[2].TestName, Is.EqualTo("IgnoredTest"));
            Assert.That(act[2].IsIgnored, Is.True);
            Assert.That(act[3].TestName, Is.EqualTo("PassedTest"));
            Assert.That(act[3].IsPassed, Is.True);
            Assert.That(act[4].TestName, Is.EqualTo("TestWithArguments"));
            Assert.That(act[4].IsIgnored, Is.True);
            Assert.That(act[5].TestName, Is.EqualTo("TestWithReturnValue"));
            Assert.That(act[5].IsIgnored, Is.True);
            Assert.That(act[6].TestName, Is.EqualTo("UnexpectedExceptionTest"));
            Assert.That(act[6].IsPassed, Is.False);
        });
    }
    
    [Test]
    public void TestRunner_shouldIgnoreTestsAfterFailedBeforeClassMethod()
    {
        // arrange
        const string path = @"..\..\..\..\ProjectFailedBefore.Tests\bin\Debug\net8.0\ProjectFailedBefore.Tests.dll";
        var assembly = Assembly.LoadFrom(path);
        // act
        var act = TestRunner.RunTestsInAssembly(assembly);
        // assert
        Assert.That(act[0].IsIgnored, Is.True);
    }
}