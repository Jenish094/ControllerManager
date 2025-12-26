using ControllerManager.Models;

namespace ControllerManager.Services;

public interface IDeviceManager
{
    event EventHandler<ControllerDevice>? DeviceConnected;
    event EventHandler<ControllerDevice>? DeviceDisconnected;
    
    IReadOnlyList<ControllerDevice> ConnectedDevices { get; }
    
    Task InitializeAsync();
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
    void Dispose();
}
