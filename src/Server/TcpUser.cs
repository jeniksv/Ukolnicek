using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;
using Testing;

namespace AppServer;

// TODO mutexes for admin operations
// TODO cache created assignments etc
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
		
		while( true ){
			var request = transfer.Receive<IRequest<object>>();
			Console.WriteLine($"{Name} - {request.Type}");
			HandleRequest(request);

			Console.WriteLine("OK");
			Thread.Sleep(100);
		}
	}

	public void HandleRequest(IRequest<object> request){
		switch(request.Type){
			case RequestEnum.SubmittedSolution:
				SubmittedSolution(request);
				break;
			//case RequestEnum.:
		}
	}

	private string currentAssignment = "";

	// TODO create static class Action -> Action.HandleSubmittedSolution -> return type should be send then
	// but i do not want to pass transfer object somewhere else
	private void SubmittedSolution(IRequest<object> request){
		var file = GetData<CustomFile>(request);
		file.Save();
		IAssignment a = new Assignment("Prime");
		var result = a.RunTests(file.Name);
		// TODO move file to users repository
		// TODO create new Solution[0-9][0-9] directory and store result here
		transfer.Send( new Response<AssignmentResult> {Data = result} );
	}

	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

        public void Dispose(){
                transfer.Dispose();
        }
}

