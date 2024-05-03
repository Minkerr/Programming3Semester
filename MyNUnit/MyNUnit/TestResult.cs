namespace MyNUnit;

/// <summary>
/// Information about unit test running 
/// </summary>
public record TestResult(
    string TestName,
    bool IsPassed,
    bool IsIgnored,
    double Duration,
    string? ExceptionMessage,
    string IgnoreMessage
)
{
    /// <summary>
    /// Print information about running group of tests
    /// </summary>
    public static void PrintResults(List<TestResult> results)
    {
        var passedCount = 0;
        var failedCount = 0;
        var ignoredCount = 0;
        Console.WriteLine("\n================================================================");
        Console.WriteLine("Tests running result:\n");
        
        foreach (var result in results)
        {
            if (result.IsIgnored)
            {
                Console.WriteLine($"Ignored: {result.TestName}");
                Console.WriteLine($"Reason: {result.IgnoreMessage}\n");
                ignoredCount++;
            }
            else if (result.IsPassed)
            {
                Console.WriteLine($"Passed: {result.TestName}");
                Console.WriteLine($"Duration: {result.Duration} ms\n");
                passedCount++;
            }
            else
            {
                Console.WriteLine($"Failed: {result.TestName}");
                Console.WriteLine($"Duration: {result.Duration} ms");
                Console.WriteLine(result.ExceptionMessage != null ? $"Message: {result.ExceptionMessage}\n" : "");
                failedCount++;
            }
        }

        Console.WriteLine($"Total test number: {results.Count}");
        Console.WriteLine($"Passed tests: {passedCount}");
        Console.WriteLine($"Failed tests: {failedCount}");
        Console.WriteLine($"Ignored tests: {ignoredCount}");
        Console.WriteLine("\n================================================================\n");
    }
}

public class TestResultComparer : IComparer<TestResult>
{
    public int Compare(TestResult x, TestResult y)
        => string.Compare(x.TestName, y.TestName, StringComparison.Ordinal);
}