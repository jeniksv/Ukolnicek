using System.Net;
using System.Net.Sockets;
using Communication;
using System.Text;

class Client{
	private TcpClient client;
	private IObjectTransfer transfer;

	public Client(){
		client = new TcpClient();
		client.Connect("192.168.10.87", 12345);
		transfer = new JsonTcpTransfer(client);
	}

	public void ClientLoop(){
		// client.Connect("192.168.10.87", 12345);
		Console.WriteLine("Connected to server.");

		//NetworkStream stream = client.GetStream();

		while(true){
			Console.WriteLine("Enter filename");
			string message = Console.ReadLine();

			if (message == "exit") break;
			
			transfer.Send(message);

			//byte[] data = Encoding.UTF8.GetBytes(file);
			//stream.WriteAsync(data, 0, data.Length);

			/*
			 * byte[] buffer = new byte[1024];
			 * int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
			 * string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			 */
		}

		client.Close();
	}
}
