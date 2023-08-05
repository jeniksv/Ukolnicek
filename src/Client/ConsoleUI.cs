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

	// TODO big one, implement linux tab auto-complete 
	public RequestEnum GetCommand(){
		return RequestEnum.Login;
	}

	public void ShowSolution(){
			
	}
}
