using System.Text;
using System.Net;
using System.Net.Sockets;
using Communication;
using Testing;

namespace AppClient;

/// <summary>
/// pattern for classes Student and Admin
/// </summary>
public abstract class User{
	public string Name;
	public int Id;
	protected IObjectTransfer transfer;

	public User(string name, IObjectTransfer t){
		Name = name;
		transfer = t; 
	}

	// TODO use events
	public void ClientLoop(){
		//SubmitSolution();
		// CreateUser();
		// AssignTask();
		// CreateAssignment();
		ShowAssignments();
		ShowAssignment("Prime");
		Notify( Request.Create(RequestEnum.Exit) );
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
///     Factory for creating users.
/// </summary>
public static class Client{
	private static string ip = "192.168.0.199"; //192.168.0.199
	private static int port = 12345;

	public static User Create(){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		while(true){
			Console.Write("Enter your username: ");
			transfer.Send(Request.Create(RequestEnum.Login, Console.ReadLine()));
			Console.Write("Enter you password: "); // TODO hive passwd, should be in GUI
			transfer.Send(Request.Create(RequestEnum.Login, Console.ReadLine()));

			var verified = transfer.Receive<IResponse<bool>>().Data;
			
			if( verified ){
				var isAdmin = transfer.Receive<IResponse<bool>>().Data;
				return isAdmin ? new Admin("Jenda", transfer) : new Student("Ann", transfer); // TODO use user names
			}

			Console.WriteLine("Incorrect username or password.");
		}
	}
}
