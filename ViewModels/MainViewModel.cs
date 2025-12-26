using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private object? _currentView;
    private string _currentPage = "Home";

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public ICommand NavigateCommand { get; }

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigationItems = new ObservableCollection<NavigationItem>
        // images from https://emojidb.org/
        { 
            new() { Name = "Home", Icon = "🏠", Page = "Home" },
            new() { Name = "Controller", Icon = "🎮", Page = "Controller" },
            new() { Name = "Profiles", Icon = "👤", Page = "Profiles" },
            new() { Name = "Settings", Icon = "⚙️", Page = "Settings" },
            new() { Name = "Connection List", Icon = "🔗", Page = "ConnectionList" },
            new() { Name = "About", Icon = "ℹ️", Page = "About" }
        };

        NavigateCommand = new RelayCommand<string>(page =>
        {
            if (!string.IsNullOrEmpty(page))
            {
                _navigationService.NavigateTo(page);
            }
        });
    }
}

public class NavigationItem
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Page { get; set; } = string.Empty;
}
