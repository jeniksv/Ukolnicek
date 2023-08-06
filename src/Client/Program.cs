using Ukolnicek.Client;

public class Progam{
        public static void Main(string[] args){
		IUserInterface consoleUI = new ConsoleUI();
		
		if( args.Contains("--create") ){
			consoleUI.SetUser( Client.CreateAccount(consoleUI) );
		} else {
			consoleUI.SetUser( Client.SignIn(consoleUI) );
		}
		
		consoleUI.MainLoop();
	}
}
