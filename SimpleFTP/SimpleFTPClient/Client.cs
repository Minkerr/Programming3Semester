using System.Net.Sockets;
using System.Text;

namespace SimpleFTPClient;

public class Client(int port, string hostName)
{
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
    public async Task<byte[]> GetAsync(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(hostName, port);

        var request = $"2 {path}\n";
        var stream = client.GetStream();
        await using var writer = new StreamWriter(stream);

        await writer.WriteAsync(request);
        await writer.FlushAsync();
        
        return ParseGetResponse(stream);
    }
    
    private static async Task<List<(string, bool)>> ParseListResponse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var stringResponse = await reader.ReadLineAsync();
        if (stringResponse == null)
        {
            throw new NullReferenceException();
        }
        var response = stringResponse.Split();

        if (response[0] == "-1")
        {
            throw new DirectoryNotFoundException();
        }

        var size = int.Parse(response[0]);

        var result = new List<(string, bool)>();

        for (var i = 0; i < size; ++i)
        {
            var fileName = response[2 * i + 1];
            var isDirectory = response[2 * i + 2] == "true";
            result.Add((fileName, isDirectory));
        }

        return result;
    }

    private static byte[] ParseGetResponse(Stream stream)
    {
        var sizeParse = new List<byte>();

        int readByte;
        while ((readByte = stream.ReadByte()) != ' ')
        {
            sizeParse.Add((byte) readByte);
        }

        var size = int.Parse(Encoding.UTF8.GetString(sizeParse.ToArray()));

        if (size == -1)
        {
            throw new FileNotFoundException();
        }
        
        List<byte> result = new();

        for (var i = 0; i < size; ++i)
        {
            result.Add((byte) stream.ReadByte());
        }
        
        return result.ToArray();
    }
}