namespace MyNUnit;

/// <summary>
/// Class that assert that result in test is correct
/// </summary>
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