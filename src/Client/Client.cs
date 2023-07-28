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

	// TODO change client server logic
	public User(string name, IObjectTransfer t){
		Name = name;
		transfer = t; 
		/*
		Id = transfer.Receive<Request<int>>().Data;
		transfer.Send( new Response<string> {Data = name} );
		*/
	}

	// TODO use events
	public void ClientLoop(){
		SubmitSolution();
		CreateUser();
		Notify( Request.Create(RequestEnum.Exit) );
	}

	private void SubmitSolution(){
		var c = new CustomFile("prime.py", File.ReadAllBytes("prime.py"));
		Notify( Request.Create(RequestEnum.SubmittedSolution, c) );
		var response = GetResponse<AssignmentResult>();
		DisplayAssignmentResult( response.Data );
	}

	private void CreateUser(){
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
	private static string ip = "192.168.10.87";
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
