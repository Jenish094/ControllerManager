using System.Collections.ObjectModel;
using ControllerManager.Models;

namespace ControllerManager.Services;

public class DeviceManager : IDeviceManager, IDisposable
{
    private readonly IXInputService _xInputService;
    private readonly IDirectInputService _directInputService;
    private readonly IHidService _hidService;
    private readonly ObservableCollection<ControllerDevice> _devices;
    private System.Threading.Timer? _pollingTimer;
    private bool _isMonitoring;

    public event EventHandler<ControllerDevice>? DeviceConnected;
    public event EventHandler<ControllerDevice>? DeviceDisconnected;

    public IReadOnlyList<ControllerDevice> ConnectedDevices => _devices;

    public DeviceManager(
        IXInputService xInputService,
        IDirectInputService directInputService,
        IHidService hidService)
    {
        _xInputService = xInputService;
        _directInputService = directInputService;
        _hidService = hidService;
        _devices = new ObservableCollection<ControllerDevice>();
    }

    public async Task InitializeAsync()
    {
        await Task.Run(() =>
        {
            DetectAllDevices();
        });
    }

    public Task StartMonitoringAsync()
    {
        if (_isMonitoring)
            return Task.CompletedTask;

        _isMonitoring = true;
        _pollingTimer = new System.Threading.Timer(
            _ => DetectAllDevices(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    public Task StopMonitoringAsync()
    {
        _isMonitoring = false;
        _pollingTimer?.Dispose();
        _pollingTimer = null;
        return Task.CompletedTask;
    }

    private void DetectAllDevices()
    {
        var currentDevices = new List<ControllerDevice>();

        //Find XInput
        try
        {
            currentDevices.AddRange(_xInputService.DetectDevices());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"XInput detection error: {ex.Message}");
        }

        //Find DInput
        try
        {
            currentDevices.AddRange(_directInputService.DetectDevices());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DirectInput detection error: {ex.Message}");
        }

        // Find HID
        try
        {
            currentDevices.AddRange(_hidService.DetectDevices());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HID detection error: {ex.Message}");
        }
        UpdateDeviceList(currentDevices);
    }

    private void UpdateDeviceList(List<ControllerDevice> currentDevices)
    {
        var disconnected = _devices
            .Where(d => !currentDevices.Any(cd => cd.InstanceId == d.InstanceId))
            .ToList();

        foreach (var device in disconnected)
        {
            _devices.Remove(device);
            DeviceDisconnected?.Invoke(this, device);
        }
        var newDevices = currentDevices
            .Where(cd => !_devices.Any(d => d.InstanceId == cd.InstanceId))
            .ToList();

        foreach (var device in newDevices)
        {
            _devices.Add(device);
            DeviceConnected?.Invoke(this, device);
        }
        foreach (var device in _devices)
        {
            var current = currentDevices.FirstOrDefault(cd => cd.InstanceId == device.InstanceId);
            if (current != null)
            {
                device.IsConnected = current.IsConnected;
                device.BatteryLevel = current.BatteryLevel;
            }
        }
    }

    public void Dispose()
    {
        StopMonitoringAsync().Wait();
        _directInputService.Dispose();
    }
}
