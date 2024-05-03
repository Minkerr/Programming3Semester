namespace MyNUnit;

public class AssertFailedException : SystemException
{
    public AssertFailedException()
    {
    }

    public AssertFailedException(string? message) : base(message)
    {
    }
}