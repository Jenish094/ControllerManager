using System.Collections.ObjectModel;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class ControllerViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private ControllerDevice? _selectedDevice;
    private string _connectionStatus = "Disconnected";

    public ObservableCollection<ControllerDevice> Controllers { get; }

    public ControllerDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            SetProperty(ref _selectedDevice, value);
            UpdateConnectionStatus();
        }
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    public ControllerViewModel(IDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
        Controllers = new ObservableCollection<ControllerDevice>();

        LoadControllers();
        _deviceManager.DeviceConnected += (s, d) => LoadControllers();
        _deviceManager.DeviceDisconnected += (s, d) => LoadControllers();
    }

    private void LoadControllers()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Controllers.Clear();
            foreach (var device in _deviceManager.ConnectedDevices)
            {
                Controllers.Add(device);
            }

            if (Controllers.Any() && SelectedDevice == null)
            {
                SelectedDevice = Controllers[0];
            }
        });
    }

    private void UpdateConnectionStatus()
    {
        if (SelectedDevice != null)
        {
            ConnectionStatus = SelectedDevice.IsConnected ? "Connected" : "Disconnected";
        }
        else
        {
            ConnectionStatus = "No controller selected";
        }
    }
}
