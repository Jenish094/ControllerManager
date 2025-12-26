using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private ControllerDevice? _selectedDevice;
    private string _statusMessage = "No controller connected";

    public ObservableCollection<ControllerDevice> ConnectedDevices { get; }

    public ControllerDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            SetProperty(ref _selectedDevice, value);
            UpdateStatusMessage();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand TestVibrationCommand { get; }
    public ICommand ChangeProfileCommand { get; }
    public ICommand ChangeLedColorCommand { get; }

    public HomeViewModel(IDeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
        ConnectedDevices = new ObservableCollection<ControllerDevice>();

        _deviceManager.DeviceConnected += OnDeviceConnected;
        _deviceManager.DeviceDisconnected += OnDeviceDisconnected;

        TestVibrationCommand = new RelayCommand(TestVibration, CanExecuteDeviceCommand);
        ChangeProfileCommand = new RelayCommand(ChangeProfile, CanExecuteDeviceCommand);
        ChangeLedColorCommand = new RelayCommand(ChangeLedColor, CanExecuteDeviceCommand);

        LoadDevices();
        UpdateStatusMessage();
    }

    private void LoadDevices()
    {
        ConnectedDevices.Clear();
        var prioritized = _deviceManager.ConnectedDevices
            .OrderBy(d => GetPriority(d.InputType))
            .ThenBy(d => d.Name);

        foreach (var device in prioritized)
        {
            ConnectedDevices.Add(device);
        }

        if (ConnectedDevices.Any())
        {
            SelectedDevice = ConnectedDevices[0];
        }
    }

    private void OnDeviceConnected(object? sender, ControllerDevice device)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            //rebuilt list with the prioritying from connectionlistviewmodel.cs
            var all = ConnectedDevices.Concat(new[] { device })
                .OrderBy(d => GetPriority(d.InputType))
                .ThenBy(d => d.Name)
                .ToList();
            ConnectedDevices.Clear();
            foreach (var d in all) ConnectedDevices.Add(d);

            //select the top most device on the list as the status device
            SelectedDevice = ConnectedDevices.FirstOrDefault();
            UpdateStatusMessage();
        });
    }

    private void OnDeviceDisconnected(object? sender, ControllerDevice device)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var toRemove = ConnectedDevices.FirstOrDefault(d => d.InstanceId == device.InstanceId);
            if (toRemove != null)
            {
                ConnectedDevices.Remove(toRemove);
            }

            SelectedDevice = ConnectedDevices
                .OrderBy(d => GetPriority(d.InputType))
                .ThenBy(d => d.Name)
                .FirstOrDefault();

            UpdateStatusMessage();
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

    private void UpdateStatusMessage()
    {
        if (SelectedDevice != null)
        {
            StatusMessage = $"Connected: {SelectedDevice.Name}";
        }
        else if (ConnectedDevices.Any())
        {
            StatusMessage = $"{ConnectedDevices.Count} controller(s) connected";
        }
        else
        {
            StatusMessage = "No controller connected";
        }
    }

    private bool CanExecuteDeviceCommand() => SelectedDevice != null;

    private void TestVibration()
    {
        // TODO: Add vibration test logic
        System.Diagnostics.Debug.WriteLine("Test Vibration clicked");
    }

    private void ChangeProfile()
    {
        // TODO:Add profile changing logic
        System.Diagnostics.Debug.WriteLine("Change Profile clicked");
    }

    private void ChangeLedColor()
    {
        // TODO: Add LED colour switching logic
        System.Diagnostics.Debug.WriteLine("Change LED Color clicked");
    }
}
