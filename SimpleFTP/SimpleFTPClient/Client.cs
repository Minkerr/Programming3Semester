using System.Net.Sockets;
using System.Text;

namespace SimpleFTPClient;

public class Client
{
    private readonly int port;
    private readonly string hostName;
    private const int bufferSize = 1024;
 
    public Client(int port, string hostName)
    {
        this.port = port;
        this.hostName = hostName;
    }

    /// <summary>
    /// Get list of files and subdirectories in the path.
    /// </summary>
    public async Task<List<(string, bool)>> ListAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostName, port);
        
        var request = $"1 {path}\n";
        var stream = client.GetStream();
        
        await stream.WriteAsync(Encoding.UTF8.GetBytes(request));
        await stream.FlushAsync();

        return await ParseListResponse(stream);
    }

    /// <summary>
    /// Download file in bytes
    /// </summary>
    public async Task GetAsync(string path, Stream outStream)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostName, port);

        var request = $"2 {path}\n";
        var stream = client.GetStream();
        await using var writer = new StreamWriter(stream);

        await writer.WriteAsync(request);
        await writer.FlushAsync();
        
        await ParseGetResponse(stream, outStream);
    }
    
    private static async Task<List<(string, bool)>> ParseListResponse(Stream stream)
    {
        var buffer = new byte[bufferSize];
        var builder = new StringBuilder();
        int bytesRead;

        do
        {
            bytesRead = await stream.ReadAsync(buffer);
            builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }
        while (buffer[bytesRead - 1] != '\n');
        
        var response = builder.ToString().Split();

        if (response[0] == "-1")
        {
            throw new DirectoryNotFoundException();
        }

        var size = int.Parse(response[0]);

        var result = new List<(string, bool)>();

        for (var i = 1; i <= size; ++i)
        {
            var fileName = response[(2 * i) - 1];
            var isDirectory = response[2 * i] == "true";

            result.Add((fileName, isDirectory));
        }

        return result.ToList();
    }

    private static async Task ParseGetResponse(Stream stream, Stream outStream)
    {
        var sizeList = new List<byte>();

        int readByte;
        while ((readByte = stream.ReadByte()) != ' ')
        {
            sizeList.Add((byte)readByte);
        }

        var size = int.Parse(Encoding.UTF8.GetString(sizeList.ToArray()));

        if (size == -1)
        {
            throw new FileNotFoundException();
        }

        var buffer = new byte[bufferSize];

        var downloadedSize = 0;

        while (downloadedSize < size)
        {
            var charsRead = await stream.ReadAsync(buffer);
            downloadedSize += charsRead;
            await outStream.WriteAsync(buffer.Take(charsRead).ToArray());
        }
    }
}