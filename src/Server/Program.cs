using System.Net;
using System.Net.Sockets;
using Ukolnicek.Server;

public class Progam{
	public static async Task Main(string[] args){
		using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
		socket.Connect("8.8.8.8", 65530);
		var endPoint = socket.LocalEndPoint as IPEndPoint;
		var ip = endPoint?.Address.ToString();

		Console.WriteLine("Server IP: " + ip);
		// TODO get rid of async and await in this method
		var server = new Server(12345);
		await server.MainLoop();
		//var serverThread = new Thread(server.MainLoop);
		//serverThread.Start();
	}
}
