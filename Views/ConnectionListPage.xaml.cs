using System.Windows.Controls;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class ConnectionListPage : Page
{
    public ConnectionListPage(ConnectionListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
