
var temp = @"..\..\..\..\Project.Tests\bin\Debug\net8.0";
// var path = Console.ReadLine();
var path = temp;

if (path != null) MyNUnit.TestRunner.RunTestsFromDirectory(path);

