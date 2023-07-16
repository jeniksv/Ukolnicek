using System.Net;
using System.Net.Sockets;

public class Server : IDisposable{
	private readonly TcpListener server;
	//private List<>

	public Server(int port = 8888){
		Console.WriteLine("Server started.");

		server = new TcpListener(IPAddress.Any, port);

		server.Start();
	}

	// maybe we can use async and awaits
	public void MainLoop(){
		while( true ){
			var client = server.AcceptTcpClient();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");

			// var newUser = new TcpUser()

			clientThread = new Thread(HandleClientiConnection);
			clientThread.Start(client);
		}
	}

	private static void HandleClientConnection(TcpClient client){
		
	}

	public void Dispose(){
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	private void ReleaseUnmanagedResources(){
		// foreach(var client in clients) client.Dispose();
		server.Stop();
		Console.WriteLine("Server stopped.");
	}

	~Server(){
		ReleaseUnmanagedResources();
	}

}
