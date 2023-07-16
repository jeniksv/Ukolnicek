using System.Net;
using System.Net.Sockets;

using Communication;

namespace AppServer;

public class TcpUser : IDisposable{ // TODO create base class user
	private IObjectTransfer transfer;

	public TcpUser(TcpClient client){
		transfer = new JsonTcpTransfer(client);

	}
	
	public void Dispose(){
		transfer.Dispose();
	}

}
