using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Testing;

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

public interface IAssignment{
	AssignmentResult RunTests(string programName);
	void AddTest(string testName, FileInfo outputFile, int points, int time, FileInfo? inputFile, FileInfo? argumentsFile);
	void RemoveTest(string testName);
}

// TODO static class at least partially? for Create, Add, RunTests ...
public class Assignment : IAssignment{
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

	public AssignmentResult RunTests(string programName){
		var tests = DeserializeTests();
		var temp = new List<TestLog>();

		foreach(var test in tests){
			temp.Add( test.Run(programName) );
		}

		Result = new AssignmentResult(temp);
		return Result;
	}

	private bool ValidTestName(string testName){
		return !Directory.Exists($"Data/Assignments/{testName}");
	}

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
		string testDirectory = $"{Name}/{testName}";

		if( Directory.Exists(testDirectory) ){
			Directory.Delete(testDirectory, true);
		}
	}
}
