using Ukolnicek.Client;

public class Progam{
        public static void Main(string[] args){
		IUserInterface consoleUI = new ConsoleUI();
	
		User? user = args.Contains("--create") ? Client.CreateAccount(consoleUI) : Client.SignIn(consoleUI);
		
		if( user != null){	
			consoleUI.SetUser(user);
			consoleUI.MainLoop();
		}
	}
}
