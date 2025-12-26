using System.Windows;
using ControllerManager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ControllerManager;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        this.DispatcherUnhandledException += (s, ex) =>
        {
            System.Diagnostics.Debug.WriteLine($"DispatcherUnhandledException: {ex.Exception}");
            MessageBox.Show(ex.Exception.Message, "Unhandled UI Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            ex.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
        {
            System.Diagnostics.Debug.WriteLine($"UnhandledException: {ex.ExceptionObject}");
        };

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<IDeviceManager, DeviceManager>();
        services.AddSingleton<IXInputService, XInputService>();
        services.AddSingleton<IDirectInputService, DirectInputService>();
        services.AddSingleton<IHidService, HidService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ViewModels.MainViewModel>();
        services.AddTransient<ViewModels.HomeViewModel>();
        services.AddTransient<ViewModels.ControllerViewModel>();
        services.AddTransient<ViewModels.ProfilesViewModel>();
        services.AddTransient<ViewModels.SettingsViewModel>();
        services.AddTransient<ViewModels.ConnectionListViewModel>();
        services.AddTransient<ViewModels.AboutViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<Views.HomePage>();
        services.AddTransient<Views.ControllerPage>();
        services.AddTransient<Views.ProfilesPage>();
        services.AddTransient<Views.SettingsPage>();
        services.AddTransient<Views.ConnectionListPage>();
        services.AddTransient<Views.AboutPage>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
