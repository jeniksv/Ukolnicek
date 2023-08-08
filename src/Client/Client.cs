using System.Text;
using System.Net;
using System.Net.Sockets;
using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client; 

/// <summary>
/// pattern for classes Student and Admin
/// </summary>
public abstract class User : IDisposable{
	public string Name;
	protected IObjectTransfer transfer;
	
	public void Dispose(){
		transfer.Dispose();
	}

	public User(string name, IObjectTransfer t){
		Name = name;
		transfer = t;
	}

	// TODO generic or whatever, not object
	public virtual object HandleCommand(RequestEnum command, string[] args){
		object data = null;
		switch( command ){
			case RequestEnum.ShowAssignments:
				data = ShowAssignments();
				break;
			case RequestEnum.ShowAssignment: // TODO admin should see assignment directory
				data = ShowAssignment(args);
				break;
			case RequestEnum.ShowSolution:
				data = ShowSolution(args);
				break;
			case RequestEnum.ShowTaskDescription:
				data = ShowTaskDescription(args);
				break;
			case RequestEnum.AddSolution:
				AddSolution(args);
				break;
			case RequestEnum.Exit:
				Notify( Request.Create(RequestEnum.Exit) ); // TODO
				break;
			}

			return data;
	}

	protected void AddSolution(string[] args){
		if( args.Length < 2 || !File.Exists(args[1]) ){ // TODO pass this to parser
			return;
		}

		var data = new object[] {args[0], args[1], File.ReadAllBytes(args[1])};
		Notify( Request.Create(RequestEnum.AddSolution, data) );
	}

	protected string[] ShowAssignments(){ // TODO
		Notify( Request.Create(RequestEnum.ShowAssignments) );
		var response = GetResponse<string[]>();
		return response.Data;
	}

	protected string[] ShowAssignment(string[] args){
		Notify( Request.Create(RequestEnum.ShowAssignment, args[0]));
		var response = GetResponse<string[]>(); // task description, list of solutions
		return response.Data;
	}

	protected AssignmentResult ShowSolution(string[] args){
		Notify( Request.Create(RequestEnum.ShowSolution, $"{args[0]}/{args[1]}"));
		var response = GetResponse<AssignmentResult>();
		return response.Data;
	}

	protected string ShowTaskDescription(string[] args){ // TODO
		Notify( Request.Create(RequestEnum.ShowTaskDescription, args[0]));
		var response = GetResponse<string>();
		return response.Data;
	}

	protected IResponse<T> GetResponse<T>(){
		return transfer.Receive<IResponse<T>>();
	}

	protected void Notify<T>(IRequest<T> notification){
		transfer.Send(notification);
	}
}

/// <summary>
///	 Factory for creating users.
/// </summary>
public static class Client{
	private static string ip = "10.24.180.46";
	private static int port = 12345;

	public static User? SignIn(IUserInterface ui){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		
		for(int i=0; i<3; i++){
			ui.AskUsername();
			var name = ui.GetUsername();
			ui.AskPassword();
			var passwd = ui.GetPassword();

			if( name == "" || passwd == "" ){
				ui.InvalidLogin();
				continue;
			}

			transfer.Send(Request.Create(RequestEnum.Login, new string[] {name, passwd}));
		
			var verified = transfer.Receive<IResponse<int>>().Data;

			if( verified > 0 ) return verified == 1 ? new Student(name, transfer) : new Admin(name, transfer);
		
			ui.InvalidLogin();
		}
		
		transfer.Dispose();
		return null;
	}

	public static User? CreateAccount(IUserInterface ui){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		
		while( true ){
			ui.AskUsername();
			var name = ui.GetUsername();
			ui.AskPassword();
			var passwd = ui.GetPassword();

			if( name == "" || passwd == "" ){
				ui.InvalidLogin();
				continue;
			}

			transfer.Send(Request.Create(RequestEnum.CreateAccount, new string[] {name, passwd}));

			var verified = transfer.Receive<IResponse<bool>>().Data;

			if( verified ) return new Student(name, transfer);

			ui.AccountExists();	
		}
	}

	public static User CreateAdmin(){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		return new Admin("Jenda", transfer);
	}

	public static User CreateStudent(){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		return new Student("Ann", transfer);
	}
}
