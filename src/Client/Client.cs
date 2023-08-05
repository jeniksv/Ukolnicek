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
		// SubmitSolution();
		// CreateUser();
		// AssignTask();
		// CreateAssignment();
		// TODO async method for creating tasks -> submitted solution, i can 
		bool b = true;
		while( b ){
			//var command = Console.ReadLine()[0];
			var command = ui.GetCommand(Name);

			switch( command ){
				case RequestEnum.ShowAssignments:
					ShowAssignments();
					break;
				case RequestEnum.ShowAssignment:
					ShowAssignment("Prime");
					break;
				case RequestEnum.SubmittedSolution:
					SubmitSolution();
					break;
				case RequestEnum.Exit:
					Notify( Request.Create(RequestEnum.Exit) );
					b = false;
					break;
			}
		}

		//ShowAssignments();
		//ShowAssignment("Prime");
	}

	// TODO extract name from path
	public void SubmitSolution(){
		var c = new CustomFile("prime.py", File.ReadAllBytes("prime.py"));
		Notify( Request.Create(RequestEnum.SubmittedSolution, c) );
		var response = GetResponse<AssignmentResult>();
		DisplayAssignmentResult( response.Data );
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

	public void AssignTask(){
		var userName = "Jenda";
		var task = "Prime";
		var data = new string[] { userName, task };

		Notify( Request.Create( RequestEnum.AssignTask, data) );
	}

	public void CreateAssignment(){
		var assignmentName = "Palindrome";
		var file = File.ReadAllBytes("task.md");
		var data = new object[] { assignmentName, file };
		Notify( Request.Create( RequestEnum.CreateAssignment, data) );
	}

	public void AddTest(){
		var assignmentName = "Palindrome";
		var input = File.Exists("in") ? File.ReadAllBytes("in") : null;
		var output = File.ReadAllBytes("out");
		var args = File.Exists("args") ? File.ReadAllBytes("args") : null;
	}

	public void ShowAssignments(){ // TODO virtual. for admin it should show all assignments
		Notify( Request.Create(RequestEnum.ShowAssignments) );
		var response = GetResponse<string[]>();
		DisplayAssignments(response.Data);
	}

	public void ShowAssignment(string assignmentName){
		// TODO from show assignments pick one and display it
		Notify( Request.Create(RequestEnum.ShowAssignment, assignmentName) );
		var response = GetResponse<string[]>(); // task description, list of solutions
		DisplayAssignment(response.Data);
	}

	public void ShowSolution(string assignmentName, string solutionName){
		Notify( Request.Create(RequestEnum.ShowSolution, $"{assignmentName}/{solutionName}") );
		var response = GetResponse<string[]>();
		// DisplaySolution(); // in gui, button which can download solution
	}

	public void DisplayAssignments(string[] a){
		foreach(var v in a ) Console.WriteLine(v);
	}

	public void DisplayAssignment(string[] a){
		foreach(var v in a ) Console.WriteLine(v);
	}

	public void DisplayAssignmentResult(AssignmentResult result){
		foreach(var testLog in result.TestLogs){
			Console.Write($"Test: {testLog.Name}");

			if( testLog.Result == TestResult.Correct ){
				Console.WriteLine(" ... OK");
				continue;
			}
			
			Console.WriteLine(" ... FAILED");

			if( testLog.Result == TestResult.OutputMismatch ){
				Console.WriteLine("Stdout actual:");
				Console.WriteLine($"{testLog.Stdout}");
				Console.WriteLine($"Stdou expected:"); // TODO also display Expected output
				continue;
			}
			
			if( testLog.Result == TestResult.TimeExceeded ){
				Console.WriteLine("Time exceeded");
				continue;
			}
			
			Console.WriteLine("Stderr:");
			Console.WriteLine($"{testLog.Stderr}");
		}

		Console.WriteLine();
		Console.Write($"Passed: {result.CorrectTests}, ");
		Console.Write($"Failed: {result.IncorrectTests}, ");
		Console.WriteLine($"Skipped: {result.SkippedTests}");
		Console.WriteLine($"Points: {result.PointsTotal}");
	}

	public IResponse<T> GetResponse<T>(){
				return transfer.Receive<IResponse<T>>();
		}

	public void Notify<T>(IRequest<T> notification){
				transfer.Send(notification);
		}

	private T GetData<T>(IRequest<object> update){
		return (T)(update.Data) ?? throw new InvalidOperationException($"");
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
