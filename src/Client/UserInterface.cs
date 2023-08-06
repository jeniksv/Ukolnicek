using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client;

public interface IUserInterface {
	// void ShowAssignment();
	void ShowAssignments(string[] assignments);
	void ShowSolution(AssignmentResult result);

	void AskUsername();
	void AskPassword();
	void InvalidLogin();
	void InvalidArguments();
	
	string GetUsername();
	string GetPassword();

	RequestEnum GetCommand(out string[] args);
}

