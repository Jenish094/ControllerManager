using System.Windows.Controls;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class AboutPage : Page
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
