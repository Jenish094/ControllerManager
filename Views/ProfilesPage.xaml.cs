using System.Windows.Controls;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class ProfilesPage : Page
{
    public ProfilesPage(ProfilesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
