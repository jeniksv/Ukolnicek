using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ukolnicek.Testing;

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test
// ITest { void Run; ... } it should be abstract class because implementation for both test would be really similar 
// TODO strategy pattern for different programming languages

/// <summary>
/// Represents the possible results of a test.
/// </summary>
public enum TestResult { NotExecuted, Correct, OutputMismatch, TimeExceeded, ExceptionError, CompilationError }


/// <summary>
/// Represents result of one test. Expected output and maximum points are included.
/// </summary>
public readonly struct TestLog{
	public readonly string Name { get; }
	public readonly int ExitCode { get; }
	public readonly TestResult Result { get; }
	public readonly string Stdout { get; }
	public readonly string StdoutExpected { get; }
	public readonly string Stderr { get; }
	public readonly int Points { get; }
	public readonly int MaxPoints { get; }

	public TestLog(string name, int exitCode, TestResult result, string stdout, string stderr, int points, int maxPoints, string stdoutExpected){
		Name = name;
		ExitCode = exitCode;
		Result = result;
		Stdout = stdout;
		Stderr = stderr;
		Points = points;
		MaxPoints = maxPoints;
		StdoutExpected = stdoutExpected;
	}
}

public class Test{
	public string? Name { get; set; } // string formated: $"Data/Assignments/{A.Name}/{T.Name}"
	public int maxPoints { get; set; }
	public int processorTime { get; set; }

	private bool commandLineArguments => File.Exists($"{Name}/args"); // name of file is always args
	private bool inputFileName => File.Exists($"{Name}/in"); // name of file is always in
	private bool expectedOutputFileName => File.Exists($"{Name}/out"); // name of file is always out

	[JsonIgnore]
	public TestLog Log;

	private ProcessStartInfo SetProcessStartInfo(string programName){
		var startInfo = new ProcessStartInfo();
		startInfo.FileName = "python3";
		startInfo.ArgumentList.Add(programName);
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardError = true;
		startInfo.RedirectStandardOutput = true;
		
		if( inputFileName ){
			startInfo.RedirectStandardInput = true;
		}

		if( commandLineArguments ){
			foreach(var arg in File.ReadAllText($"{Name}/args").Split()){
				startInfo.ArgumentList.Add(arg);
			}
		}

		return startInfo;
	}

	private void SetInput(Process p){
		if( !p.StartInfo.RedirectStandardInput || !inputFileName ) return;

		using(var reader = new StreamReader($"{Name}/in")){
			string? line = reader.ReadLine();

			while( line != null){
				p.StandardInput.WriteLine(line);
				line = reader.ReadLine();
			}
		}
	}

	private bool CorrectOutput(Process p){ // more efficient way, but i will send all file anyway
		if( !expectedOutputFileName ) return false;

		using(var reader = new StreamReader($"{Name}/out")){
			string? lineFile = reader.ReadLine();
			string? lineOutput = p.StandardOutput.ReadLine();

			while( lineFile != null || lineOutput != null){
				if( lineFile != lineOutput ) return false;
				lineFile = reader.ReadLine();
				lineOutput = p.StandardOutput.ReadLine();
			}

			if( lineFile == null && lineOutput == null ) return true;
			else return false;
		}
	}

	private bool CorrectOutput(string actual, string expected){
		if( !expectedOutputFileName ) return false;

		return actual == expected;
	}

	/// <summary>
	/// Runs the test using the specified program.
	/// </summary>
	/// <param name="programName">The name of the program to run.</param>
	/// <returns>The test log containing the results.</returns>
	public TestLog Run(string programName){
		var process = new Process(){ StartInfo = SetProcessStartInfo(programName) };

		var result = TestResult.Correct;
		var stdout = "";
		var stdoutExpected = File.ReadAllText($"{Name}/out");

		try{
			process.Start();

			SetInput(process);

			process.WaitForExit(processorTime);

			if( !process.HasExited ){
				process.Kill();
				process.WaitForExit();
				result = TestResult.TimeExceeded;
			} else if( process.ExitCode == 0 ){
				stdout = process.StandardOutput.ReadToEnd();
				if( CorrectOutput(stdout, stdoutExpected) ) result = TestResult.Correct;
				else result = TestResult.OutputMismatch;
			} else {
				result = TestResult.ExceptionError;	
			}
		}
		catch( Exception ex ){
			Console.WriteLine($"exception {ex}");
		}
		finally{
			int exitCode = process.ExitCode;
			// string stdout = process.StandardOutput.ReadToEnd();
			string stderr = process.StandardError.ReadToEnd();
			int points = result == TestResult.Correct ? maxPoints : 0;
			process.Close();
			Log = new TestLog(Name!, exitCode, result, stdout, stderr, points, maxPoints, stdoutExpected);
		}

		return (TestLog)Log;
	}

	/// <summary>
	/// A builder class for creating Test instances.
	/// </summary>
	public class Builder{
		private Test test;

		public Builder(){
			test = new Test();
		}

		public Builder WithName(string name){ test.Name = name; return this; }
		//public Builder WithExpectedOutputFileName(){ test.expectedOutputFileName = true; return this; }
		public Builder WithMaxPoints(int points){ test.maxPoints = points; return this; }
		public Builder WithProcessorTime(int time){ test.processorTime = time; return this; }
		//public Builder WithInputFileName(){ test.inputFileName = true; return this; }
		//public Builder WithCommandLineArguments(){ test.commandLineArguments = true; return this; }

		public Test Build() => test;
	}
}
