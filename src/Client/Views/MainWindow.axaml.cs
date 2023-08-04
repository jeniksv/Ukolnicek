using Avalonia.Controls;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
    	AvaloniaXamlLoader.Load(this);
    }
}
