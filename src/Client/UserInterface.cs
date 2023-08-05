using Ukolnicek.Communication;

namespace Ukolnicek.Client;

public interface IUserInterface {
	//void ShowAssignment();
	//void ShowAssignments();
	void ShowSolution();

	void AskUsername();
	void AskPassword();
	void InvalidLogin();
	
	string GetUsername();
	string GetPassword();
	
	RequestEnum GetCommand();
}

