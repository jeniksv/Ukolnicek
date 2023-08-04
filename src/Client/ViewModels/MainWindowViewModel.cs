using ReactiveUI;

namespace Client.ViewModels;

public class MainWindowViewModel : ViewModelBase {
	private ViewModelBase contentViewModel;

	public ViewModelBase ContentViewModel{
		get => contentViewModel;
		set => this.RaiseAndSetIfChanged(ref contentViewModel, value);
	}

	public MainWindowViewModel(){
		contentViewModel = new LoginViewModel();
	}

	public string Greeting => "Welcome to Avalonia!";

	public void AddItem(){
		ContentViewModel = new StudentMainViewModel();
	}
}
