using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ukolnicek.Testing;

public struct AssignmentResult{
	public List<TestLog> TestLogs { get; set; }
	public int CorrectTests { get; set; }
	public int IncorrectTests { get; set; }
	public int SkippedTests { get; set; }

	public int PointsTotal { get; set; }

	public AssignmentResult(List<TestLog> logs){
		TestLogs = logs;

		foreach(var log in TestLogs){
			PointsTotal += log.Points;

			if( log.Result == TestResult.Correct ){
				CorrectTests++;
			} else if( log.Result == TestResult.OutputMismatch ){
				IncorrectTests++;
			} else{
				SkippedTests++;
			}
		}
	}
}

public class Assignment{
	public List<string> testNames;
	public string Name;

	public AssignmentResult Result;

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

	private static string GetFullAssignmentName(string name){
		return !name.StartsWith($"Data/Assignments/") ? $"Data/Assignments/{name}" : name;
	}

	public static void Create(string assignmentName){
		assignmentName = GetFullAssignmentName(assignmentName);

		if( Directory.Exists(assignmentName) ){
			throw new InvalidOperationException("assignment already exists");
		}
			
		Directory.CreateDirectory($"{assignmentName}");
	}

	public static string GetTaskDescription(string assignmentName){
		assignmentName = GetFullAssignmentName(assignmentName);
		return File.ReadAllText($"{assignmentName}/README.md");
	}

	public static void AddTaskDescription(string assignmentName, string contents){
		assignmentName = GetFullAssignmentName(assignmentName);
		File.WriteAllText($"{assignmentName}/README.md", contents);
	}

	public static void RemoveTaskDescription(string assignmentName){
		assignmentName = GetFullAssignmentName(assignmentName);
		if( File.Exists($"{assignmentName}/README.md") ) File.Delete($"{assignmentName}/README.md");
	}

	public static void Remove(string name){
		name = GetFullAssignmentName(name);

		if( Exists(name) ) Directory.Delete(name, true);
	}

	private static List<Test> DeserializeTests(string assignmentName){
		assignmentName = GetFullAssignmentName(assignmentName);
		
		List<Test> tests = new List<Test>();

		foreach(var testName in Directory.GetDirectories($"{assignmentName}")){
			var config = File.ReadAllText($"{testName}/config.json");
			var test = JsonSerializer.Deserialize<Test>(config);
			tests.Add(test!);
		}

		return tests;
	}

	public static AssignmentResult RunTests(string assignmentName, string programName){
		assignmentName = GetFullAssignmentName(assignmentName);

		var tests = DeserializeTests(assignmentName);
		var temp = new List<TestLog>();

		foreach(var test in tests){
			var t = test.Run(programName);
			temp.Add( test.Run(programName) );
		}

		return new AssignmentResult(temp);
	}

	private static bool ValidTestName(string testName){
		return !Directory.Exists($"Data/Assignments/{testName}");
	}

	private static void MoveFiles(string directory, byte[] outputFile, byte[] inputFile, byte[] argsFile){
		if( outputFile != null ) File.WriteAllBytes($"{directory}/out", outputFile);

		if( inputFile != null ) File.WriteAllBytes($"{directory}/in", inputFile);
		
		if( argsFile != null ) File.WriteAllBytes($"{directory}/args", argsFile);
	}

	public static void AddTest(string assignmentName, string testName, byte[] outputFile, byte[] inputFile, byte[] argsFile, int time, int points){
		string testDirectory = $"Data/Assignments/{assignmentName}/{testName}";

		if( !Directory.Exists($"Data/Assignments/{assignmentName}") ){
			throw new InvalidOperationException("assignment does not exist");
		}

		if( !ValidTestName(testDirectory) ){
			throw new InvalidOperationException("test name already exists");
		}
		
		Directory.CreateDirectory(testDirectory);

		MoveFiles(testDirectory, outputFile, inputFile, argsFile);

		var test = new Test.Builder().WithName(testDirectory).WithMaxPoints(points).WithProcessorTime(time).Build();

		var config = JsonSerializer.Serialize<Test>(test);
		File.WriteAllText($"{testDirectory}/config.json", config);
	}

	public static void RemoveTest(string assignmentName, string testName){
		assignmentName = GetFullAssignmentName(assignmentName);

		if( Directory.Exists($"{assignmentName}/{testName}") ){
			Directory.Delete($"{assignmentName}/{testName}", true);
		}
	}
}
