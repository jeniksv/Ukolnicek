using System.Net;
using System.Net.Sockets;
using Communication;

namespace AppServer; 

public class Server : IDisposable{
	private readonly TcpListener server;
	//private List<>

	public Server(int port = 8888){
		Console.WriteLine("Server started.");

		server = new TcpListener(IPAddress.Any, port);

		server.Start();
	}

	// TODO use asyncs and awaits
	public void MainLoop(){
		while( true ){
			var client = server.AcceptTcpClient();
			Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");

			// TODO authentication here, because i want to pass specific user to
			// var newUser = new TcpUser()

			clientThread = new Thread(HandleClientiConnection);
			clientThread.Start(client);
		}
	}

	private void Authenticate(){}

	private static void HandleClientConnection(TcpClient client){
		// TODO user can sends notifications
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
