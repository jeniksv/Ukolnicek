using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;
using Testing;

namespace AppServer;

// TODO mutexes for admin operations
public class TcpUser : IDisposable{
        private readonly IObjectTransfer transfer;
	public string Name;

        public TcpUser(TcpClient client){
                transfer = new JsonTcpTransfer(client);
        }

	private void Verification(){
		var name = transfer.Receive<Notification<string>>().Data;
		var passwd = transfer.Receive<Notification<string>>().Data;
		Console.WriteLine(name);
		Console.WriteLine(passwd);
	}

	public void ClientLoop(){
		Verification();
	}

	private T GetData<T>(INotification<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

        public void Dispose(){
                transfer.Dispose();
        }
}

