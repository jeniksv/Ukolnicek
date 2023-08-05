using Ukolnicek.Communication;

namespace Ukolnicek.Client;

public class Student : User{
	public Student(string name, IObjectTransfer transfer) : base(name, transfer){}
}
