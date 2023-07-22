using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;

class TcpUser : IDisposable{
        private readonly IObjectTransfer transfer;

        public TcpUser(TcpClient client){
                transfer = new JsonTcpTransfer(client);
        }

        public T GetResponse<T>(){
                return transfer.Receive<T>();
        }

        public void Notify<T>(T notification){ // TODO wrap object for notification
                transfer.Send(notification);
        }

        public async Task<T?> GetResponseAsync<T>(){
                var responseTask = Task.Run(() => transfer.Receive<T>());
                var completedTask = await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromSeconds(5)));

                return responseTask.Result;
        }

        public void Dispose(){
                transfer.Dispose();
        }
}

