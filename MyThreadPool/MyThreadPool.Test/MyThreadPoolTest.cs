using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;

namespace MyThreadPool.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    private int CalculationImitation()
    {
        int result = 0;
        for (int i = 0; i < 1_000_000_000; i++)
        {
            result++;
        }
        return result;
    }
    
    [Test]
    public void ThreadPool_shouldExecuteSimpleExpression()
    {
        MyThreadPool threadPool = new(2);
        IMyTask<int> task = threadPool.Submit(() => 2 + 2);
        threadPool.Shutdown();
        Assert.That(task.Result, Is.EqualTo(4));
    }
    
    [Test]
    public void ThreadPool_shouldExecuteALotOfHardExpression()
    {
        MyThreadPool threadPool = new(8);
        var tasks = new IMyTask<int>[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = threadPool.Submit(CalculationImitation);
        }
        Thread.Sleep(6000);
        threadPool.Shutdown();
        for (int i = 0; i < 20; i++)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(1000000000));
        }
    }
    
    [Test]
    [ExpectedException(typeof(OperationCanceledException))]
    public void ThreadPool_shouldThrowExceptionIfTryTOExecuteALotOfHardExpressionInShortTime()
    {
        MyThreadPool threadPool = new(8);
        var tasks = new IMyTask<int>[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = threadPool.Submit(CalculationImitation);
        }
        threadPool.Shutdown();
    }
    
    [Test]
    [ExpectedException(typeof(AggregateException))]
    public void ThreadPool_shouldThrowExceptionThatCaughtInTask()
    {
        MyThreadPool threadPool = new(8);
        int x = 0;
        IMyTask<int> task = threadPool.Submit(() => 2 / x);
        threadPool.Shutdown();
    }
    
    [Test]
    public void ThreadPool_shouldExecuteContinuingTasks()
    {
        MyThreadPool threadPool = new(8);
        IMyTask<int> task = threadPool.Submit(() => 2 * 2)
            .ContinueWith(x => x + 1)
            .ContinueWith(y => y * 3);
        Thread.Sleep(1000);
        threadPool.Shutdown();
        Assert.That(task.Result, Is.EqualTo(15));
    }
    
    [Test]
    public void ThreadPool_shouldExecuteALotOfContinuingTasks()
    {
        MyThreadPool threadPool = new(8);
        IMyTask<int>[] tasks = new IMyTask<int>[20];
        for (int i = 0; i < 20; i++)
        {
            tasks[i] = threadPool.Submit(() => 2 * 2)
                .ContinueWith(x => x + 1)
                .ContinueWith(y => y * 3)
                .ContinueWith(z => z + 0);
        }
        Thread.Sleep(2000);
        threadPool.Shutdown();
        for (int i = 0; i < 20; i++)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(15));
        }
    }
}