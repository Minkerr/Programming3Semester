namespace MyNUnit;

/// <summary>
/// AfterClass and BeforeClass methods should be static 
/// </summary>
public class NotStaticAfterOrBeforeClassMethod : SystemException
{
    public NotStaticAfterOrBeforeClassMethod()
    {
    }

    public NotStaticAfterOrBeforeClassMethod(string? message) : base(message)
    {
    }
}