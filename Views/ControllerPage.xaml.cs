using System.Windows.Controls;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class ControllerPage : Page
{
    public ControllerPage(ControllerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
