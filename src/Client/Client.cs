using System.Text;
using System.Net;
using System.Net.Sockets;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client; 

/// <summary>
/// pattern for classes Student and Admin
/// </summary>
public abstract class User{
	public string Name;
	public int Id;
	protected IObjectTransfer transfer;
	private IUserInterface ui;

	public User(string name, IObjectTransfer t){
		Name = name;
		transfer = t;
		ui = new ConsoleUI();
	}

	public void ClientLoop(){
		// TODO async method for creating tasks -> submitted solution, i can 
		bool running = true;
		while( running ){
			switch( ui.GetCommand(out string[] args) ){
				case RequestEnum.ShowAssignments:
					ShowAssignments();
					break;
				case RequestEnum.ShowAssignment:
					ShowAssignment("Prime");
					break;
				case RequestEnum.SubmittedSolution:
					SubmitSolution(args);
					break;
				case RequestEnum.ShowSolution:
					ShowSolution(args);
					break;
				case RequestEnum.AddTest:
					AddTest(args);
					break;
				case RequestEnum.Exit:
					Notify( Request.Create(RequestEnum.Exit) );
					running = false;
					break;
			}
		}
	}

	public void SubmitSolution(string[] args){
		// TODO handle invalid arguments
		var data = new object[] {args[0], args[1], File.ReadAllBytes(args[1])};
		Notify( Request.Create(RequestEnum.SubmittedSolution, data) );
	}

	public void CreateUser(){
		while(true){
			Console.Write("Enter username: ");
			Notify( Request.Create(RequestEnum.CreateUser, Console.ReadLine()) );
			var response = GetResponse<bool>();
			if( response.Data ){
				// something like passwords are same and thne notify
				Console.Write("Enter password: ");
				Notify( Request.Create(RequestEnum.CreateUser, Console.ReadLine()) );
				break;
			}
			Console.WriteLine("User already exists");
		}
	}

	public void AssignTask(string[] args){
		var userName = "Jenda";
		var task = "Prime";
		var data = new string[] {userName, task};

		Notify( Request.Create(RequestEnum.AssignTask, data) );
	}

	public void CreateAssignment(){
		var assignmentName = "Palindrome";
		var file = File.ReadAllBytes("task.md");
		var data = new object[] {assignmentName, file};
		Notify( Request.Create(RequestEnum.CreateAssignment, data) );
	}

	public void AddTest(string[] args){
		var p = new Parser(args);
		
		if( !p.CorrectArguments ){
			ui.InvalidArguments();
			return;
		}

		var outputBytes = p.OutputFileName != null ? File.ReadAllBytes(p.OutputFileName) : null;
		var inputBytes = p.InputFileName != null ? File.ReadAllBytes(p.InputFileName) : null;
		var argsBytes = p.ArgsFileName != null ? File.ReadAllBytes(p.ArgsFileName) : null;
		var time = p.Time != null ? p.Time : 5000; // TODO default values should be in assignment
		var points = p.Points != null ? p.Points : 1;

		var data = new object[]{p.AssignmentName, p.TestName, outputBytes, inputBytes, argsBytes, time, points};

		Notify( Request.Create(RequestEnum.AddTest, data) );	
	}

	public void ShowAssignments(){ // TODO virtual. for admin it should show all assignments
		Notify( Request.Create(RequestEnum.ShowAssignments) );
		var response = GetResponse<string[]>();
		ui.ShowAssignments(response.Data);
	}

	public void ShowAssignment(string assignmentName){
		Notify( Request.Create(RequestEnum.ShowAssignment, assignmentName) );
		var response = GetResponse<string[]>(); // task description, list of solutions
		DisplayAssignment(response.Data);
	}

	public void ShowSolution(string[] args){
		Notify( Request.Create(RequestEnum.ShowSolution, $"{args[0]}/{args[1]}") );
		var response = GetResponse<AssignmentResult>();
		ui.ShowSolution(response.Data);
	}

	public void DisplayAssignment(string[] a){
		foreach(var v in a ) Console.WriteLine(v);
	}

	public IResponse<T> GetResponse<T>(){
		return transfer.Receive<IResponse<T>>();
	}

	public void Notify<T>(IRequest<T> notification){
		transfer.Send(notification);
	}
}

/// <summary>
///	 Factory for creating users.
/// </summary>
public static class Client{
	private static string ip = "192.168.5.147"; //192.168.0.199
	private static int port = 12345;

	public static User? SignIn(IUserInterface ui){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		
		for(int i=0; i<3; i++){
			ui.AskUsername();
			var name = ui.GetUsername();
			ui.AskPassword();
			var passwd = ui.GetPassword();

			transfer.Send(Request.Create(RequestEnum.Login, new string[] {name, passwd}));
		
			var verified = transfer.Receive<IResponse<int>>().Data;

			if( verified > 0 ) return verified == 1 ? new Student(name, transfer) : new Admin(name, transfer);
		
			ui.InvalidLogin();
		}
		
		transfer.Dispose();
		return null;
	}
}
