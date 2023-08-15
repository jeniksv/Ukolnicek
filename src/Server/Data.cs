using Ukolnicek.Communication;

namespace Ukolnicek.Server;

public static class Data{
	public static bool UserExists(string name) => Directory.Exists($"Data/Users/{name}");

	public static bool GroupExists(string name) => File.Exists($"Data/Users/Groups/{name}");

	public static bool AssignmentExists(string name) => Directory.Exists($"Data/Assignments/{name}");

	public static bool SolutionExists(string userName, string assignmentName, string solutionName) => Directory.Exists($"Data/Users/{userName}/{assignmentName}/{solutionName}");

	public static bool TaskDescriptionExists(string assignmentName) => File.Exists($"Data/Assignments/{assignmentName}/README.md");

	public static bool TestExists(string assignmentName, string testName) => Directory.Exists($"Data/Assignments/{assignmentName}/{testName}");

	public static string[] UsersInGroup(string name) => File.ReadAllLines($"Data/Users/Groups/{name}");

	public static bool UserHasAssignment(string userName, string assignmentName) => Directory.Exists($"Data/Users/{userName}/{assignmentName}");

	public static T Get<T>(IRequest<object> update) => (T)(update.Data ?? throw new InvalidOperationException($""));	
}
