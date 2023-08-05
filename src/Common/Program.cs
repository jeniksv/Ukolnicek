// See https://aka.ms/new-console-template for more information
using Ukolnicek.Testing;
using Ukolnicek.Communication;

/*
var task = new FileInfo("task.md");
Assignment.Create("A1", task);
*/

/*
var fin = new FileInfo("simple.in");
var fout = new FileInfo("simple.out");

Assignment.AddTest("A1", "T2", fout, 2, 5000, fin, null);
*/

// Assignment.RunTests("A1", "Data/Users/Temp/test.py");
/*
var a = new Assignment("A1");
a.RunTests("Data/Users/Temp/correct.py");

a.RunTests("Data/Users/Temp/incorrect.py");
// var task = new FileInfo("task.md");
var a2 = new Assignment("Prime");
*/
/*
a2.AddTask(task);
var fin = new FileInfo("simple.in");
var fout = new FileInfo("simple.out");
// public void AddTest(string testName, FileInfo outputFile, int points, int time, FileInfo? inputFile, FileInfo? argumentsFile)
a2.AddTest("Test1", fout, 1, 2000, fin, null);
*/
//a2.RunTests("Data/Users/Temp/prime.py");
//Console.WriteLine(a2.PointsTotal);
var a = new Assignment("Prime");
var result = a.RunTests("Data/Users/Temp/prime.py");
Console.WriteLine();
