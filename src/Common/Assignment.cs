using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppLogic;

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test
// ITest { void Run; ... } it should be abstract class because implementation for both test would be really similar 

public enum TestResult { NotExecuted, Correct, OutputMismatch, TimeExceeded, ExceptionError, CompilationError }

public readonly struct TestLog{
	public readonly int ExitCode = 0;
	public readonly TestResult Result = TestResult.NotExecuted;
	public readonly string Stdout;
	public readonly string Stderr;
	public readonly int Points;

	public TestLog(int exitCode, TestResult result, string stdout, string stderr, int points){
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

	// these fields are not necessary, it is ok hold TestLog structure
	[JsonIgnore]
	public int Points => Result == TestResult.Correct ? maxPoints : 0;
	[JsonIgnore]
	public TestResult Result = TestResult.NotExecuted;
	[JsonIgnore]
	public string? TestOutput; // TODO ability to hide this field
	[JsonIgnore]
	public int ExitCode;
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
	
	// TODO security -> test should run in virtual env
	// TODO return value TestLog
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
				Result = TestResult.TimeExceeded;
			} else{
				if( CorrectOutput(process) ) Result = TestResult.Correct;
				else Result = TestResult.OutputMismatch;
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
			// int exitCode, TestResult result, string stdout, string stderr, int points)
			Log = new TestLog(exitCode, result, stdout, stderr, points);
		}

		return (TestLog)Log;
	}
	// TODO public string Interpreter;
	
	public class Builder{
		private Test test;

		public Builder(){
			test = new Test();
		}
		

		// maybe builder should handle file copying?
		public Builder WithName(string name){ test.Name = name; return this; }
		//public Builder WithExpectedOutputFileName(){ test.expectedOutputFileName = true; return this; }
		public Builder WithMaxPoints(int points){ test.maxPoints = points; return this; }
		public Builder WithProcessorTime(int time){ test.processorTime = time; return this; }
		//public Builder WithInputFileName(){ test.inputFileName = true; return this; }
		//public Builder WithCommandLineArguments(){ test.commandLineArguments = true; return this; }

		public Test Build() => test;
	}
}

// maybe assignment can hold only name and then all methods dont have to be static
// but then assignment can hold something like log test etc ...
public class Assignment{
	public List<string> testNames;
	public string Name;
	public int PointsTotal = 0;

	public List<TestLog> Result;

	public Assignment(string name){
		Name = GetFullAssignmentName(name);
		
		if( !Exists(Name) ) Directory.CreateDirectory($"{Name}");

		testNames = new List<string>(Directory.GetDirectories(Name));
	}

	private static bool Exists(string name){
		name = GetFullAssignmentName(name);

		foreach(var assignment in Directory.GetDirectories($"Data/Assignments/")){
			if(assignment == name) return true;
		}

		return false;
	}

	private static string GetFullAssignmentName(string name) => !name.StartsWith($"Data/Assignments/") ? $"Data/Assignments/{name}" : name;

	public void AddTask(FileInfo task){
		task.MoveTo($"{Name}/README.md");
	}

	public static void Delete(string name){
		name = GetFullAssignmentName(name);

		if( Exists(name) ) Directory.Delete(name, true);
	}

	private List<Test> DeserializeTests(){
		List<Test> tests = new List<Test>();

		foreach(var testName in Directory.GetDirectories($"{Name}")){
			var config = File.ReadAllText($"{testName}/config.json");
			var test = JsonSerializer.Deserialize<Test>(config);
			tests.Add(test!);
		}

		return tests;
	}

	public List<TestLog> RunTests(string programName){
		PointsTotal = 0;
		var tests = DeserializeTests();
		var result = new List<TestLog>();

		foreach(var test in tests){
			test.Run(programName);
			PointsTotal += test.Points;
		}

		return result;
	}

	private bool ValidTestName(string testName) => !Directory.Exists($"Data/Assignments/{testName}");

	private void MoveFiles(string directory, FileInfo outputFile, FileInfo? inputFile, FileInfo? argumentsFile){
		outputFile.MoveTo($"{directory}/out");

		if( inputFile != null ) inputFile.MoveTo($"{directory}/in");
		
		if( argumentsFile != null ) argumentsFile.MoveTo($"{directory}/args");
	}

	public void AddTest(string testName, FileInfo outputFile, int points, int time, FileInfo? inputFile, FileInfo? argumentsFile){
		string testDirectory = $"{Name}/{testName}";

		if( !ValidTestName(testDirectory) ){
			throw new InvalidOperationException("test name already exists");
		}
		
		Directory.CreateDirectory(testDirectory);

		MoveFiles(testDirectory, outputFile, inputFile, argumentsFile);

		var test = new Test.Builder().WithName(testDirectory).WithMaxPoints(points).WithProcessorTime(time).Build();

		var config = JsonSerializer.Serialize<Test>(test);
		File.WriteAllText($"{testDirectory}/config.json", config);
	}

	public void RemoveTest(string testName){
		// TODO
	}
}
