using Ukolnicek.Communication;

namespace Ukolnicek.Client;

public class Admin : User{
	public Admin(string name, IObjectTransfer transfer) : base(name, transfer) {}
}
