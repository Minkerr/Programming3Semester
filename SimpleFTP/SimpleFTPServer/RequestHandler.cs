using System.Text;

namespace SimpleFTPServer;

public class RequestHandler
{
    /// <summary>
    /// Recognize and handle command from request 
    /// </summary>
    public async void RecognizeCommand(string request, Stream stream)
    {
        var splitRequest = request.Split();

        switch (splitRequest[0])
        {
            case "1":
                await ListAsync(splitRequest[1], stream);
                break;
            case "2":
                await GetAsync(splitRequest[1], stream);
                break;
            default:
                await SendResponse("Incorrect Input", stream);
                break;
        }
    }
    
    /// <summary>
    /// Get list of files in directory.
    /// </summary>
    private async Task ListAsync(string path, Stream stream)
    {
        if (!Directory.Exists(path))
        {
            await SendResponse("-1\n", stream);
            return;
        }

        var result = new StringBuilder();

        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        var size = directories.Length + files.Length;

        foreach (var directory in directories)
        {
            result.Append($" {directory} true");
        }
        foreach (var file in files)
        {
            result.Append($" {file} false");
        }
        result.Append('\n');

        await SendResponse(size + result.ToString(), stream);
    }

    /// <summary>
    /// Download file from server.
    /// </summary>
    private async Task GetAsync(string path, Stream stream)
    {
        if (!File.Exists(path))
        {
            await SendResponse("-1\n", stream);
            return;
        }

        await SendResponse($"{new FileInfo(path).Length} ", stream);
        var file = File.OpenRead(path);

        await file.CopyToAsync(stream);
    }

    /// <summary>
    /// Method to send string in byte form to stream.
    /// </summary>
    private async Task SendResponse(string message, Stream stream)
    {
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        await stream.FlushAsync();
    }
}