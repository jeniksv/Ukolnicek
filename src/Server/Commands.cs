using Ukolnicek.Communication;

namespace Ukolnicek.Server;

public interface ICommand {
	void Execute(IRequest<object> request);
}

public class Login : ICommand {
	public void Execute(IRequest<object> request){}
}
