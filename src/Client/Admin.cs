using Ukolnicek.Communication;

namespace Ukolnicek.Client;

public class Admin : User{
	public Admin(string name, IObjectTransfer transfer) : base(name, transfer) {}

	public override object HandleCommand(RequestEnum command, string[] args){
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
			case RequestEnum.ShowGroup:
				data = ShowGroup(args);
				break;
			case RequestEnum.ShowGroups:
				data = ShowGroups();
				break;
			case RequestEnum.ShowUsers:
				data = ShowUsers();
				break;
			case RequestEnum.AddSolution:
				AddSolution(args);
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
				break;
			case RequestEnum.AddAdmin:
				AddAdmin(args);
				break;
			case RequestEnum.AddGroup:
				AddGroup(args);
				break;
			case RequestEnum.RemoveGroup:
				RemoveGroup(args);
				break;
		}

		return data;
	}

	private void AddTest(string[] args){
		var p = new Parser(args);

		if( !p.CorrectArguments ){
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

	private void AddAssignment(string[] args){
		if( args.Length == 0 ){
			return;
		}

		Notify(Request.Create(RequestEnum.AddAssignment, args[0]));
	}

	private void AddTaskDescription(string[] args){
		if( args.Length < 2 ){
			return;
		}

		var data = new string[] {args[0], File.ReadAllText(args[1])};

		Notify(Request.Create(RequestEnum.AddTaskDescription, data));
	}

	private void RemoveTest(string[] args){
		if( args.Length < 2 ){
			return;
		}

		Notify( Request.Create(RequestEnum.RemoveTest, args));
	}

	private void RemoveAssignment(string[] args){
		if( args.Length == 0 ){
			return;
		}

		Notify( Request.Create(RequestEnum.RemoveAssignment, args[0]));
	}

	private void RemoveTaskDescription(string[] args){
		if( args.Length == 0 ){
			return;
		}

		Notify( Request.Create(RequestEnum.RemoveTaskDescription, args[0]));
	}

	private void AssignTask(string[] args){
		if( args.Length < 2 ){
			return;
		}

		Notify( Request.Create(RequestEnum.AssignTask, args) );
	}

	private void UnassignTask(string[] args){
		if( args.Length < 2 ){
			return;
		}

		Notify( Request.Create(RequestEnum.UnassignTask, args) );
	}

	private void AddAdmin(string[] args){
		Notify( Request.Create(RequestEnum.AddAdmin, args[0]) );
	}

	private void AddGroup(string[] args){
		Notify( Request.Create(RequestEnum.AddGroup, args) );
	}

	private string ShowGroup(string[] args){
		Notify( Request.Create(RequestEnum.ShowGroup, args[0]) );
		var response = GetResponse<string>();
		return response.Data!;
	}

	private string[] ShowGroups(){
		Notify( Request.Create(RequestEnum.ShowGroups) );
		var response = GetResponse<string[]>();
		return response.Data!;
	}

	private string[] ShowUsers(){
		Notify( Request.Create(RequestEnum.ShowUsers) );
		var response = GetResponse<string[]>();
		return response.Data!;
	}

	private void RemoveGroup(string[] args){
		Notify( Request.Create(RequestEnum.RemoveGroup, args[0]) );
	}
}
