using System.Net;
using System.Net.Sockets;
using System.Text;
using Ukolnicek.Communication;
using Ukolnicek.Testing;
using Newtonsoft.Json;

namespace Ukolnicek.Server;

// TODO mutexes for admin operations
// TODO cache created assignments etc
public class TcpUser : IDisposable{
	public string Name { get; set; }
	private readonly IObjectTransfer transfer;
	private bool isAdmin = false;

	public TcpUser(TcpClient client){
		transfer = new JsonTcpTransfer(client);
	}

	private void Verification(IRequest<object> request){
		var login = GetData<string[]>(request);
		Name = login[0];
		string passwd = login[1];

		bool verified = Directory.Exists($"Data/Users/{Name}") && passwd == File.ReadAllText($"Data/Users/{Name}/passwd").Trim();

		// response is int where 0 - not verified, 1 - verified students account, 2 - verified admins account
		int response = verified ? (File.Exists($"Data/Users/{Name}/admin") ? 2 : 1) : 0;

		isAdmin = response == 2;

		transfer.Send( new Response<int> {Data = response} );
	} 

	public void ClientLoop(){
		while( true ){
			var request = transfer.Receive<IRequest<object>>(); // TODO handle end of transfer
			
			if(request.Type == RequestEnum.Exit) break;

			if(request.Type != RequestEnum.Login) Console.WriteLine($"{Name} - {request.Type}");
			
			HandleRequest(request); // TODO handle exceptions
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
			case RequestEnum.ShowSolution:
				ShowSolution(request);
				break;
			case RequestEnum.AddTest:
				AddTest(request);
				break;
		}
	}

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
		var data = GetData<object[]>(request);
		
		var assignmentName = (string)data[0];
		var fileName = (string)data[1];
		File.WriteAllBytes(fileName, (byte[])data[2]);
		
		// TODO restructure assignments
		IAssignment a = new Assignment(assignmentName);
		var result = a.RunTests(fileName);
		
		var solutionName = SolutionName(assignmentName);
		
		Directory.CreateDirectory($"Data/Users/{Name}/{assignmentName}/{solutionName}");
		File.Move(fileName, $"Data/Users/{Name}/{assignmentName}/{solutionName}/{fileName}");

		var json = JsonConvert.SerializeObject(result);
		File.WriteAllText($"Data/Users/{Name}/{assignmentName}/{solutionName}/result.json", json);
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
		// TODO check if second argument is not name of group
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
		var data = GetData<object[]>(request);
	
		var assignmentName = (string)data[0];
		var testName = (string)data[1];
		var outputBytes = (byte[])data[2];
		var inputBytes = (byte[])data[3];
		var argsBytes = (byte[])data[4];
		var time = (int)(long)data[5]; // nechapu proc to musim napsat takhle
		var points = (int)(long)data[6];
		
		Assignment.AddTest(assignmentName, testName, outputBytes, inputBytes, argsBytes, time, points);
	}

	private void ShowAssignments(IRequest<object> request){
		string[] assignments;
		if( isAdmin ){
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

	private void ShowSolution(IRequest<object> request){
		var solutionName = GetData<string>(request);
		var json = File.ReadAllText($"Data/Users/{Name}/{solutionName}/result.json");
		var assignmentResult = JsonConvert.DeserializeObject<AssignmentResult>(json);
		transfer.Send( new Response<AssignmentResult> { Data = assignmentResult } );	
	}


	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

	public void Dispose(){
		transfer.Dispose();
	}
}

