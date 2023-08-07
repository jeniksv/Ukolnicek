using System.Diagnostics;

namespace Ukolnicek.Communication;

/// <summary>
///     Possible user actions.
/// </summary>
public enum RequestEnum{
	Login,
	CreateAccount, 
	Exit,

	ShowAssignments, // string[]
	ShowAssignment, // for admin user should be specified
	ShowSolution, // + *.py
	ShowTaskDescription,
	ShowGroup, 
	ShowGroups, 

	ShowStudents, // TODO

	AssignTask, 
	UnassignTask,

	AddAssignment,
	AddTest,
	AddTaskDescription,
	AddGroup,
	AddSolution, // = SubmittedSolution
	AddComment, // TODO
	AddAdmin, 

	RemoveTest,
	RemoveAssignment,
	RemoveTaskDescription,
	RemoveGroup,

	UpdatePoints, // TODO

	/*
	 * -> Admin TODO
	 *  manualni oprava bodu, moznost napsat komentar
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
