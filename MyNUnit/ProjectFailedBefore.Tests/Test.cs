using MyNUnit;
using MyNUnit.Attributes;

namespace ProjectFailedBefore.Tests;

public class Test
{
    [BeforeClass]
    public void BeforeClass()
    {
    }

    [MyTest]
    public void IgnoredTest()
    {
        Assert.That(true);
    }
}