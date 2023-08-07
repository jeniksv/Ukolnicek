using System.IO;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client;

public class ConsoleUI : IUserInterface {
	private Dictionary<string, RequestEnum> commandOptions;
	private User user;

	public ConsoleUI(){
		commandOptions = new Dictionary<string, RequestEnum> {
			{"show-assignment", RequestEnum.ShowAssignment},
			{"show-assignments", RequestEnum.ShowAssignments},
			{"show-solution", RequestEnum.ShowSolution},
			{"show-task-description", RequestEnum.ShowTaskDescription},
			{"add-solution", RequestEnum.AddSolution},
			{"exit", RequestEnum.Exit},
		};
	}

	public void SetUser(User u){
		user = u;

		var adminOptions = new Dictionary<string, RequestEnum> {
                        {"add-assignment", RequestEnum.AddAssignment},
                        {"add-test", RequestEnum.AddTest},
                        {"add-task-description", RequestEnum.AddTaskDescription},
                        {"remove-assignment", RequestEnum.RemoveAssignment},
                        {"remove-test", RequestEnum.RemoveTest},
                        {"remove-task-description", RequestEnum.RemoveTaskDescription},
                        {"assign-task", RequestEnum.AssignTask},
                        {"unassign-task", RequestEnum.UnassignTask},
		};

		if(user is Admin){
			foreach(var pair in adminOptions){
				commandOptions[pair.Key] = pair.Value;
			}	
		}
	}

	public void MainLoop(){
		if( user == null ) return;

		bool running = true;

		while( running ){
			var command = GetCommand(out string[] args);

			if( command == RequestEnum.Exit ) break;

			var data = user.HandleCommand(command, args);
			
			// TODO tohle zabal do funkce
			switch( command ){
				case RequestEnum.ShowAssignments:
					ShowAssignments((string[])data);
					break;
				case RequestEnum.ShowAssignment:
					ShowAssignment((string[])data);
					break;
				case RequestEnum.ShowSolution:
					ShowSolution((AssignmentResult)data);
					break;
				case RequestEnum.ShowTaskDescription:
					ShowTaskDescription((string)data);
					break;
			}
		}
	}

	public string GetUsername(){
		return Console.ReadLine()!;
	}

	public void AskUsername(){
		Console.Write("username: ");
	}

	public void AccountExists(){
		Console.WriteLine("That username is taken. Try another one.");
	}

	public string GetPassword(){
		string password = ""; // readability of code is more important here then performance (StringBuilder)
		ConsoleKeyInfo key;

		while( true ){
			key = Console.ReadKey(true);

			if( key.Key == ConsoleKey.Enter ){
				Console.WriteLine();
				return password;
			}

			if( key.Key != ConsoleKey.Backspace ){
				password += key.KeyChar;
			} else if( password.Length > 0 ){
				password = password.Remove(password.Length - 1);
			}
		}
	}

	public void AskPassword(){
		Console.Write("password: ");
	}

	public void InvalidLogin(){
		Console.WriteLine("Permission denied, please try again.");
	}

	private string TabMatch(string prefix){
		var result = new List<string>();

		foreach(var pair in commandOptions){
			if( pair.Key.StartsWith(prefix) ){
				result.Add(pair.Key);
			}
		}

		if( result.Count == 0 ){
			return prefix;
		}
		
		if( result.Count == 1 ){
			for(int i=0; i<prefix.Length; i++){
				Console.Write("\b \b");
			}

			Console.Write(result[0]);
			
			return result[0];
		}

		Console.WriteLine();

		foreach(var command in result){
			Console.Write($"{command} ");
		}

		Console.WriteLine();
		ShowPrompt();
		prefix = LongestCommonPrefix(result);
		Console.Write(prefix);
		return prefix;
	}

	private string LongestCommonPrefix(List<string> commands){
		commands.Sort();
		var first = commands[0];
		var last = commands[^1];

		for(int i=0; i < Math.Min(first.Length, last.Length); i++){
			if( first[i] != last[i] ){
				return first.Substring(0, i);
			}
		}

		return first.Substring(0, Math.Min(first.Length, last.Length));
	}

