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
	public string? Name { get; set; }
	private readonly IObjectTransfer transfer;
	private bool isAdmin = false;

	public TcpUser(TcpClient client){
		transfer = new JsonTcpTransfer(client);
	}

	private void Login(IRequest<object> request){
		var login = GetData<string[]>(request);
		Name = login[0];
		var passwd = login[1];

		var verified = Directory.Exists($"Data/Users/{Name}") && passwd == File.ReadAllText($"Data/Users/{Name}/passwd").Trim();

		// response is int where 0 - not verified, 1 - verified students account, 2 - verified admins account
		int response = verified ? (File.Exists($"Data/Users/{Name}/admin") ? 2 : 1) : 0;

		isAdmin = response == 2;

		transfer.Send( new Response<int> {Data = response} );
	}

	private void CreateAccount(IRequest<object> request){
		var login = GetData<string[]>(request);
		var username = login[0];
		var passwd = login[1];

		var correctUsername = !Directory.Exists($"Data/Users/{username}");
		
		transfer.Send( new Response<bool> {Data = correctUsername} );

		if( correctUsername ){
			Directory.CreateDirectory($"Data/Users/{username}");
			File.WriteAllText($"Data/Users/{username}/passwd", passwd);
		}
	}	

	public void ClientLoop(){
		while( true ){
			var request = transfer.Receive<IRequest<object>>(); // TODO handle end of transfer
			
			if(request.Type == RequestEnum.Exit) break;

			if(request.Type != RequestEnum.Login) Console.WriteLine($"{Name} - {request.Type}");
			
			try{	
				HandleRequest(request); // TODO handle exceptions
			} catch(Exception e){
				Console.WriteLine($"{Name} - {e.Message}");
			}
		}
	}

	public void HandleRequest(IRequest<object> request){
		switch(request.Type){
			case RequestEnum.Login:
				Login(request);
				break;
			case RequestEnum.CreateAccount:
				CreateAccount(request);
				break;
			case RequestEnum.AddSolution:
				AddSolution(request);
				break;
			case RequestEnum.AssignTask:
				AssignTask(request);
				break;
			case RequestEnum.UnassignTask:
				UnassignTask(request);
				break;
			case RequestEnum.AddAssignment:
				AddAssignment(request);
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
			case RequestEnum.ShowGroup:
				ShowGroup(request);
				break;
			case RequestEnum.ShowGroups:
				ShowGroups(request);
				break;
			case RequestEnum.AddTest:
				AddTest(request);
				break;
			case RequestEnum.AddTaskDescription:
				AddTaskDescription(request);
				break;
			case RequestEnum.ShowTaskDescription:
				ShowTaskDescription(request);
				break;
			case RequestEnum.RemoveTest:
				RemoveTest(request);
				break;
			case RequestEnum.RemoveAssignment:
				RemoveAssignment(request);
				break;
			case RequestEnum.RemoveTaskDescription:
				RemoveTaskDescription(request);
				break;
			case RequestEnum.AddAdmin:
				AddAdmin(request);
				break;
			case RequestEnum.AddGroup:
				AddGroup(request);
				break;
			case RequestEnum.RemoveGroup:
				RemoveGroup(request);
				break;
			case RequestEnum.ShowUsers:
				ShowUsers(request);
				break;
			case RequestEnum.DownloadSolution:
				DownloadSolution(request);
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

	// TODO create static class User for user actions
	private void AddSolution(IRequest<object> request){
		var data = GetData<object[]>(request);
		
		var assignmentName = (string)data[0];
		var fileName = (string)data[1];
		File.WriteAllBytes(fileName, (byte[])data[2]);
	
		var result = Assignment.RunTests(assignmentName, fileName);
		var solutionName = SolutionName(assignmentName);
		
		Directory.CreateDirectory($"Data/Users/{Name}/{assignmentName}/{solutionName}");
		File.Move(fileName, $"Data/Users/{Name}/{assignmentName}/{solutionName}/{fileName}");

		var json = JsonConvert.SerializeObject(result);
		File.WriteAllText($"Data/Users/{Name}/{assignmentName}/{solutionName}/result.json", json);
	}

	private void AssignTask(IRequest<object> request){
		// TODO check if second argument is not name of group
		var data = GetData<string[]>(request); // assignmentName, studentName

		var directory = $"Data/Users/{data[1]}/{data[0]}";
		if( !Directory.Exists(directory) ) Directory.CreateDirectory(directory);
	}

	private void UnassignTask(IRequest<object> request){
		// TODO check for group
		var data = GetData<string[]>(request); // assignmentName, studentName
		
		var directory = $"Data/Users/{data[1]}/{data[0]}";
		
		if( Directory.Exists(directory) ) Directory.Delete(directory, true);
	}

	private void AddAssignment(IRequest<object> request){
		var data = GetData<string>(request);
		Assignment.Create(data);
	}

	private void AddTaskDescription(IRequest<object> request){
		var data = GetData<string[]>(request);
		Assignment.AddTaskDescription(data[0], data[1]);
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
			// TODO picovina, spatne hodnoty
			assignments = Directory.GetDirectories($"Data/Users/{Name}");
		}

		transfer.Send( new Response<string[]> {Data = assignments} );
	}

	private void ShowAssignment(IRequest<object> request){
		var assignmentName = GetData<string>(request);
		
		var response = new List<string>(){ Assignment.GetTaskDescription(assignmentName) };

		foreach(var d in Directory.GetDirectories($"Data/Users/{Name}/{assignmentName}")){
			response.Add(d);
		}
		
		transfer.Send( new Response<string[]> {Data = response.ToArray()} );
	}

	private void ShowSolution(IRequest<object> request){
		var solutionName = GetData<string>(request);
		var json = File.ReadAllText($"Data/Users/{Name}/{solutionName}/result.json");
		var assignmentResult = JsonConvert.DeserializeObject<AssignmentResult>(json);
		transfer.Send( new Response<AssignmentResult> { Data = assignmentResult } );	
	}

	private void ShowTaskDescription(IRequest<object> request){
		var assignmentName = GetData<string>(request);
		
		var response = Assignment.GetTaskDescription(assignmentName);

		transfer.Send( new Response<string> {Data = response} );
	}

	private void ShowGroup(IRequest<object> request){
		var groupName = GetData<string>(request);

		var response = File.ReadAllText($"Data/Users/Groups/{groupName}");
		
		transfer.Send( new Response<string> {Data = response} );
	}

	private void ShowGroups(IRequest<object> request){
		var response = Directory.GetFiles($"Data/Users/Groups");

		transfer.Send( new Response<string[]> {Data = response} );	
	}

	private void ShowUsers(IRequest<object> request){
		// TODO remove Groups, ted je to zadratovany v ConsoleUI
		var response = Directory.GetDirectories($"Data/Users");

		transfer.Send( new Response<string[]> {Data = response} );
	}

	private void RemoveTest(IRequest<object> request){
		var data = GetData<string[]>(request);
		var assignmentName = data[0];
		var testName = data[1];

		Assignment.RemoveTest(assignmentName, testName);	
	}
	
	private void RemoveAssignment(IRequest<object> request){
		var assignmentName = GetData<string>(request);

		Assignment.Remove(assignmentName);
	}

	private void RemoveTaskDescription(IRequest<object> request){
		var assignmentName = GetData<string>(request);

		Assignment.RemoveTaskDescription(assignmentName);
	}

	private void AddAdmin(IRequest<object> request){
		var name = GetData<string>(request);

		if( !File.Exists($"Data/Users/{name}/admin") ){
			File.Create($"Data/Users/{name}/admin");
		}
	}

	private void AddGroup(IRequest<object> request){
		var data = GetData<string[]>(request);
		var groupName = data[0];

		if( !File.Exists($"Data/Users/Groups/{groupName}") ){
			using(StreamWriter writer = new StreamWriter($"Data/Users/Groups/{groupName}")){
				for(int i=1; i<data.Length; i++){
					writer.WriteLine(data[i]);
				}
			}
		}
	}

	private void RemoveGroup(IRequest<object> request){
		var groupName = GetData<string>(request);

		if( File.Exists($"Data/Users/Groups/{groupName}") ) {
			File.Delete($"Data/Users/Groups/{groupName}");
		}
	}

	private string FindProgramName(string assignmentName, string solutionName){
		var directory = $"Data/Users/{Name}/{assignmentName}/{solutionName}";

		foreach(var file in Directory.GetFiles(directory)){
			// TODO not in set
			if(Path.GetFileName(file) != "result.json"){
				return file;
			}		
		}

		throw new InvalidOperationException();
	}

	private void DownloadSolution(IRequest<object> request){
		var data = GetData<string[]>(request);
		var assignmentName = data[0];
		var solutionName = data[1];

		var solutionFile = FindProgramName(assignmentName, solutionName); 
		var contents = File.ReadAllBytes($"{solutionFile}");

		transfer.Send( new Response<string> {Data = Path.GetFileName(solutionFile)} );
		transfer.Send( new Response<byte[]> {Data = contents} );
	}

	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}

	public void Dispose(){
		transfer.Dispose();
	}
}
