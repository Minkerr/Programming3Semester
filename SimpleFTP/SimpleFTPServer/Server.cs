using System.Net;
using System.Net.Sockets;

namespace SimpleFTPServer;

/// <summary>
/// Class that implements server with two simple file operations
/// </summary>
public class Server(int port)
{
    private readonly TcpListener listener = new(IPAddress.Any, port);
    private readonly CancellationTokenSource tokenSource = new();
    private readonly List<TcpClient> clients = new();
    private readonly RequestHandler handler = new();

    /// <summary>
    /// Starts work of server.
    /// </summary>
    public async Task Start()
    {
        listener.Start();
        List<Task> tasks = [];
        
        while (!tokenSource.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(tokenSource.Token);
            clients.Add(client);
            tasks.Add(ProcessClient(client, tokenSource.Token));
        }
        
        await Task.WhenAll(tasks.ToArray());
    }

    /// <summary>
    /// Method that stops work of server.
    /// </summary>
    public void Stop()
    {
        foreach (var client in clients)
        {
            client.Close();
        }
        clients.Clear();
        listener.Stop();
        tokenSource.Cancel();
    }

    private Task ProcessClient(TcpClient client, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            await using var stream = client.GetStream();
            using var streamReader = new StreamReader(stream);
            
            while (!token.IsCancellationRequested)
            {
                var request = await streamReader.ReadLineAsync(token);
                if (request != null)
                {
                    handler.RecognizeCommand(request, stream);
                }
            }
            client.Close();
            clients.Remove(client);
        }, token);
    }
}
