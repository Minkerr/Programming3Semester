using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;

namespace MyNUnit;

/// <summary>
/// Class that execute all tests in given directory
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Run all tests in all assemblies in given directory
    /// </summary>
    public static void RunTestsFromDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }

        foreach (var item in GetAssembliesFromDirectory(path))
        {
            TestResult.PrintResults(RunTestsInAssembly(item));
        }
    }

    private static List<Assembly> GetAssembliesFromDirectory(string path)
        => Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToList();
    
    /// <summary>
    /// Get results of running tests in given assembly 
    /// </summary>
    public static List<TestResult> RunTestsInAssembly(Assembly assembly)
    {
        var tasks = new List<Task<TestResult>>();
        
        foreach (var type in assembly.GetTypes())
        {
            var testMethods = type.GetMethods()
                .Where(methodInfo => 
                    methodInfo.GetCustomAttributes(typeof(MyTestAttribute), false).Length != 0)
                .ToList();

            if (testMethods.Count == 0) continue;
            var shouldRunTestsFlag = true;
            try
            {
                InvokeMethodsByAttribute(type, typeof(BeforeClassAttribute));
            }
            catch (NotStaticAfterOrBeforeClassMethod)
            {
                shouldRunTestsFlag = false;
            }
            
            foreach (var method in testMethods)
            {
                var task = Task.Run(() => RunTest(type, method, shouldRunTestsFlag));
                tasks.Add(task);
            }

            Task.WhenAll(tasks).Wait();

            InvokeMethodsByAttribute(type, typeof(AfterClassAttribute));
        }

        return tasks.Select(t => t.Result).ToList();
    }

    private static TestResult RunTest(Type type, MethodInfo testMethod, bool shouldRunTestsFlag)
    {
        var isIgnored = true;
        var isPassed = true;
        var ignoreMessage = "";
        var duration = 0.0;
        string? exceptionMessage = null;

        var testArgument = testMethod.GetCustomAttribute<MyTestAttribute>();
        if (!shouldRunTestsFlag)
        {
            ignoreMessage = "BeforeClass method execution failed";
            return new TestResult(testMethod.Name, isPassed, isIgnored, duration, exceptionMessage, ignoreMessage);
        }
        if (testArgument?.Ignore is not null)
        {
            ignoreMessage = testArgument.Ignore;
            return new TestResult(testMethod.Name, isPassed, isIgnored, duration, exceptionMessage, ignoreMessage);
        }
        if (testMethod.GetParameters().Length > 0 || testMethod.ReturnType != typeof(void))
        {
            ignoreMessage = "Invalid test method signature.";
            return new TestResult(testMethod.Name, isPassed, isIgnored, duration, exceptionMessage, ignoreMessage);
        }

        isIgnored = false;
        
        InvokeMethodsByAttribute(type, typeof(BeforeAttribute));

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        try
        {
            testMethod.Invoke(Activator.CreateInstance(type), null);
        }
        catch (Exception e)
        {
            if (e.InnerException?.GetType() == typeof(AssertFailedException))
            {
                isPassed = false;
                exceptionMessage = "Test failed";

            }
            else if (!(testArgument?.Expected is not null && e.InnerException?.GetType() == testArgument.Expected))
            {
                isPassed = false;
                exceptionMessage = e.InnerException?.Message;
            }
        }
        stopwatch.Stop();
        
        InvokeMethodsByAttribute(type, typeof(AfterAttribute));
        duration = stopwatch.Elapsed.TotalMilliseconds;
        
        return new TestResult(testMethod.Name, isPassed, isIgnored, duration, exceptionMessage, ignoreMessage);
    }

    private static void InvokeMethodsByAttribute(Type type, Type attributeType)
    {
        foreach (var method in type.GetMethods())
        {
            if (method.GetCustomAttributes(attributeType, false).Length > 0)
            {
                if (attributeType == typeof(BeforeClassAttribute) || attributeType == typeof(AfterClassAttribute))
                {
                    if (method.IsStatic)
                    {
                        method.Invoke(null, null);
                    }
                    else
                    {
                        throw new NotStaticAfterOrBeforeClassMethod();
                    }
                }
                else
                {
                    method.Invoke(Activator.CreateInstance(type), null);
                }
            }
        }
    }
}