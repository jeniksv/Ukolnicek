// See https://aka.ms/new-console-template for more information
using AppLogic;

//start point should create directory with or smt like that

var test = new Test.Builder().WithInputFileName("Data/Assignments/simple.in").WithExpectedOutputFileName("Data/Assignments/simple.out").WithProcessorTime(5000).Build();
test.Run("Data/Users/Temp/test.py");
Console.WriteLine( test.Result );

//        public void AddTest(string testName, string eOFN, int mP = 0, int pT = 5000,
//                        string? iFN = null, string? cLA = null){

var a = new Assignment("A1");
a.AddTest("Test1", "s.out", 2, 5000, "s.in", null);
a.AddTest("Test2", "s2.out", 3, 5000, "s2.in", null);
//a.AddTest(testName = "Test2", eOFN = "s1.out", mP = 3, iFN = "s1.in");
a.RunTests("Data/Users/Temp/test.py");
Console.WriteLine($"points: {a.PointsTotal}");
