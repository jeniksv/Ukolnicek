using System.Net;
using System.Net.Sockets;

public class Progam{
        public static void Main(string[] args){
                var client = new Admin("Ann", "192.168.0.199", 12345);
                client.ClientLoop();
                //var serverThread = new Thread(server.MainLoop);
                //serverThread.Start();
        }
}

