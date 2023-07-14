using System.Diagnostics;

namespace Assignment;

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test
// TODO builder design patter for Test class 
// TODO for test we can 


public enum TestResult { Correct, TimeExceeded, ExceptionError, CompilationError }

internal class TestBuilder{
	public double WithProcessorTime(){}
	public string WithCommandLineArguments(){}
	public string WithInputFileName(){}
	public string ExpectedOutputFileName(){}
}

internal class Test{
	public int maxPoints;
	// these should be private field;
	public double processorTime;
	public string commandLineArguments;
	public string inputFileName;
	public string expectedOutputFileName;


	
	// definitely public 
	public int Points = 0;
	// test message
	public TestResult Result;
	public string TestOutput; // TODO ability to hide this field
	
	public void Evaluate(){
		// TODO security -> test should run in virtual env
	}
	// TODO public string Interpreter;

}

public class Assignment{
	public List<Test> tests;
	public int PointsTotal;

	public void EvaluateTests(){
		PointsTotal = 0;

		foreach(var test in tests){
			test.Evaluate();
			PointsTotal += test.Points;
		}
	}
}
