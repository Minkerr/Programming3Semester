using System.Collections.Concurrent;

namespace MyThreadPool;

public partial class MyThreadPool
{
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
        public TResult Result
        {
            get
            {
                if (!IsCompleted)
                {
                    shutdownToken.ThrowIfCancellationRequested();
                    resultResetEvent.WaitOne();
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
            var nextTask = new MyTask<TNewResult>(() => continueMethod(Result), shutdownToken, threadPool);
            continuingTasks.Enqueue(() => nextTask.Execute());
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
            if (continuingTasks.TryDequeue(out var action))
            {
                threadPool.SubmitContinueAction(action);
            }

            resultResetEvent.Set();
        }
    }
}