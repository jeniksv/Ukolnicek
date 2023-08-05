using Ukolnicek.Client;

public class Progam{
        public static void Main(string[] args){
		var client = Client.SignIn( new ConsoleUI() );
		client?.ClientLoop();
        }
}

