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
		
		var n = Notification.Create(NotifEnum.AskName, 123);
		Console.WriteLine(n.Data.GetType());
		Notify(Notification.Create(NotifEnum.AskName, 123));
		var nameResponse = GetResponse<string>();

		Name = nameResponse.Data;
		Console.WriteLine(Name);
        }

	// TODO async ?
	public void ClientLoop(){
		var f = GetResponse<CustomFile>();
		f.Data.Save();
		IAssignment a = new Assignment("Prime");
		var r = a.RunTests(f.Data.Name);
		Notify(Notification.Create(NotifEnum.AssignmentResult, r));
		while( true ){
		}
	}

        public IResponse<T> GetResponse<T>(){
        	return transfer.Receive<IResponse<T>>();
	}

        public void Notify<T>(INotification<T> notification){
                transfer.Send(notification);
        }

        public async Task<T?> GetResponseAsync<T>(){
                var responseTask = Task.Run(() => transfer.Receive<IResponse<T>>());
                var completedTask = await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(5))); 

                return responseTask.Result.Data;
        }

        public void Dispose(){
                transfer.Dispose();
        }
}

