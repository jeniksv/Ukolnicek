using System.Diagnostics;

namespace Communication;

/// <summary>
///     Possible user actions.
/// </summary>
public enum RequestEnum{
	Login,
	CreateUser,
	Exit,

	ShowAssignments, // List<string>
	ShowAssignment, // zadani plus pokusy co byly List<string> + README.md
	ShowSolution, // AssignmentResult + *.py
	
	SubmittedSolution,
	
	// admin requests
	AssignTask, // TODO mozna AssignAll?

	CreateAssignment,
	AddTest,
	RemoveTest,

	/*
	 * -> Admin TODO
	 * CreateGroup (s tim bude prcani tez), mozna by pak bylo fajn videt body ostatnich, vytvareni predmetu jako c# delat nebudu
	 * UpdateStudentPoints
	 * ShowAllStudents
	 */
}

public interface IRequest<out T>{
	public RequestEnum Type { get; }
	public T? Data { get; }
}

/// <summary>
///     Generic user request for communication with server.
/// </summary>
public readonly struct Request<T> : IRequest<object>{
	public RequestEnum Type { get; }
	public T? Data { get; }

	public Request(RequestEnum type, T data){
		Type = type;
		Data = data;
	}

	object? IRequest<object>.Data => Data;
}

/// <summary>
///     Factory for creating notifications.
/// </summary>
public static class Request{
	public static Request<object> Create(RequestEnum type) => new Request<object>(type, default!);

	public static Request<T> Create<T>(RequestEnum type, T data) => new Request<T>(type, data);
}
