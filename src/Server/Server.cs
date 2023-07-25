using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;
using Testing;

namespace AppServer; 

public class Server : IDisposable{
	private readonly TcpListener server;
	private List<TcpUser> clients; // TODO create abstraction for TcpClient

	public Server(int port = 8888){
		Console.WriteLine("Server started.");

		server = new TcpListener(IPAddress.Any, port);
		clients = new List<TcpUser>();
		
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
			
			// clients.Add(client);

			var newClient = new TcpUser(client);
			
			clients.Add(newClient);
			// TODO authentication here, because i want to pass specific user to
			//HandleClientAsync(client);
		}
	}

	private async void HandleClientAsync(TcpClient client){
		// TODO handle abortion from client
		var transfer = new JsonTcpTransfer(client);
		
		while( true ){
			var m = await Task.Run(() => transfer.Receive<Notification<object>>());
			Console.WriteLine("Received message from client");
			if( m.Type == NotifEnum.SubmittedSolution ){
				Console.WriteLine("Submitted solution.");
				var f = m.Data; // TODO data conversion does not work
				Console.WriteLine($"Message: {f}");
			}
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
