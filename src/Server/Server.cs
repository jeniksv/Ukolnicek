using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;
using Testing;

namespace AppServer; 

public class Server : IDisposable{
	private readonly TcpListener server;
	private List<TcpClient> clients; // TODO create abstraction for TcpClient

	public Server(int port = 8888){
		Console.WriteLine("Server started.");

		server = new TcpListener(IPAddress.Any, port);
		clients = new List<TcpClient>();
		
		server.Start();
	}

	private void FirstSetUp(){
		// TODO ask for path for data => timhle si zas vykopes hrob
		if( !Directory.Exists("Data") ){
			Directory.CreateDirectory("Data");
			Directory.CreateDirectory("Data/Assignments");
			Directory.CreateDirectory("Data/Users");
			Console.WriteLine("Enter user name");
			var name = Console.ReadLine();
			Directory.CreateDirectory($"Data/Users/{name}");
			Console.WriteLine("Enter your password"); // TODO dont display this line
			var passwd = Console.ReadLine();
			File.WriteAllText($"Data/Users/{name}/passwd", passwd);
		}
	}

	public async Task MainLoop(){
		while( true ){
			var client = await server.AcceptTcpClientAsync();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
			
			clients.Add(client);

			// var newClient = new TcpUser(client);
			// TODO authentication here, because i want to pass specific user to
			HandleClientAsync(client);
		}
	}

	private async void HandleClientAsync(TcpClient client){
		// TODO handle abortion from client
		var transfer = new JsonTcpTransfer(client);
		
		while( true ){
			// TODO create wrap interface instead of an object
			var m = await transfer.ReceiveAsync<object>();
			Console.WriteLine("Received message from client");
			Console.WriteLine( m.GetType() );
			// TODO reflection is not the best idea xddd
			// good way is to create INotification{ }, ktera bude dva fieldy, NotifEnum a Data
			if( m is CustomFile ){
				Console.WriteLine("File received.");
			}
			// if( m.Equals("ahoj") ) transfer.Send(m);
		}

		// transfer.Dispose();
	}

	public void Dispose(){
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	private void ReleaseUnmanagedResources(){
		// foreach(var client in clients) client.Close(); // in case of TcpUser Dispose()
		server.Stop();
		Console.WriteLine("Server stopped.");
	}

	~Server(){
		ReleaseUnmanagedResources();
	}
}
