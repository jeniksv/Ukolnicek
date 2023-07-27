using Communication;

namespace AppClient;

public class Student : User{
	public Student(string name, IObjectTransfer transfer) : base(name, transfer){}
}
