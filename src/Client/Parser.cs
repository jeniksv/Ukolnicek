using Ukolnicek.Communication;

namespace Ukolnicek.Client;

// TODO more generic (next time start earlier then week before deadline)
public class Parser {
	// add-test [assignment name] [test name] --out [file] --in [file] --args [file] --time --points
	public string? AssignmentName;
	public string? TestName;
	public string? InputFileName;
	public string? OutputFileName;
	public string? ArgsFileName;
	public int? Time;
	public int? Points;

	public bool CorrectArguments = false;

	private string[] args;

	public Parser(string[] command){
		args = command;
		Parse();
	}

	public void Parse(){
		// TODO check for file permissions
		if( args.Length < 2 ) return;

		AssignmentName = args[0];
		TestName = args[1];

		for(int i=2; i<args.Length; i++){
			if( args.Length < i + 1 ) return;

			// TODO hodit to do slovniku, ale na to uz neni cas
			if(args[i] == "--out"){
				if( !File.Exists(args[i+1]) ) return;
				OutputFileName = args[i+1];
			}
			else if (args[i] == "--in"){
				if( !File.Exists(args[i+1]) ) return;
				InputFileName = args[i+1];
			}
			else if (args[i] == "--args"){
				if( !File.Exists(args[i+1]) ) return;
				ArgsFileName = args[i+1];
			} else if (args[i] == "--time"){
				Console.WriteLine(int.TryParse(args[i+1], out int r));
				if( int.TryParse(args[i+1], out int result) && result > 0 ) Time = result;
				else return;
			} else if(args[i] == "--points"){
				Console.WriteLine(int.TryParse(args[i+1], out int r));
				if( int.TryParse(args[i+1], out int result) && result > 0 ) Points = result;
				else return;
			} else {
				return;
			}

			i++; // in case of argument without value just continue;
		}

		CorrectArguments = true;
	}
}
