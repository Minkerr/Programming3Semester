using System.Text;

namespace Matrix;

/// <summary>
/// Сlass that implements matrices with a multiplication operation
/// </summary>
public class Matrix
{
    public int[,] MatrixArray { get; }
    private static Random random = new();
    private static readonly int threadNumber = Environment.ProcessorCount;

    /// <summary>
    /// Random constructor
    /// </summary>
    public Matrix(int rows, int columns)
    {
        MatrixArray = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                MatrixArray[i, j] = random.Next(100);
            }
        }
    }
    
    /// <summary>
    /// Constructor by 2d-array
    /// </summary>
    public Matrix(int[,] matrixArray) => MatrixArray = matrixArray;
    

    /// <summary>
    /// Constructor by matrix from file 
    /// </summary>
    public Matrix(string filePath)
    {
        var input = File.ReadAllLines(filePath);
        var rows = input.Length;
        var columns = input[0].Split(" ").Length;
        MatrixArray = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            var row = input[i].Split(" ");
            if (row.Length != columns)
            {
                throw new ArgumentException();
            }
            for (int j = 0; j < columns; j++)
            {
                MatrixArray[i, j] = int.Parse(row[j]);
            }
        }
    }
    
    /// <summary>
    /// Write matrix to file
    /// </summary>
    public void WriteToFile(string filePath)
    {
        var rows = MatrixArray.GetLength(0);
        var columns = MatrixArray.GetLength(1);
        var matrixToWrite = new string[rows];
        for (int i = 0; i < rows; i++)
        {
            var row = new StringBuilder();
            for (int j = 0; j < columns; j++)
            {
                row.Append(MatrixArray[i, j]).Append(' ');
            }

            matrixToWrite[i] = row.ToString()[..(row.Length - 1)];
        }
        File.WriteAllLines(filePath, matrixToWrite);
    }

    /// <summary>
    /// Simple matrix multiplication
    /// </summary>
    public Matrix Multiply(Matrix another)
    {
        var rows = MatrixArray.GetLength(0);
        var columns = MatrixArray.GetLength(1);
        var anotherColumns = another.MatrixArray.GetLength(1);
        var resultMatrix = new int[rows, anotherColumns];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < anotherColumns; j++)
            {
                for (var k = 0; k < columns; k++)
                {
                    resultMatrix[i, j] += MatrixArray[i, k] * another.MatrixArray[k, j];
                }
            }
        }

        return new Matrix(resultMatrix);
    }

    /// <summary>
    /// Concurrent matrix multiplication
    /// </summary>
    public Matrix ConcurrentMultiply(Matrix another)
    {
        var rows = MatrixArray.GetLength(0);
        var columns = MatrixArray.GetLength(1);
        var anotherColumns = another.MatrixArray.GetLength(1);
        var resultMatrix = new int[rows, anotherColumns];
        int nThread = int.Min(threadNumber, rows);
        var threads = new Thread[nThread];
        int chunkSize = (rows + nThread - 1) / nThread;
        for (int t = 0; t < nThread; t++)
        {
            int localT = t;
            threads[t] = new Thread(() =>
            {
                for (int i = localT * chunkSize; i < (localT + 1) * chunkSize && i < rows; i++) 
                {
                    for (int j = 0; j < anotherColumns; j++)
                    {
                        for (var k = 0; k < columns; k++)
                        {
                            resultMatrix[i, j] += MatrixArray[i, k] * another.MatrixArray[k, j];
                        }
                    }
                }
            });
        }
        
        for (int i = 0; i < nThread; i++)
        {
            threads[i].Start();
        }
        for (int i = 0; i < nThread; i++)
        {
            threads[i].Join();
        }
        return new Matrix(resultMatrix);
    }
}