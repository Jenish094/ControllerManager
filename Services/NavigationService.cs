namespace ControllerManager.Services;

public class NavigationService : INavigationService
{
    public event EventHandler<string>? NavigationRequested;

    public void NavigateTo(string pageName)
    {
        NavigationRequested?.Invoke(this, pageName);
    }
}