	private void ShowPrompt(){
		Console.Write($"{user.Name} > ");
	}

	private string DeleteChar(string command){
		if( command.Length > 0 ){
			command = command.Remove(command.Length - 1);
			Console.Write("\b \b");
			return command;
		}

		return "";
	}

	private void HelpCommand(){
		Console.WriteLine("exit");
		Console.WriteLine("show-assignment [assignment name]");
		Console.WriteLine("show-assignments");
		Console.WriteLine("show-solution [assignment name] [solution name]");
		Console.WriteLine("add-solution [assignment name] [file]");

		if(user is Admin){
		Console.WriteLine("add-assignment [assignment name]");
		Console.WriteLine("add-test [assignment name] [test name] --out [file] --in [file] --args [file] --time --points");
		Console.WriteLine("add-task-description [assignment name] [file]");
		
		Console.WriteLine("assign-task [assignment name] [student name]");
		Console.WriteLine("unassign-task [assignment name] [student name]");
		
		Console.WriteLine("remove-assignment [assignment name]");
		Console.WriteLine("remove-test [assignment name] [test name]");
		Console.WriteLine("remove-task-description [assignment name]");
		}
	}

	public RequestEnum GetCommand(out string[] args){
		string command = "";
		ConsoleKeyInfo key;

		ShowPrompt();
		
		while( true ){
			key = Console.ReadKey(true);
			if( key.Key == ConsoleKey.Enter ){
				Console.WriteLine();
				var commandSplit = command.Split();
				var request = commandSplit[0];
				args = new string[commandSplit.Length - 1];
				Array.Copy(commandSplit, 1, args, 0, commandSplit.Length - 1);
					
				if( commandOptions.ContainsKey(request) ){
					return commandOptions[request];
				} else {
					if( request == "help" ) HelpCommand();
					else if(request != "" ) Console.WriteLine("Invalid command");
					return GetCommand(out args);
				}
			}

			// TODO arrow keys add command history
			if( key.Key == ConsoleKey.Tab ){
				command = TabMatch(command);
			} else if( key.Key == ConsoleKey.Backspace ){
				command = DeleteChar(command);
			} else {
				command += key.KeyChar;
				Console.Write(key.KeyChar);
			}
		}
	}

	private string ExtractName(string name){
		return Path.GetFileName(name);
	}

	public void ShowSolution(AssignmentResult result){
		foreach(var testLog in result.TestLogs){
			Console.Write($"Test: {ExtractName(testLog.Name)}");
			
			if( testLog.Result == TestResult.Correct ){
				Console.WriteLine(" ... OK");
				continue;
			}

			Console.WriteLine(" ... FAILED");

			if( testLog.Result == TestResult.OutputMismatch ){
				Console.WriteLine("Stdout actual:");
				Console.Write($"{testLog.Stdout}");
				Console.WriteLine($"Stdout expected:");
				Console.Write($"{testLog.StdoutExpected}");
				continue;
			}

			if( testLog.Result == TestResult.TimeExceeded ){
				Console.WriteLine("Time exceeded");
				continue;
			}

			Console.WriteLine("Stderr:");
			Console.WriteLine($"{testLog.Stderr}");
		}

		Console.WriteLine();
		Console.Write($"Passed: {result.CorrectTests}, ");
		Console.Write($"Failed: {result.IncorrectTests}, ");
		Console.WriteLine($"Skipped: {result.SkippedTests}");
		Console.WriteLine($"Points: {result.PointsTotal}");
	}

	public void ShowAssignments(string[] assignments){
		foreach(var assignment in assignments){
			Console.WriteLine($"{ExtractName(assignment)}");
		}
	}

	public void ShowAssignment(string[] assignment){
		ShowTaskDescription(assignment[0]);
		Console.WriteLine();

		for(int i=1; i < assignment.Length; i++){
			Console.WriteLine(ExtractName(assignment[i])); // TODO display basic info about solution
		}
	}

	public void ShowTaskDescription(string description){
		Console.Write(description);
	}

	public void InvalidArguments(){
		Console.WriteLine("Invalid arguments");	
	}
}
