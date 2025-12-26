using System.Windows.Controls;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
