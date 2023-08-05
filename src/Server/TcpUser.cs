using System.Net;
using System.Net.Sockets;
using System.Text;
using Ukolnicek.Communication;
using Ukolnicek.Testing;
using Newtonsoft.Json;

namespace AppServer;

// TODO mutexes for admin operations
// TODO cache created assignments etc
// TODO where communication is not needed ()
public class TcpUser : IDisposable{
        private readonly IObjectTransfer transfer;
	public string Name;
	public bool IsAdmin = false;

        public TcpUser(TcpClient client){
                transfer = new JsonTcpTransfer(client);
        }

	private void Verification(IRequest<object> request){
		var login = GetData<string[]>(request);
		Name = login[0];
		string passwd = login[1];

		bool verified = Directory.Exists($"Data/Users/{Name}") && passwd == File.ReadAllText($"Data/Users/{Name}/passwd").Trim();

		int response = verified ? (File.Exists($"Data/Users/{Name}/admin") ? 2 : 1) : 0;

		transfer.Send( new Response<int> {Data = response} );
		// response is int -> 0 - not verified, 1 - verified students account, 2 - verified admins account
	} 

	public void ClientLoop(){
		while( true ){
			var request = transfer.Receive<IRequest<object>>(); // TODO handle end of transfer
			
			if(request.Type == RequestEnum.Exit) break;

			Console.WriteLine($"{Name} - {request.Type}");
			HandleRequest(request);
		}
	}

	public void HandleRequest(IRequest<object> request){
		switch(request.Type){
			case RequestEnum.Login:
				Verification(request);
				break;
			case RequestEnum.SubmittedSolution:
				SubmittedSolution(request);
				break;
			case RequestEnum.CreateUser:
				CreateUser(request);
				break;
			case RequestEnum.AssignTask:
				AssignTask(request);
				break;
			case RequestEnum.CreateAssignment:
				CreateAssignment(request);
				break;
			case RequestEnum.ShowAssignment:
				ShowAssignment(request);
				break;
			case RequestEnum.ShowAssignments:
				ShowAssignments(request);
				break;
			//case RequestEnum.ShowSolution:
			//	ShowSolution(request);
			//	break;
		}
	}

	// TODO create static class Action -> Action.HandleSubmittedSolution -> return type should be send then
	// but i do not want to pass transfer object somewhere else
	private string SolutionName(string assignmentName){
		int i = 1;
	
		while( true ){
			var temp = i < 10 ? $"0{i}" : $"{i}";
			if( !Directory.Exists($"Data/Users/{Name}/{assignmentName}/Solution{temp}") ){
				return $"Solution{temp}";
			}

			i++;
		}
	}

	private void SubmittedSolution(IRequest<object> request){
		var assignmentName = "Prime";
		var file = GetData<CustomFile>(request);
		file.Save();
		IAssignment a = new Assignment(assignmentName);
		var result = a.RunTests(file.Name);
		transfer.Send( new Response<AssignmentResult> {Data = result} );
		var solutionName = SolutionName(assignmentName);	
		Directory.CreateDirectory($"Data/Users/{Name}/{assignmentName}/{solutionName}");
		File.Move(file.Name, $"Data/Users/{Name}/{assignmentName}/{solutionName}/{file.Name}");
		var json = JsonConvert.SerializeObject(result);
		File.WriteAllText($"Data/Users/{Name}/{assignmentName}/{solutionName}/result", json);
	}

	private void CreateUser(IRequest<object> request){
		// TODO zase data v requestu a vracim true kdyz uspech a false kdyz neuspech
		var userName = GetData<string>(request);
		while(true){
			var correctUserName = !Directory.Exists($"Data/Users/{userName}");
			transfer.Send( new Response<bool> {Data = correctUserName} );
			if( correctUserName ){
				var passwd = transfer.Receive<Request<string>>().Data;
				Directory.CreateDirectory($"Data/Users/{userName}");
				File.WriteAllText($"Data/Users/{userName}/passwd", passwd);
				break;
			}
			userName = transfer.Receive<Request<string>>().Data;
		}
	}

	private void AssignTask(IRequest<object> request){
		//var task = GetData<string>(request);
		// TODO get all data from request.Data, so serialize string array or object array ...
		//var userName = transfer.Receive<Request<string>>().Data;
		var data = GetData<string[]>(request);

		var directory = $"Data/Users/{data[0]}/{data[1]}";
		if( !Directory.Exists(directory) ) Directory.CreateDirectory(directory);
	}

	private void CreateAssignment(IRequest<object> request){
		// TODO update assignment class to be static
		// TODO refactory all these methods
		var data = GetData<object[]>(request);
		if( !Directory.Exists($"Data/Assignments/{(string)data[0]}") ){
			Directory.CreateDirectory($"Data/Assignments/{(string)data[0]}");
			// File.WriteAllBytes($"Data/Assignments/{(string)data[0]}/README.md", (byte[])data[1]);
		}
	}

	private void AddTaskDescription(IRequest<object> request){
		// TODO
		// Assignment.AddTaskDescription();
	}

	private void AddTest(IRequest<object> request){
	}

	private void ShowAssignments(IRequest<object> request){
		string[] assignments;
		if( IsAdmin ){
			assignments = Directory.GetDirectories($"Data/Assignments/");
		} else{
			// TODO admin should have all assignments in your directory
			assignments = Directory.GetDirectories($"Data/Users/{Name}");
		}

		transfer.Send( new Response<string[]> {Data = assignments} );
	}

	private void ShowAssignment(IRequest<object> request){
		// TODO repair for user
		// TODO send taskDescription as bytes?
		var assignmentName = GetData<string>(request);
		var taskDescription = Assignment.GetTaskDescription(assignmentName);
		var temp = new List<string>() { taskDescription };

		foreach(var d in Directory.GetDirectories($"Data/Users/{Name}/{assignmentName}")){
			temp.Add(d);
		}
		
		transfer.Send( new Response<string[]> {Data = temp.ToArray()} );
	}



	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

        public void Dispose(){
                transfer.Dispose();
        }
}

