using System.Diagnostics;

namespace Matrix.Tests;

public class MatrixSpeedTests
{
    [Test]
    public void TestMatrixMultiplicationWorkSpeed()
    {
        string[] results = new string[12];
        results[0] = "Average:";
        results[6] = "Deviation:";
        for (int i = 0; i < 5; i++)
        {
            Matrix first = new Matrix(100 * i + 10, 100 * i + 10);
            Matrix second = new Matrix(100 * i + 10, 100 * i + 10);
            Stopwatch stopwatch = new();
            double sumCommon = 0;
            double sumSquaredCommon = 0;
            double sumConcurrent = 0;
            double sumSquaredConcurrent = 0;
            for (int j = 0; j < 10; j++)
            {
                stopwatch.Restart();
                first.Multiply(second);
                stopwatch.Stop();
                sumCommon += (stopwatch.Elapsed.TotalSeconds);
                sumSquaredCommon += (stopwatch.Elapsed.TotalSeconds * stopwatch.Elapsed.TotalSeconds);
                
                stopwatch.Restart();
                first.ConcurrentMultiply(second);
                stopwatch.Stop();
                sumConcurrent += (stopwatch.Elapsed.TotalSeconds);
                sumSquaredConcurrent += (stopwatch.Elapsed.TotalSeconds * stopwatch.Elapsed.TotalSeconds);
            }
            results[i + 1] = (sumCommon / 10).ToString($"F{6}").PadLeft(7, ' ') + " " 
                    + (sumConcurrent / 10).ToString($"F{6}").PadLeft(7, ' ');
            results[i + 7] = (sumSquaredCommon / 10 - sumCommon * sumCommon / 100)
                .ToString($"F{6}").PadLeft(7, ' ') + " " 
                + (sumSquaredCommon / 10 - sumConcurrent * sumConcurrent / 100)
                .ToString($"F{6}").PadLeft(7, ' ');
        }
        File.WriteAllLines("..\\..\\..\\runResult.txt" , results);
        Assert.Pass();
    }
    
}