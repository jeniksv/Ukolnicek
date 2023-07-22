using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;

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

	// TODO use asyncs and awaits
	public async Task MainLoop(){
		while( true ){
			var client = await server.AcceptTcpClientAsync();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
			
			clients.Add(client);

			// var newClient = new TcpUser(client);
			// TODO authentication here, because i want to pass specific user to
			// TODO handle async
			HandleClientAsync(client);
		}
	}

	private async void HandleClientAsync(TcpClient client){
		// TODO handle abortion from client
		var transfer = new JsonTcpTransfer(client);
		//NetworkStream stream = client.GetStream();
		byte[] buffer = new byte[1024];
		
		while( true ){
			var m = await transfer.ReceiveAsync<object>();
			Console.WriteLine(m);
		}

		transfer.Dispose();
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
