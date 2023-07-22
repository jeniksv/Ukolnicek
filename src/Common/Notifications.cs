using System.Diagnostics;

namespace Communication;

public enum NotifEnum{
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

public struct Notification<T>{
	public NotifEnum Type { get; }
	public T? Data;
	
	public Notification(NotifEnum type, T? data){
		Type = type;
		Data = data;
	}
}

public static class Notification{
	public static Notification<T> Create<T>(NotifEnum type, T data) => new Notification<T>(type, data);
}
