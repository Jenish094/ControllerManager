using System.Collections.ObjectModel;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class ConnectionListViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;

    public ObservableCollection<ControllerDevice> Devices { get; }

    public ConnectionListViewModel(IDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
        Devices = new ObservableCollection<ControllerDevice>();

        LoadDevices();
        _deviceManager.DeviceConnected += (s, d) => LoadDevices();
        _deviceManager.DeviceDisconnected += (s, d) => LoadDevices();
    }

    private void LoadDevices()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Devices.Clear();
            //prioritise xinput/dinput because mouse and keyboard are always at the top of the list
            var prioritized = _deviceManager.ConnectedDevices
                .OrderBy(d => GetPriority(d.InputType))
                .ThenBy(d => d.Name);

            foreach (var device in prioritized)
            {
                Devices.Add(device);
            }
        });
    }

    private static int GetPriority(InputType input)
    {
        return input switch
        {
            InputType.XInput => 0,
            InputType.DirectInput => 1,
            InputType.HID => 2,
            _ => 3
        };
    }
}
