using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool;

/// <summary>
/// Class that implements pool of threads that can execute tasks concurrently 
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource shutdownCancellationTokenSource;
    private readonly ConcurrentQueue<Action> tasks;
    private Thread[] threads;

    public MyThreadPool(int threadNumber)
    {
        tasks = new ConcurrentQueue<Action>();
        shutdownCancellationTokenSource = new CancellationTokenSource();
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
        tasks.Enqueue(() => task.Execute());
        return task;
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