namespace MyThreadPool;

/// <summary>
/// Class that implement a task executed by MyThreadPool
/// </summary>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Contains information about the completion of the task
    /// </summary>
    bool IsCompleted { get; }
    
    /// <summary>
    /// Returns task result.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Run another task after executing task.
    /// </summary>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continueMethod);
}