using System.IO;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client;

public class ConsoleUI : IUserInterface {
	private Dictionary<string, RequestEnum>? commandOptions;
	private User? user;
	private ConsoleReader reader;

	public ConsoleUI(){
		reader = new ConsoleReader();
	}

	public void SetUser(User u){
		user = u;

		commandOptions = new Dictionary<string, RequestEnum> {
			{"show-assignment", RequestEnum.ShowAssignment},
			{"show-assignments", RequestEnum.ShowAssignments},
			{"show-solution", RequestEnum.ShowSolution},
			{"show-task-description", RequestEnum.ShowTaskDescription},
			{"add-solution", RequestEnum.AddSolution},
			{"exit", RequestEnum.Exit},
			{"download-solution", RequestEnum.DownloadSolution},
		};

		var adminOptions = new Dictionary<string, RequestEnum> {
			{"show-group", RequestEnum.ShowGroup},
			{"show-groups", RequestEnum.ShowGroups},
			{"show-users", RequestEnum.ShowUsers},
			{"add-assignment", RequestEnum.AddAssignment},
			{"add-test", RequestEnum.AddTest},
			{"add-task-description", RequestEnum.AddTaskDescription},
			{"add-admin", RequestEnum.AddAdmin},
			{"add-group", RequestEnum.AddGroup},
			{"remove-assignment", RequestEnum.RemoveAssignment},
			{"remove-test", RequestEnum.RemoveTest},
			{"remove-task-description", RequestEnum.RemoveTaskDescription},
			{"remove-group", RequestEnum.RemoveGroup},
			{"assign-task", RequestEnum.AssignTask},
			{"unassign-task", RequestEnum.UnassignTask},
		};

		if(user is Admin){
			foreach(var pair in adminOptions){
				commandOptions[pair.Key] = pair.Value;
			}	
		}

		reader.SetPropertiesOptions(user.Name, user is Admin, commandOptions);
	}

	public void MainLoop(){
		bool running = true;

		while( running ){
			var command = reader.GetCommand(out string[] args);

			if( command == RequestEnum.Exit ) break;

			object data = null;

			try{
				data = user.HandleCommand(command, args);
			} catch( Exception ){
				Console.WriteLine("Invalid arguments");
			}
			// TODO tohle zabal do funkce ShowResponse();
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
				case RequestEnum.ShowGroup:
					ShowGroup((string)data);
					break;
				case RequestEnum.ShowGroups:
					ShowGroups((string[])data);
					break;
				case RequestEnum.ShowUsers:
					ShowUsers((string[])data);
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

	private string ExtractName(string name){
		return Path.GetFileName(name);
	}

	public void ShowSolution(AssignmentResult result){
		foreach(var testLog in result.TestLogs){
			Console.Write($"{ExtractName(testLog.Name)}");
			
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
		if( assignment == null ){
			InvalidArguments();
			return;
		}

		for(int i=0; i < assignment.Length; i++){
			Console.WriteLine(ExtractName(assignment[i])); // TODO display basic info about solution
		}
	}

	public void ShowTaskDescription(string description){
		Console.Write(description);
	}

	public void InvalidArguments(){
		Console.WriteLine("Invalid arguments");	
	}

	public void ShowGroup(string names){
		Console.Write(names);
	}

	public void ShowGroups(string[] groups){
		if( groups == null ) return;
		
		foreach(var g in groups){
			Console.WriteLine(ExtractName(g));
		}
	}

	public void ShowUsers(string[] users){
		if( users == null ) return;

		foreach(var user in users){
			if(user != "Data/Users/Groups") Console.WriteLine(ExtractName(user));
		}
	}
}


// TODO refactor
public interface IConsoleReader{
	 RequestEnum GetCommand(out string[] args);
}

public class ConsoleReader{
	private Dictionary<string, RequestEnum>? commandOptions { get; set; }
	private bool userAdmin { get; set; }
	private string username { get; set; }


	public void SetPropertiesOptions(string name, bool admin, Dictionary<string, RequestEnum> options){
		username = name;
		commandOptions = options;
		userAdmin = admin;
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
                Console.ForegroundColor = ConsoleColor.Green;
		Console.Write($"{username}@Ukolnicek");
		Console.ResetColor();
		Console.Write($"$ ");
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

        private void HelpCommand(){
                if( userAdmin ){
                Console.WriteLine("show-users");
                Console.WriteLine("show-groups");
                Console.WriteLine("show-group [group name]");
		Console.WriteLine("show-assignment [assignment name] --user [user name]");
		Console.WriteLine("show-assignments --user [user name]");
		Console.WriteLine("show-solution [assignment name] [solution name] --user [user name]");
		Console.WriteLine("download-solution [assignment name] [solution name] --user [user name]");
                Console.WriteLine("add-solution [assignment name] [file]");
		Console.WriteLine("add-assignment [assignment name]");
                Console.WriteLine("add-test [assignment name] [test name] --out [file] --in [file] --args [file] --time --points");
                Console.WriteLine("add-task-description [assignment name] [file]");
                Console.WriteLine("add-admin [student name]");
                Console.WriteLine("add-group [group name] [student name] ...");

                Console.WriteLine("assign-task [assignment name] [student name]");
                Console.WriteLine("unassign-task [assignment name] [student name]");

                Console.WriteLine("remove-assignment [assignment name]");
                Console.WriteLine("remove-test [assignment name] [test name]");
                Console.WriteLine("remove-task-description [assignment name]");
                Console.WriteLine("remove-group [group name]");
                } else {
		Console.WriteLine("show-assignment [assignment name]");
		Console.WriteLine("show-assignments");
		Console.WriteLine("show-solution [assignment name] [solution name]");
		Console.WriteLine("add-solution [assignment name] [file]");
		Console.WriteLine("download-solution [assignment name] [solution name]");
		Console.WriteLine("exit");
		}
	}

        private string DeleteChar(string command){
                if( command.Length > 0 ){
                        command = command.Remove(command.Length - 1);
                        Console.Write("\b \b");
                        return command;
                }

                return "";
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
				// TODO condition
				command = DeleteChar(command);
			} else {
				command += key.KeyChar;
				Console.Write(key.KeyChar);
			}
		}
	}
}
