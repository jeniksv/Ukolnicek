using Ukolnicek.Communication;

namespace Ukolnicek.Client;

public class ConsoleUI : IUserInterface {
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
				//Console.Write("*");
			} else if( password.Length > 0 ){
				password = password.Remove(password.Length - 1);
				//Console.Write("\b \b");
			}
		}
	}

	public void AskPassword(){
		Console.Write("password: ");
	}

	public void InvalidLogin(){
		Console.WriteLine("Permission denied, please try again.");
	}

	private List<string> MatchOptions(string prefix, string[] options){
		var result = new List<string>();

		foreach(var option in options){
			if( option.StartsWith(prefix) ){
				result.Add(option);
			}
		}

		return result;
	}

	public RequestEnum GetCommand(string username){
		var commands = new string[] {
			"show-assignment",
			"show-assignments",
			"show-solution",
			"create-assignment",
			"create-test",
			"baa",
			"cccccccccccc"
		};

		string command = "";
		ConsoleKeyInfo key;

		ShowPrompt(username);
		
		while( true ){
			key = Console.ReadKey(true);
			if( key.Key == ConsoleKey.Enter ){
				Console.WriteLine();
				return RequestEnum.Exit;
			}

			if( key.Key == ConsoleKey.Tab ){
				var result = MatchOptions(command, commands);
				if( result.Count == 1 ){
					for(int i=0; i<command.Length; i++) Console.Write("\b \b");
					command = result[0];
					Console.Write(command);
				}
				if( result.Count > 1 ){
					Console.WriteLine();
					foreach(string s in result){
						Console.WriteLine(s);
					}
					ShowPrompt(username);
					// TODO command = longest common prefix of strings in result
					Console.Write(command);
				}
			} else if( key.Key == ConsoleKey.Backspace ){
				if( command.Length > 0 ){
					command = command.Remove(command.Length - 1);
					Console.Write("\b \b");
				}
			} else {
				command += key.KeyChar;
				Console.Write(key.KeyChar);
			}
		}
	}

	private void ShowPrompt(string name){
		// TODO change color?
		Console.Write($"{name} > ");
	}

	public void ShowSolution(){
			
	}
}
