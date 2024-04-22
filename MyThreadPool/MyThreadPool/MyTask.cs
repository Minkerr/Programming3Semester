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

    public MyTask(Func<TResult> func, CancellationToken token)
    {
        this.func = func;
        shutdownToken = token;
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

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod)
    {
        throw new NotImplementedException();
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