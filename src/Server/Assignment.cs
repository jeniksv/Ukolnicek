using System.Diagnostics;

namespace AppLogic;

// TODO interface for test, so we can have more implementations of test like unittest based or program based
// maybe Assignment should be more generic then test

public enum TestResult { NotExecuted, Correct, OutputMismatch, TimeExceeded, ExceptionError, CompilationError }

public class Test{
	public string? Name;
	private int maxPoints;
	private int processorTime;
	private string? commandLineArguments;
	private string? inputFileName;
	private string? expectedOutputFileName;

	public int Points => Result == TestResult.Correct ? maxPoints : 0;
	public TestResult Result = TestResult.NotExecuted;
	public string? TestOutput; // TODO ability to hide this field
	public int ExitCode;

	private ProcessStartInfo SetProcessStartInfo(string programName){
		var startInfo = new ProcessStartInfo();
		startInfo.FileName = "python3";
		startInfo.Arguments = programName;
		// TODO handle arguments better;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardError = true;
		startInfo.RedirectStandardOutput = true;
		if( inputFileName != null ) startInfo.RedirectStandardInput = true;
		
		return startInfo;
	}

	private void SetInput(Process p){
		if( !p.StartInfo.RedirectStandardInput || inputFileName == null ) return;

		using(var reader = new StreamReader(inputFileName)){
			string? line = reader.ReadLine();
			
			while( line != null){
				p.StandardInput.WriteLine(line);
				line = reader.ReadLine();
			}	
		}
	}

	private bool CorrectOutput(Process p){
		if( expectedOutputFileName == null ) return false;
		
		using(var reader = new StreamReader(expectedOutputFileName)){
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
		
		public Builder WithName(string name){ test.Name = name; return this; }
		public Builder WithExpectedOutputFileName(string file){ test.expectedOutputFileName = file; return this; }
		public Builder WithMaxPoints(int points){ test.maxPoints = points; return this; }
		public Builder WithProcessorTime(int time){ test.processorTime = time; return this; }
		public Builder WithInputFileName(string file){ test.inputFileName = file; return this; }
		public Builder WithCommandLineArguments(string cmd){ test.commandLineArguments = cmd; return this; }

		public Test Build() => test;
	}
}

public class Assignment{
	// TODO serialize + deserialize
	public string Name;
	public List<Test> tests;
	public int PointsTotal;

	public Assignment(string name){
		// debug version
		Name = name;
		tests = new List<Test>();

		if( Directory.Exists($"Data/Assignments/{name}") ){
			return; // debug version
			throw new InvalidOperationException("assignment name already exists");
		}

		//Name = name;
		//tests = new List<Test>();

		Directory.CreateDirectory($"Data/Assignments/{name}");
	}

	public void RunTests(string programName){
		PointsTotal = 0;

		foreach(var test in tests){
			test.Run(programName);
			PointsTotal += test.Points;
		}
	}

	private bool ValidTestName(string name){
		foreach(var test in tests){
			if(test.Name == name) return false;
		}

		return true;
	}

	public void AddTest(string testName, string eOFN, int mP = 1, int pT = 5000,
			string? iFN = null, string? cLA = null){
		if( !ValidTestName( testName) ) throw new InvalidOperationException("test name already exists");

		// TODO copy these files to proper place
		eOFN = $"Data/Assignments/{Name}/{eOFN}";
		if( iFN != null ) iFN = $"Data/Assignments/{Name}/{iFN}";

		var test = new Test.Builder().WithName(testName).WithExpectedOutputFileName(eOFN).WithMaxPoints(mP)
			.WithProcessorTime(pT).WithInputFileName(iFN).WithCommandLineArguments(cLA).Build();

		tests.Add(test);
	}

	public void RemoveTest(string testName){
	}
}
