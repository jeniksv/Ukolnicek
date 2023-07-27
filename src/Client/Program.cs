using System.Net;
using System.Net.Sockets;
using AppClient;

public class Progam{
        public static void Main(string[] args){
                var client = Client.Create();
		client.ClientLoop();
                //var serverThread = new Thread(server.MainLoop);
                //serverThread.Start();
        }
}

