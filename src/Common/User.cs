namespace AppLogic;

public abstract class User{
	public bool IsAdmin { get; set; }
}

public class Student : User{
}

public class Admin : User{
}
