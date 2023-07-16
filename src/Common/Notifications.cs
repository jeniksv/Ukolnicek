using System.Diagnostics;

namespace Communication;

public enum NotifEnum{
	/*
	TestsResult,
	RunTests,
	AddTest,
	*/
}

public struct Notification<T>{
	public NotifEnum Type { get; }
	public T? Data
	
	public Notification(NotifEnum type, T? data = null){
		Type = type;
		Data = data;
	}
}

public static class Notification{
	public static Notification<T> Create<T>(NotifEnum type, T data) => new Notification<T>(type, data);
}
