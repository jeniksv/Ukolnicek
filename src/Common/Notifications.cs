using System.Diagnostics;

namespace Communication;

// TODO inner logic, client has notifications and server has responses ?
public enum NotifEnum{
	SubmittedSolution,
	AssignmentResult,
	AskName,
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

public interface INotification<out T>{
	public NotifEnum Type { get; }
	public T? Data { get; }
}

/// <summary>
///     Generic notification for communication.
/// </summary>
public readonly struct Notification<T> : INotification<object>{
	public NotifEnum Type { get; }
	public T? Data { get; }

	public Notification(NotifEnum type, T data){
		Type = type;
		Data = data;
	}

	object? INotification<object>.Data => Data;
}

/// <summary>
///     Factory for creating notifications.
/// </summary>
public static class Notification{
	public static Notification<object> Create(NotifEnum type) => new Notification<object>(type, default!);

	public static Notification<T> Create<T>(NotifEnum type, T data) => new Notification<T>(type, data);
}
