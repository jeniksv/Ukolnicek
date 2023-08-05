namespace Ukolnicek.Communication;

public interface IResponse<out T>{
	T? Data { get; }
}

public class Response<T> : IResponse<T>{
	public T? Data { get; init; }
}
