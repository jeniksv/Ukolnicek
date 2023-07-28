using System.Diagnostics;

namespace Communication;

// TODO inner logic, client has notifications and server has responses ?
public enum RequestEnum{
	SubmittedSolution,
	Login,
	Exit,
	CreateUser,
	ShowAssignments, // List<string>
	ShowAssignment, // zadani plus pokusy co byly List<string> + README.md
	ShowSolution, // AssignmentResult + *.py
	/*
	 * -> User student
	 * EvalSolution
	 * ShowMyAssignments ?
	 * -> User admin
	 * CreateAssignment
	 * AddTest
	 * DeleteTest
	 * GetTestInfo (s tim bude prcani ale)
	 * CreateGroup (s tim bude prcani tez)
	 * AssignTaskToStudent
	 * JesteNeco
	 */
}

public interface IRequest<out T>{
	public RequestEnum Type { get; }
	public T? Data { get; }
}

/// <summary>
///     Generic notification for communication.
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
