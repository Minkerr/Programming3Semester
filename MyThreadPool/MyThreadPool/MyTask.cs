using System.Collections.Concurrent;

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
    private AutoResetEvent resultResetEvent = new(true);
    private readonly ConcurrentQueue<Action> continuingTasks = new();
    private object locker = new();

    public MyTask(Func<TResult> func,
        CancellationToken token,
        MyThreadPool pool,
        Object locker)
    {
        this.func = func;
        shutdownToken = token;
        threadPool = pool;
        this.locker = locker;
    }

    /// <summary>
    /// Contains information about the completion of the task
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Returns task result.
    /// </summary>
    public TResult Result
    {
        get
        {
            lock (locker)
            {
                if (!IsCompleted)
                {
                    shutdownToken.ThrowIfCancellationRequested();
                    resultResetEvent.WaitOne();
                }
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
        var nextTask = new MyTask<TNewResult>(
            () => continueMethod(Result), shutdownToken, threadPool, locker);
        continuingTasks.Enqueue(() => nextTask.Execute());
        lock (locker)
        {
            if (IsCompleted && continuingTasks.TryDequeue(out var action))
            {
                threadPool.SubmitContinueAction(action);
            }
        }
        return nextTask;
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

        func = null;
        IsCompleted = true;
        lock (locker)
        {
            if (continuingTasks.TryDequeue(out var action))
            {
                threadPool.SubmitContinueAction(action);
            }
        }

        resultResetEvent.Set();
    }
}
