using System;
using System.Threading;

namespace MyThreadPool;

/// <summary>
/// IMyTask implementation
/// </summary>
public class MyTask<TResult> : IMyTask<TResult>
{
    private Func<TResult> func;
    private Exception? exception;
    private TResult result;
    private CancellationToken shutdownToken;
    private MyThreadPool threadPool;

    public MyTask(Func<TResult> func, CancellationToken token, MyThreadPool pool)
    {
        this.func = func;
        shutdownToken = token;
        threadPool = pool;
    }

    /// <summary>
    /// Contains information about the completion of the task
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Returns task result.
    /// </summary>
    public TResult Result {
        get
        {
            while (!IsCompleted)
            {
                shutdownToken.ThrowIfCancellationRequested();
            }

            if (exception != null)
            {
                throw new AggregateException(exception);
            }
            return result;
        } 
    }

    /// <summary>
    /// Run another task after executing task.
    /// </summary>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod)
    {
        var newFunc = () => continueMethod(Result);
        var task = new MyTask<TNewResult>(newFunc, shutdownToken, threadPool);
        threadPool.ExecuteTask(newFunc);
        return task;
    }
    
    /// <summary>
    /// Tries to invoke task
    /// </summary>
    public void Execute()
    {
        try
        {
            result = func.Invoke();
        }
        catch (Exception e)
        {
            exception = e;
        }

        IsCompleted = true;
    }
}