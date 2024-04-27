using System.Text;
using SimpleFTPClient;

namespace SimpleFTP.Tests;

public class Tests
{
    private const int port = 8888;
    private const string host = "localhost";
    private readonly Server server = new(port);
    private readonly Client client = new(port, host);
    private Client[] clients = [new(port, host), new(port, host), new(port, host)];

    [OneTimeSetUp]
    public void Setup()
        => Task.Run(() => server.Start());

    [OneTimeTearDown]
    public void Teardown()
        => server.Stop();
    
    [Test]
    public async Task List_ShouldReturnListOfFilesAndDirectories()
    {
        // Arrange
        const string path = ".\\..\\..\\..\\Test";
        const string expected = "(.\\..\\..\\..\\Test\\Folder, True) (.\\..\\..\\..\\Test\\test.txt, False)";
        // Act
        var list = await client.ListAsync(path);
        var builder = new StringBuilder();
        foreach (var item in list)
        {
            builder.Append(item.ToString()).Append(' ');
        }
        var actual = builder.ToString()[..^1];
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test]
    public void List_ShouldThrowExceptionIfDirectoryDoesNotExist()
    {
        const string path = ".\\..\\..\\..\\Testy";
        Assert.ThrowsAsync<DirectoryNotFoundException>(() => client.ListAsync(path));
    }

    [Test]
    public async Task Get_ShouldByteDataOfFile()
    {
        // Arrange
        const string path = ".\\..\\..\\..\\Test\\Folder\\test.txt";
        var expected = await File.ReadAllBytesAsync(path);
        using var stream = new MemoryStream();
        // Act
        await client.GetAsync(path, stream);
        var actual = stream.ToArray();
        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test]
    public async Task Server_ShouldServeManyClients()
    {
        // Arrange
        const string path = ".\\..\\..\\..\\Test\\Folder\\test.txt";
        var expected = await File.ReadAllBytesAsync(path);
        using var stream = new MemoryStream();
        // Act
        await clients[0].GetAsync(path, stream);
        await clients[1].GetAsync(path, stream);
        await clients[2].GetAsync(path, stream);
        var actual1 = stream.ToArray();
        var actual2 = stream.ToArray();
        var actual3 = stream.ToArray(); 
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
            Assert.That(actual3, Is.EqualTo(expected));
        });
    }
}