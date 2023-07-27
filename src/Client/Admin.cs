using Communication;

namespace AppClient;

public class Admin : User{
	public Admin(string name, IObjectTransfer transfer) : base(name, transfer) {}
}
