namespace Matrix.Tests;

public class MatrixTests
{
    [Test]
    public void Multiply_shouldMultiplyTwoSmallMatrix()
    {
        Matrix first = new(new[,]{{1, 2}, {3, 4}});
        Matrix second = new(new[,]{{1, 0}, {0, 1}});
        Matrix expected = new(new [,]{{1, 2}, {3, 4}});
        // act
        var result = first.Multiply(second);
        // assert
        CollectionAssert.AreEqual(result.MatrixArray, expected.MatrixArray);
    }
    
    [Test]
    public void ConcurrentMultiply_shouldMultiplySmallMatrix()
    {
        Matrix first = new(new[,]{{1, 2}, {3, 4}});
        Matrix second = new(new[,]{{1, 0}, {0, 1}});
        Matrix expected = new(new [,]{{1, 2}, {3, 4}});
        // act
        var result = first.ConcurrentMultiply(second);
        // assert
        CollectionAssert.AreEqual(result.MatrixArray, expected.MatrixArray);
    }
    
    [Test]
    public void ConcurrentMultiply_shouldMultiplyMatricesWithDifferentSize()
    {
        Matrix first = new(new[,]{{1, 2, 3}, {4, 5, 6}});
        Matrix second = new(new[,]{{1, 1, 1}, {1, 1, 1}, {1, 1, 1}});
        Matrix expected = new(new[,]{{6, 6, 6}, {15, 15, 15}});
        // act
        var result = first.ConcurrentMultiply(second);
        // assert
        CollectionAssert.AreEqual(result.MatrixArray, expected.MatrixArray);
    }
    
    [Test]
    public void ConcurrentMultiply_shouldMultiplyMatricesFromFile()
    {
        Matrix first = new("matrix1.txt");
        Matrix second = new("matrix2.txt");
        Matrix expected = new("matrix3.txt");
        // act
        first.ConcurrentMultiply(second).WriteToFile("matrix4.txt");
        Matrix result = new("matrix4.txt");
        // assert
        CollectionAssert.AreEqual(result.MatrixArray, expected.MatrixArray);
    }
    
    [Test]
    public void ConcurrentMultiply_shouldWorkLikeMultiply()
    {
        Matrix first = new(50, 50);
        Matrix second = new(50, 50);
        // act
        var resultConcurrent = first.ConcurrentMultiply(second);
        var resultSimple = first.ConcurrentMultiply(second);
        // assert
        CollectionAssert.AreEqual(resultSimple.MatrixArray, resultConcurrent.MatrixArray);
    }
}