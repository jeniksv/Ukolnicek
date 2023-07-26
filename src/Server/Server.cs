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

	/*
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
	} */

	public async Task MainLoop(){
		while( true ){
			var client = await server.AcceptTcpClientAsync();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
			
			var newClient = new TcpUser(client);
			newClient.ClientLoop();
			clients.Add(newClient);
		}
	}

	public void Dispose(){
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	private void ReleaseUnmanagedResources(){
		foreach(var client in clients) client.Dispose(); 
		server.Stop();
		Console.WriteLine("Server stopped.");
	}

	~Server(){
		ReleaseUnmanagedResources();
	}
}
