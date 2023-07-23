using System.Diagnostics;

namespace Communication;

public enum NotifEnum{
	SubmittedSolution,
	AssignmentResult,
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

/// <summary>
///     Generic notification for communication.
/// </summary>
public struct Notification<T>{
	public NotifEnum Type { get; }
	public T? Data { get; }
	
	public Notification(NotifEnum type, T data){
		Type = type;
		Data = data;
	}
}

/// <summary>
///     Factory for creating notifications.
/// </summary>
public static class Notification{
	public static Notification<object> Create(NotifEnum type) => new Notification<object>(type, default!);

	public static Notification<T> Create<T>(NotifEnum type, T data) => new Notification<T>(type, data);
}

/// <summary>
///     Generic interface for responses to requests from the client.
/// </summary>
public class Response<T>{
	public T? Data { get; set; }
}
