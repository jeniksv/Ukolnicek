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
		bool running = true;
		while( running ){
			switch( ui.GetCommand(out string[] args) ){
				case RequestEnum.ShowAssignments:
					ShowAssignments();
					break;
				case RequestEnum.ShowAssignment: // TODO admin should see assignment directory
					ShowAssignment(args);
					break;
				case RequestEnum.ShowSolution:
					ShowSolution(args);
					break;
				case RequestEnum.ShowTaskDescription:
					ShowTaskDescription(args);
					break;
				case RequestEnum.SubmittedSolution: // TODO change to AddSolution
					SubmitSolution(args);
					break;
				case RequestEnum.AddTest:
					AddTest(args);
					break;
				case RequestEnum.AddAssignment:
					AddAssignment(args);
					break;
				case RequestEnum.AddTaskDescription:
					AddTaskDescription(args);
					break;
				case RequestEnum.AssignTask:
					AssignTask(args);
					break;
				case RequestEnum.UnassignTask:
					UnassignTask(args);
					break;
				case RequestEnum.RemoveTest:
					RemoveTest(args);
					break;
				
				case RequestEnum.RemoveAssignment:
					RemoveAssignment(args);
					break;
				case RequestEnum.RemoveTaskDescription:
					RemoveTaskDescription(args);
					break;
				case RequestEnum.Exit:
					Notify( Request.Create(RequestEnum.Exit) );
					running = false;
					break;
			}
		}
	}

	public void SubmitSolution(string[] args){
		if( args.Length < 2 || !File.Exists(args[1]) ){ // TODO pass this to parser
			ui.InvalidArguments();
			return;
		}

		var data = new object[] {args[0], args[1], File.ReadAllBytes(args[1])};
		Notify( Request.Create(RequestEnum.SubmittedSolution, data) );
	}

	public void CreateUser(){
	}

	public void AssignTask(string[] args){
		if( args.Length < 2 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.AssignTask, args) );
	}

	private void UnassignTask(string[] args){ // TODO recursive directory delete dont have to work
		if( args.Length < 2 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.UnassignTask, args) );
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

		var data = new object[] {p.AssignmentName!, p.TestName!, outputBytes!, inputBytes!, argsBytes!, time, points};

		Notify(Request.Create(RequestEnum.AddTest, data) );	
	}

	public void AddAssignment(string[] args){
		if( args.Length == 0 ){
			ui.InvalidArguments();
			return;
		}

		Notify(Request.Create(RequestEnum.AddAssignment, args[0]));
	}

	public void AddTaskDescription(string[] args){
		if( args.Length < 2 ){
			ui.InvalidArguments();
			return;
		}

		var data = new string[] {args[0], File.ReadAllText(args[1])};

		Notify(Request.Create(RequestEnum.AddTaskDescription, data));
	}

	public void ShowAssignments(){
		Notify( Request.Create(RequestEnum.ShowAssignments) );
		var response = GetResponse<string[]>();
		ui.ShowAssignments(response.Data);
	}

	public void ShowAssignment(string[] args){ // TODO possible more arguments, display assignment info about given assignemnts
		if( args.Length == 0 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.ShowAssignment, args[0]));
		var response = GetResponse<string[]>(); // task description, list of solutions
		ui.ShowAssignment(response.Data);
	}

	public void ShowSolution(string[] args){
		Notify( Request.Create(RequestEnum.ShowSolution, $"{args[0]}/{args[1]}"));
		var response = GetResponse<AssignmentResult>();
		ui.ShowSolution(response.Data);
	}

	public void ShowTaskDescription(string[] args){
		if( args.Length == 0 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.ShowTaskDescription, args[0]));
		var response = GetResponse<string>();
		ui.ShowTaskDescription(response.Data);
	}

	private void RemoveTest(string[] args){
		if( args.Length < 2 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.RemoveTest, args));
	}

	private void RemoveAssignment(string[] args){
		if( args.Length == 0 ){
			ui.InvalidArguments();
			return;
		}

		Notify( Request.Create(RequestEnum.RemoveAssignment, args[0]));
	}

	private void RemoveTaskDescription(string[] args){
		if( args.Length == 0 ){
			ui.InvalidArguments();
			return;
		}
		
		Notify( Request.Create(RequestEnum.RemoveTaskDescription, args[0]));
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
	private static string ip = "10.0.4.108"; // "192.168.5.147"; //192.168.0.199
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
