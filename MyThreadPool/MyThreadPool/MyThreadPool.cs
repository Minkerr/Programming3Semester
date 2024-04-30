using System.Collections.Concurrent;

namespace MyThreadPool;

/// <summary>
/// Class that implements pool of threads that can execute tasks concurrently 
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource shutdownCancellationTokenSource = new();
    private readonly ConcurrentQueue<Action> tasks = new();
    private ManualResetEvent threadResetEvent = new (true); 
    private Thread[] threads;
    private object locker = new();

    public MyThreadPool(int threadNumber)
    {
        threads = new Thread[threadNumber];
        for (int i = 0; i < threadNumber; i++)
        {
            threads[i] = new Thread(() =>
            {
                 var token = shutdownCancellationTokenSource.Token;
                 while (!token.IsCancellationRequested)
                 { 
                     if (tasks.TryDequeue(out var task))
                     {
                         task.Invoke();
                     }
                     threadResetEvent.WaitOne();
                 }
            });
            threads[i].Start();
        }
    }

    /// <summary>
    /// Add task to pool for executing
    /// </summary>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        var task = new MyTask<TResult>(func, shutdownCancellationTokenSource.Token, this);
        lock (locker)
        {
            shutdownCancellationTokenSource.Token.ThrowIfCancellationRequested();
            tasks.Enqueue(() => task.Execute());
            threadResetEvent.Set();
        }
        return task;
    }

    public void SubmitContinueAction(Action task)
    {
        lock (locker)
        {
            shutdownCancellationTokenSource.Token.ThrowIfCancellationRequested();
            tasks.Enqueue(task);
            threadResetEvent.Set();
        }
    }

    /// <summary>
    /// Close thread pool and break all running tasks 
    /// </summary>
    public void Shutdown()
    {
        shutdownCancellationTokenSource.Cancel();
        
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}