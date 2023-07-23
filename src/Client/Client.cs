using System.Text;
using System.Net;
using System.Net.Sockets;
using Communication;

//namespace Client;

class Client{
	private TcpClient client;
	private IObjectTransfer transfer;

	public Client(){
		client = new TcpClient();
		client.Connect("192.168.88.111", 12345);
		transfer = new JsonTcpTransfer(client);

		// TODO verification in ctor?
	}

	public void ClientLoop(){
		// client.Connect("192.168.10.87", 12345);
		Console.WriteLine("Connected to server.");

		//NetworkStream stream = client.GetStream();

		while(true){
			//var m = await transfer.ReceiveAsync<object>();
			//Console.WriteLine(m);
			Console.WriteLine("Enter filename");
			string file = Console.ReadLine();

			if (file == "exit") break;

			var content = File.ReadAllBytes(file);
			// foreach(var b in content) Console.WriteLine(b);
			var f = new CustomFile(file, content);	
			transfer.Send( Notification.Create<string>(NotifEnum.SubmittedSolution, "ahoj") );
		}
		
		client.Close();
		transfer.Dispose();
	}


}
