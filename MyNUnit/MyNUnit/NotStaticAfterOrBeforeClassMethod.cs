namespace MyNUnit;

public class NotStaticAfterOrBeforeClassMethod : SystemException
{
    public NotStaticAfterOrBeforeClassMethod()
    {
    }

    public NotStaticAfterOrBeforeClassMethod(string? message) : base(message)
    {
    }
}