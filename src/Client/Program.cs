using System.Net;
using System.Net.Sockets;

public class Progam{
        public static void Main(string[] args){
                var client = new Client();
                client.ClientLoop();
                //var serverThread = new Thread(server.MainLoop);
                //serverThread.Start();
        }
}

