using Ukolnicek.Communication;
using Ukolnicek.Testing;

namespace Ukolnicek.Client;

public interface IUserInterface {
	void MainLoop();

	void ShowAssignment(string[] assignment);
	void ShowAssignments(string[] assignments);
	void ShowSolution(AssignmentResult result);
	void ShowTaskDescription(string description);

	void ShowGroup(string names);
	void ShowGroups(string[] groups);
	void ShowUsers(string[] users);

	void SetUser(User u);

	void AskUsername();
	void AskPassword();

	string GetUsername();
	string GetPassword();

	void AccountExists();
	void InvalidLogin();
	void InvalidArguments();
}

