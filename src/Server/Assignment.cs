using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppLogic;

// TODO TODO TODO make these data persistent!!!

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test

public enum TestResult { NotExecuted, Correct, OutputMismatch, TimeExceeded, ExceptionError, CompilationError }

public class TestLog{
}

public class Test{
	public string? Name { get; set; } // string formated: $"Data/Assignments/{A.Name}/{T.Name}"
	public int maxPoints { get; set; }
	public int processorTime { get; set; }
	
	private bool commandLineArguments => File.Exists($"{Name}/args"); // name of file is always args
	private bool inputFileName => File.Exists($"{Name}/in"); // name of file is always in
	private bool expectedOutputFileName => File.Exists($"{Name}/out"); // name of file is always out

	[JsonIgnore]
	public int Points => Result == TestResult.Correct ? maxPoints : 0;
	[JsonIgnore]
	public TestResult Result = TestResult.NotExecuted;
	[JsonIgnore]
	public string? TestOutput; // TODO ability to hide this field
	[JsonIgnore]
	public int ExitCode;

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
	public void Run(string programName){
		var process = new Process(){ StartInfo = SetProcessStartInfo(programName) };

		Result = TestResult.Correct;

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
			ExitCode = process.ExitCode;
			Console.WriteLine($"exit code {process.ExitCode}"); // debug logger
			Console.WriteLine($"err {process.StandardError.ReadToEnd()}"); // debug logger
			Console.WriteLine(); // debug logger should write smt like result of each test
			process.Close();
		}
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

public class Assignment{
	// static field which should hold names of all
	public static List<string> NameList;

	static Assignment(){
		NameList = new List<string>(Directory.GetDirectories($"Data/Assignments"));
	}

	public static void Create(string name, FileInfo task){
		if( AssignmentExists(name) ){
			throw new InvalidOperationException("assignment already exists");
		}

		Directory.CreateDirectory($"Data/Assignments/{name}");

		task.MoveTo($"Data/Assignments/{name}/README.md");

		NameList.Add(name);
	}

	public static void RunTests(string assignmentName, string programName){
		if( !assignmentName.StartsWith($"Data/Assignments/") ) assignmentName = $"Data/Assignments/{assignmentName}";

		int PointsTotal = 0;
		List<Test> tests = new List<Test>();

		foreach(var testName in Directory.GetDirectories($"{assignmentName}")){
			var config = File.ReadAllText($"{testName}/config.json");
			var test = JsonSerializer.Deserialize<Test>(config);
			tests.Add(test!);
		}

		foreach(var test in tests){
			test.Run(programName);
			PointsTotal += test.Points;
		}

		Console.WriteLine($"points: {PointsTotal}");

		// it will be nice to create file with test logs etc.
	}

	private static bool AssignmentExists(string name){
		if( !name.StartsWith($"Data/Assignments/") ) name = $"Data/Assignments/{name}";

		foreach(var assignment in NameList){
			if(assignment == name) return true;
		}

		return false;
	}

	private static bool ValidTestName(string testName) => !Directory.Exists($"Data/Assignments/{testName}");

	private static void MoveFiles(string directory, FileInfo outputFile, FileInfo? inputFile, FileInfo? argumentsFile){
		outputFile.MoveTo($"{directory}/out");

		if( inputFile != null ) inputFile.MoveTo($"{directory}/in");
		
		if( argumentsFile != null ) argumentsFile.MoveTo($"{directory}/args");
	}

	public static void AddTest(string assignmentName, string testName, FileInfo outputFile, int points = 1,
			int time = 5000, FileInfo? inputFile = null, FileInfo? argumentsFile = null){
		
		if( !AssignmentExists(assignmentName) ){
			throw new InvalidOperationException("assignment does not exist");
		}

		if( !ValidTestName($"{assignmentName}/{testName}") ){
			throw new InvalidOperationException("test name already exists");
		}

		string testDirectory = $"Data/Assignments/{assignmentName}/{testName}";
		
		Directory.CreateDirectory(testDirectory);

		MoveFiles(testDirectory, outputFile, inputFile, argumentsFile);

		var test = new Test.Builder().WithName(testDirectory).WithMaxPoints(points).WithProcessorTime(time).Build();

		var config = JsonSerializer.Serialize<Test>(test);
		File.WriteAllText($"{testDirectory}/config.json", config);
	}

	public void RemoveTest(string testName){
	}
}
