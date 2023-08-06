using System.Net;
using System.Net.Sockets;
using System.Text;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Server; 

public class Server : IDisposable{
	private readonly TcpListener server;
	private List<TcpUser> clients;

	public Server(int port = 8888){
		Console.WriteLine("Server started.");

		server = new TcpListener(IPAddress.Any, port);
		clients = new List<TcpUser>();
		
		server.Start();
	}

	public async Task MainLoop(){
		while( true ){
			var client = await server.AcceptTcpClientAsync();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
			
			var newClient = new TcpUser(client);
			// newClient.ClientLoop();
			Task.Run(() => newClient.ClientLoop());
			clients.Add(newClient);
			Console.WriteLine("do not wait");
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
