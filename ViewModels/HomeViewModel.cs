using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private readonly IVibrationService _vibrationService;
    private readonly ILedControlService _ledControl;
    private readonly IRemappingService _remappingService;
    private readonly IConfigurationService _configService;
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

    public HomeViewModel(
        IDeviceManager deviceManager,
        IVibrationService vibrationService,
        ILedControlService ledControl,
        IRemappingService remappingService,
        IConfigurationService configService)
    {
        _deviceManager = deviceManager;
        _vibrationService = vibrationService;
        _ledControl = ledControl;
        _remappingService = remappingService;
        _configService = configService;
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
            var all = ConnectedDevices.Concat(new[] { device })
                .OrderBy(d => GetPriority(d.InputType))
                .ThenBy(d => d.Name)
                .ToList();
            ConnectedDevices.Clear();
            foreach (var d in all) ConnectedDevices.Add(d);
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

    private static int GetPriority(Models.InputType input)
    {
        return input switch
        {
            Models.InputType.XInput => 0,
            Models.InputType.DirectInput => 1,
            Models.InputType.HID => 2,
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
        if (SelectedDevice != null)
        {
            _vibrationService.TestVibration(SelectedDevice);
        }
    }

    private void ChangeProfile()
    {
        if (SelectedDevice == null)
            return;

        var profiles = _configService.LoadProfiles();
        if (profiles.Any())
        {
            var currentIndex = profiles.FindIndex(p => p.Name == SelectedDevice.CurrentProfile);
            var nextIndex = (currentIndex + 1) % profiles.Count;
            var nextProfile = profiles[nextIndex];
            
            SelectedDevice.CurrentProfile = nextProfile.Name;
            if (_remappingService.IsRemapping(SelectedDevice.InstanceId))
            {
                _remappingService.SetProfile(SelectedDevice.InstanceId, nextProfile);
            }
            
            UpdateStatusMessage();
        }
    }

    private void ChangeLedColor()
    {
        if (SelectedDevice == null)
            return;

        if (_ledControl.SupportsLedControl(SelectedDevice))
        {
            var colors = new[] { (0, 120, 255), (255, 0, 0), (0, 255, 0), (255, 255, 0), (255, 0, 255) };
            var random = new Random();
            var color = colors[random.Next(colors.Length)];
            _ledControl.SetLedColor(SelectedDevice, (byte)color.Item1, (byte)color.Item2, (byte)color.Item3);
        }
    }
}
