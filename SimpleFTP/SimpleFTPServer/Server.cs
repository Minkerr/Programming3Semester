using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

/// <summary>
/// Class that implements server with two simple file operations
/// </summary>
public class Server
{
    private readonly TcpListener listener;
    private readonly CancellationTokenSource tokenSource = new();
    private readonly List<TcpClient> clients = new();
    private readonly RequestHandler handler = new();
    
    public Server(int port)
        => listener = new TcpListener(IPAddress.Any, port);

    /// <summary>
    /// Starts work of server.
    /// </summary>
    public async Task Start()
    {
        listener.Start();
        List<Task> tasks = new();
        
        while (!tokenSource.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync(tokenSource.Token);
            clients.Add(client);
            tasks.Add(HandleRequests(client, tokenSource.Token));
        }
        
        Task.WaitAll(tasks.ToArray());
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

    private Task HandleRequests(TcpClient client, CancellationToken token)
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
            Disconnect(client);
        }, token);
    }

    private void Disconnect(TcpClient client)
    {
        client.Close();
        clients.Remove(client);
    }
}
