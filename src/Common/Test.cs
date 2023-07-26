using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testing;

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test
// ITest { void Run; ... } it should be abstract class because implementation for both test would be really similar 

public enum TestResult { NotExecuted, Correct, OutputMismatch, TimeExceeded, ExceptionError, CompilationError }

public readonly struct TestLog{
	public readonly string Name;
	public readonly int ExitCode = 0;
	public readonly TestResult Result = TestResult.NotExecuted;
	public readonly string Stdout;
	public readonly string Stderr;
	public readonly int Points;

	public TestLog(string name, int exitCode, TestResult result, string stdout, string stderr, int points){
		Name = name;
		ExitCode = exitCode;
		Result = result;
		Stdout = stdout;
		Stderr = stderr;
		Points = points;
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
	public TestLog? Log;

	private ProcessStartInfo SetProcessStartInfo(string programName){
		var startInfo = new ProcessStartInfo();
		startInfo.FileName = "python3";
		startInfo.Arguments = programName;
		// TODO handle arguments better;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardError = true;
		startInfo.RedirectStandardOutput = true;
		if( inputFileName ) startInfo.RedirectStandardInput = true;

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

	private bool CorrectOutput(Process p){
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

	public TestLog Run(string programName){
		var process = new Process(){ StartInfo = SetProcessStartInfo(programName) };

		var result = TestResult.Correct;

		try{
			process.Start();

			SetInput(process);

			process.WaitForExit(processorTime);

			if( !process.HasExited ){
				process.Kill();
				process.WaitForExit();
				result = TestResult.TimeExceeded;
			} else{
				if( CorrectOutput(process) ) result = TestResult.Correct;
				else result = TestResult.OutputMismatch;
			}
		}
		catch( Exception ex ){
			Console.WriteLine($"exception {ex}");
		}
		finally{
			int exitCode = process.ExitCode;
			string stdout = process.StandardOutput.ReadToEnd();
			string stderr = process.StandardError.ReadToEnd();
			int points = result == TestResult.Correct ? maxPoints : 0;
			process.Close();
			// TODO save TestLog in users directory
			Log = new TestLog(Name, exitCode, result, stdout, stderr, points);
		}

		return (TestLog)Log;
	}
	// TODO public string Interpreter;

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
