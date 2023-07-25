using System.Text;
using System.Net;
using System.Net.Sockets;
using Communication;
using Testing;

/// <summary>
///     Way to communicate with the server from the UI - it also holds the game state.
/// </summary>
public class Admin{
	public string Name;
	public int Id;
	// protected TcpClient client;
	protected IObjectTransfer transfer;

	public Admin(string name, string ip, int port){
		Name = name;
		transfer = new JsonTcpTransfer(ip, port);

		Id = transfer.Receive<Notification<int>>().Data;
		// Console.WriteLine("received!");
		transfer.Send( new Response<string> {Data = name} );
		// TODO verification in ctor?
	}

	// TODO use events
	public void ClientLoop(){
		while( true ){
			var update = transfer.Receive<INotification<object>>();
			Console.WriteLine($"{Name}: {update.Type}");

			try{
				HandleNotification(update);
			} catch(InvalidOperationException ex){
				Console.WriteLine($"Error: {ex.Message}");
			}

			//MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));
			//Thread.Sleep(100);
		}
	}

	public void HandleNotification(INotification<object> update){
		switch(update.Type){
			case NotifEnum.AskName:
				DisplayAssignmentResult((AssignmentResult)update.Data);
				break;
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
		Console.WriteLine($"Passed: {result.CorrectTests}, Failed: {result.IncorrectTests}, Skipped: {result.SkippedTests}");
		Console.WriteLine($"Points: {result.PointsTotal}");
	}

	private T GetData<T>(INotification<object> update){
		return (T)(update.Data ?? throw new InvalidOperationException($""));
	}
}
/*
public class Admin : Client{
	public Admin(string name) : base(name){}

}*/
