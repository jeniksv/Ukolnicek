/*
using System.Text;
using System.Net;
using System.Net.Sockets;
using Communication;
using Testing;

/// <summary>
///     Way to communicate with the server from the UI - it also holds the game state.
/// </summary>
public abstract class Client{
	public string Name;
	protected TcpClient client;
	protected IObjectTransfer transfer;

	public Client(string name){
		Name = name;
		client = new TcpClient();
		client.Connect("192.168.10.87", 12345);
		transfer = new JsonTcpTransfer(client);
		// TODO verification in ctor?
	}

	// TODO use events
	public void ClientLoop(){
		while( true ){
			var update = transfer.Receive<Notification<object>>();

			try{
				HandleNotification(update);
			} catch(InvalidOperationException ex){
				Console.WriteLine($"Error: {ex.Message}");
			}

			//MessageReceived?.Invoke(this, new MessageReceivedEventArgs(update.Data, update.Type));
			//Thread.Sleep(100);
		}
	}

	public void HandleNotification(Notification<object> update){
		switch(update.Type){
			case NotifyEnum.TestResult:
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
		Console.WriteLine($"Passed: {result.CorrectTests}, Failed: {result.IncorrectTests}, Skipped: {result.Skipped}");
		Console.WriteLine($"Points: {result.PointsTotal}");
	}
}

public class Admin : Client{
	public Admin(string name) : base(name){}

}*/
