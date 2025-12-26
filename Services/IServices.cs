using ControllerManager.Models;

namespace ControllerManager.Services;

public interface IXInputService
{
    List<ControllerDevice> DetectDevices();
    bool IsDeviceConnected(int userIndex);
}

public interface IDirectInputService
{
    List<ControllerDevice> DetectDevices();
    void Dispose();
}

public interface IHidService
{
    List<ControllerDevice> DetectDevices();
}

public interface IConfigurationService
{
    AppSettings LoadSettings();
    void SaveSettings(AppSettings settings);
    List<GameProfile> LoadProfiles();
    void SaveProfile(GameProfile profile);
    void DeleteProfile(string profileId);
}

public interface INavigationService
{
    event EventHandler<string>? NavigationRequested;
    void NavigateTo(string pageName);
}
