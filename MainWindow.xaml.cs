using System.Windows;
using ControllerManager.Services;
using ControllerManager.ViewModels;
using ControllerManager.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ControllerManager;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDeviceManager _deviceManager;
    private readonly INavigationService _navigationService;

    public MainWindow(
        MainViewModel viewModel,
        IServiceProvider serviceProvider,
        IDeviceManager deviceManager,
        INavigationService navigationService)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        _serviceProvider = serviceProvider;
        _deviceManager = deviceManager;
        _navigationService = navigationService;

        _navigationService.NavigationRequested += OnNavigationRequested;
        
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        //Init device manager
        await _deviceManager.InitializeAsync();
        await _deviceManager.StartMonitoringAsync();

        //Go to Home
        NavigateToPage("Home");
    }

    private void OnNavigationRequested(object? sender, string pageName)
    {
        NavigateToPage(pageName);
    }

    private void NavigateToPage(string pageName)
    {
        object? page = pageName switch
        {
            "Home" => _serviceProvider.GetRequiredService<HomePage>(),
            "Controller" => _serviceProvider.GetRequiredService<ControllerPage>(),
            "Profiles" => _serviceProvider.GetRequiredService<ProfilesPage>(),
            "Settings" => _serviceProvider.GetRequiredService<SettingsPage>(),
            "ConnectionList" => _serviceProvider.GetRequiredService<ConnectionListPage>(),
            "About" => _serviceProvider.GetRequiredService<AboutPage>(),
            _ => null
        };

        if (page != null)
        {
            MainFrame.Navigate(page);
        }
    }

    protected override async void OnClosed(EventArgs e)
    {
        await _deviceManager.StopMonitoringAsync();
        _deviceManager.Dispose();
        base.OnClosed(e);
    }
}
