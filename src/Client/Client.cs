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
		
		Notify(Notification.Create(NotifEnum.Verification, Name));
		Notify(Notification.Create(NotifEnum.Verification, Console.ReadLine()));
		var verified = GetResponse<bool>();
		Console.WriteLine(verified.Data);
		
		/*
		Id = transfer.Receive<Notification<int>>().Data;
		transfer.Send( new Response<string> {Data = name} );
		*/
	}

	// TODO use events
	public void ClientLoop(){
		/*
		var c = new CustomFile("prime.py", File.ReadAllBytes("prime.py"));
		transfer.Send( new Response<CustomFile> {Data = c} );

		while( true ){
			var update = transfer.Receive<INotification<object>>();
			Console.WriteLine($"{Name}: {update.Type}");

			try{
				HandleNotification(update);
			} catch(InvalidOperationException ex){
				Console.WriteLine($"Error: {ex.Message}");
			}
		} */
	}

	public void HandleNotification(INotification<object> update){
		/*
		switch(update.Type){
			case NotifEnum.AssignmentResult:
				DisplayAssignmentResult((AssignmentResult)update.Data);
				break;
		} */
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
		Console.Write($"Passed: {result.CorrectTests},");
		Console.Write($"Failed: {result.IncorrectTests},");
		Console.WriteLine($"Skipped: {result.SkippedTests}");
		Console.WriteLine($"Points: {result.PointsTotal}");
	}

	public IResponse<T> GetResponse<T>(){
                return transfer.Receive<IResponse<T>>();
        }

	public void Notify<T>(INotification<T> notification){
                transfer.Send(notification);
        }

	private T GetData<T>(INotification<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}
}

/// <summary>
///     Factory for creating clients.
/// </summary>
public static class Client{
	private static string ip = "192.168.10.87";
	private static int port = 12345;

	public static User Create(){
		IObjectTransfer transfer = new JsonTcpTransfer(ip, port);
		return new Admin("Ann", transfer);
	}
}
