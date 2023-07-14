// See https://aka.ms/new-console-template for more information
using Assignment;


//start point should create directory with or smt like that

var test = new Test.Builder().WithInputFileName("Data/Assignments/simple.in").WithExpectedOutputFileName("Data/Assignments/simple.out").Build();
test.Run("Data/Users/Temp/test.py");
Console.WriteLine( test.Result );


