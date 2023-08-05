using Ukolnicek.Communication;

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

	private string TabMatch(string prefix, string[] options){
		var result = new List<string>();

		foreach(var option in options){
			if( option.StartsWith(prefix) ){
				result.Add(option);
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

	public RequestEnum GetCommand(string username){
		var commands = new string[] {
			"show-assignment",
			"show-assignments",
			"show-solution",
			"create-assignment",
			"create-test",
		};

		string command = "";
		ConsoleKeyInfo key;

		ShowPrompt();
		
		while( true ){
			key = Console.ReadKey(true);
			if( key.Key == ConsoleKey.Enter ){
				Console.WriteLine();
				return RequestEnum.Exit;
			}

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

	public void ShowSolution(){
			
	}
}
