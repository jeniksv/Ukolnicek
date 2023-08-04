using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Views;

public partial class StudentMainView : UserControl
{
    public StudentMainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
