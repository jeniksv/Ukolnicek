using System.IO;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client;

public class ConsoleUI : IUserInterface {
	public string Username { get; set; }
	public string[] CommandOptions { get; set; }

	public string GetUsername(){
		return Console.ReadLine()!;
	}

	public void AskUsername(){
		Console.Write("username: ");
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

	private string TabMatch(string prefix, Dictionary<string, RequestEnum> options){
		var result = new List<string>();

		foreach(var pair in options){
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
		Console.Write($"{Username} > ");
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
		// TODO command options diff for admin
		Console.WriteLine("show-assignment [assignment name]");
		Console.WriteLine("show-assignments");
		Console.WriteLine("show-solution [assignment name] [solution name]");
		Console.WriteLine("add-assignment [assignment name]");
		Console.WriteLine("add-test [assignment name] [test name] --out [file] --in [file] --args [file] --time --points");
		Console.WriteLine("add-task-description [assignment name] [file]");
		Console.WriteLine("submit-solution [assignment name] [file]");
		Console.WriteLine("assign-task [assignment name] [student name]");
		Console.WriteLine("exit");
	}

	public RequestEnum GetCommand(out string[] args){
		var commands = new Dictionary<string, RequestEnum> {
			{"show-assignment", RequestEnum.ShowAssignment},
			{"show-assignments", RequestEnum.ShowAssignments},
			{"show-solution", RequestEnum.ShowSolution},
			{"submit-solution", RequestEnum.SubmittedSolution},
			{"add-assignment", RequestEnum.CreateAssignment},
			{"add-test", RequestEnum.AddTest},
			{"add-task-description", RequestEnum.AddTaskDescription},
			{"assign-task", RequestEnum.AssignTask},
			{"exit", RequestEnum.Exit},
		};

		string command = "";
		ConsoleKeyInfo key;

		ShowPrompt();
		
		while( true ){
			key = Console.ReadKey(true);
			if( key.Key == ConsoleKey.Enter ){
				Console.WriteLine();
				var commandSplit = command.Split();
				var request = commandSplit[0];
				// TODO check for valid arguments
				args = new string[commandSplit.Length - 1];
				Array.Copy(commandSplit, 1, args, 0, commandSplit.Length - 1);
					
				if( commands.ContainsKey(request) ){
					return commands[request];
				} else {
					if( request == "help" ) HelpCommand();
					else if(request != "" ) Console.WriteLine("Invalid command");
					return GetCommand(out args);
				}
			}

			// TODO arrow keys add command history
			if( key.Key == ConsoleKey.Tab ){
				command = TabMatch(command, commands);
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
				Console.WriteLine($"{testLog.Stdout}");
				Console.WriteLine($"Stdou expected:");
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
		foreach(var a in assignments){
			Console.WriteLine($"{ExtractName(a)}");
		}
	}

	public void InvalidArguments(){
		Console.WriteLine("Invalid arguments");	
	}
}
