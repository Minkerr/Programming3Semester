using System.Text;

namespace Matrix;

public class Matrix
{
    public int[,] matrix { get; }
    private int rows;
    private int columns;
    private static readonly int threadNumber = 8;

    public Matrix(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        matrix = new int[this.rows, this.columns];
        var random = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = random.Next(100);
            }
        }
    }
    
    public Matrix(int[,] matrix)
    {
        this.matrix = matrix;
        rows = matrix.GetLength(0);
        columns = matrix.GetLength(1);
    }

    public Matrix(string filePath)
    {
        var input = File.ReadAllLines(filePath);
        rows = input.Length;
        columns = input[0].Split(" ").Length;
        matrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            var row = input[i].Split(" ");
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = int.Parse(row[j]);
            }
        }
    }
    
    public void WriteToFile(string filePath)
    {
        string[] matrixToWrite = new string[rows];
        for (int i = 0; i < rows; i++)
        {
            StringBuilder row = new StringBuilder();
            for (int j = 0; j < columns; j++)
            {
                row.Append(matrix[i, j]).Append(' ');
            }

            matrixToWrite[i] = row.ToString()[..(row.Length - 1)];
        }
        File.WriteAllLines(filePath, matrixToWrite);
    }

    public Matrix Multiply(Matrix another)
    {
        var resultMatrix = new int[this.rows, another.columns];
        for (var i = 0; i < this.rows; i++)
        {
            for (var j = 0; j < another.columns; j++)
            {
                for (var k = 0; k < this.columns; k++)
                {
                    resultMatrix[i, j] += matrix[i, k] * another.matrix[k, j];
                }
            }
        }

        return new Matrix(resultMatrix);
    }

    public Matrix ConcurrentMultiply(Matrix another)
    {
        var resultMatrix = new int[this.rows, another.columns];
        Thread[] threads = new Thread[threadNumber];
        int chunkSize = (this.rows + threadNumber - 1) / threadNumber;
        for (int t = 0; t < threadNumber; t++)
        {
            int localT = t;
            threads[t] = new Thread(() =>
            {
                for (int i = localT * chunkSize; i < (localT + 1) * chunkSize && i < this.rows; i++) 
                {
                    for (int j = 0; j < another.columns; j++)
                    {
                        for (var k = 0; k < this.columns; k++)
                        {
                            resultMatrix[i, j] += matrix[i, k] * another.matrix[k, j];
                        }
                    }
                }
            });
        }
        
        for (int i = 0; i < threadNumber; i++)
        {
            threads[i].Start();
        }
        for (int i = 0; i < threadNumber; i++)
        {
            threads[i].Join();
        }
        return new Matrix(resultMatrix);
    }
}