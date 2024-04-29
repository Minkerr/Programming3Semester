namespace MyNUnit;

public static class Assert
{
    public static void That(bool expression)
    {
        if (!expression)
        {
            throw new AssertFailedException();
        }
    }
}