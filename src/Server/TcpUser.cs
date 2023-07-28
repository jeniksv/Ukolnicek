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
		while(true){
			Name = transfer.Receive<Request<string>>().Data;
			var passwd = transfer.Receive<Request<string>>().Data;
			// TODO read name and passwd from users directory
			var verified = Name == "Ann" && passwd == "123";
			transfer.Send( new Response<bool> {Data = verified} );

			if( verified ){
				// TODO get isAdmin from users directory
				var isAdmin = true;
				transfer.Send( new Response<bool> {Data = isAdmin} );
				break;
			}
		}
	}

	public void ClientLoop(){
		Verification();
	}

	public void HandleRequest(IRequest<object> update){
		switch(update.Type){
			case RequestEnum.SubmittedSolution:
				break;
			//case RequestEnum.:
		}
	}

	private void SubmittedSolution(IRequest<object> update){
	
	}

	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

        public void Dispose(){
                transfer.Dispose();
        }
}

