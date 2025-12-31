using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class ControllerViewModel : ViewModelBase
{
    private readonly IDeviceManager _deviceManager;
    private readonly IRemappingService _remappingService;
    private readonly IConfigurationService _configService;
    private readonly IInputPollingService _inputPollingService;
    private ControllerDevice? _selectedDevice;
    private string _connectionStatus = "Disconnected";
    private bool _isRemappingActive;
    private bool _lt_Pressed;
    private bool _lb_Pressed;
    private bool _rt_Pressed;
    private bool _rb_Pressed;
    private bool _a_Pressed;
    private bool _b_Pressed;
    private bool _x_Pressed;
    private bool _y_Pressed;
    private bool _dpadUp_Pressed;
    private bool _dpadDown_Pressed;
    private bool _dpadLeft_Pressed;
    private bool _dpadRight_Pressed;
    private bool _menu_Pressed;
    private bool _view_Pressed;
    private bool _leftThumb_Pressed;
    private bool _rightThumb_Pressed;
    private double _leftStickX;
    private double _leftStickY;
    private double _rightStickX;
    private double _rightStickY;

    public ObservableCollection<ControllerDevice> Controllers { get; }

    public ControllerDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (_selectedDevice != null && _selectedDevice.InstanceId != value?.InstanceId)
            {
                _inputPollingService.StopPolling(_selectedDevice.InstanceId);
            }

            SetProperty(ref _selectedDevice, value);
            
            if (_selectedDevice != null)
            {
                _inputPollingService.StartPolling(_selectedDevice);
            }

            UpdateConnectionStatus();
        }
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    public bool IsRemappingActive
    {
        get => _isRemappingActive;
        set => SetProperty(ref _isRemappingActive, value);
    }

    public bool LT_Pressed
    {
        get => _lt_Pressed;
        set => SetProperty(ref _lt_Pressed, value);
    }

    public bool LB_Pressed
    {
        get => _lb_Pressed;
        set => SetProperty(ref _lb_Pressed, value);
    }

    public bool RT_Pressed
    {
        get => _rt_Pressed;
        set => SetProperty(ref _rt_Pressed, value);
    }

    public bool RB_Pressed
    {
        get => _rb_Pressed;
        set => SetProperty(ref _rb_Pressed, value);
    }

    public bool A_Pressed
    {
        get => _a_Pressed;
        set => SetProperty(ref _a_Pressed, value);
    }

    public bool B_Pressed
    {
        get => _b_Pressed;
        set => SetProperty(ref _b_Pressed, value);
    }

    public bool X_Pressed
    {
        get => _x_Pressed;
        set => SetProperty(ref _x_Pressed, value);
    }

    public bool Y_Pressed
    {
        get => _y_Pressed;
        set => SetProperty(ref _y_Pressed, value);
    }

    public bool DPadUp_Pressed
    {
        get => _dpadUp_Pressed;
        set => SetProperty(ref _dpadUp_Pressed, value);
    }

    public bool DPadDown_Pressed
    {
        get => _dpadDown_Pressed;
        set => SetProperty(ref _dpadDown_Pressed, value);
    }

    public bool DPadLeft_Pressed
    {
        get => _dpadLeft_Pressed;
        set => SetProperty(ref _dpadLeft_Pressed, value);
    }

    public bool DPadRight_Pressed
    {
        get => _dpadRight_Pressed;
        set => SetProperty(ref _dpadRight_Pressed, value);
    }

    public bool Menu_Pressed
    {
        get => _menu_Pressed;
        set => SetProperty(ref _menu_Pressed, value);
    }

    public bool View_Pressed
    {
        get => _view_Pressed;
        set => SetProperty(ref _view_Pressed, value);
    }

    public bool LeftThumb_Pressed
    {
        get => _leftThumb_Pressed;
        set => SetProperty(ref _leftThumb_Pressed, value);
    }

    public bool RightThumb_Pressed
    {
        get => _rightThumb_Pressed;
        set => SetProperty(ref _rightThumb_Pressed, value);
    }

    public double LeftStickX
    {
        get => _leftStickX;
        set => SetProperty(ref _leftStickX, value);
    }

    public double LeftStickY
    {
        get => _leftStickY;
        set => SetProperty(ref _leftStickY, value);
    }

    public double RightStickX
    {
        get => _rightStickX;
        set => SetProperty(ref _rightStickX, value);
    }

    public double RightStickY
    {
        get => _rightStickY;
        set => SetProperty(ref _rightStickY, value);
    }

    public ICommand ToggleRemappingCommand { get; }

    public ControllerViewModel(
        IDeviceManager deviceManager,
        IRemappingService remappingService,
        IConfigurationService configService,
        IInputPollingService inputPollingService)
    {
        _deviceManager = deviceManager;
        _remappingService = remappingService;
        _configService = configService;
        _inputPollingService = inputPollingService;
        Controllers = new ObservableCollection<ControllerDevice>();

        ToggleRemappingCommand = new RelayCommand(ToggleRemapping, () => SelectedDevice != null);
        _inputPollingService.InputStateChanged += OnInputStateChanged;

        LoadControllers();
        _deviceManager.DeviceConnected += (s, d) => LoadControllers();
        _deviceManager.DeviceDisconnected += (s, d) => LoadControllers();
    }

    private void OnInputStateChanged(object? sender, InputStateEventArgs e)
    {
        if (SelectedDevice?.InstanceId != e.DeviceId)
            return;

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var buttonState = e.State;
            LT_Pressed = buttonState.LeftTrigger > 127;
            LB_Pressed = buttonState.LeftBumper;
            RT_Pressed = buttonState.RightTrigger > 127;
            RB_Pressed = buttonState.RightBumper;
            A_Pressed = buttonState.A;
            B_Pressed = buttonState.B;
            X_Pressed = buttonState.X;
            Y_Pressed = buttonState.Y;
            DPadUp_Pressed = buttonState.DPadUp;
            DPadDown_Pressed = buttonState.DPadDown;
            DPadLeft_Pressed = buttonState.DPadLeft;
            DPadRight_Pressed = buttonState.DPadRight;
            Menu_Pressed = buttonState.Back;
            View_Pressed = buttonState.Start;
            LeftThumb_Pressed = buttonState.LeftThumb;
            RightThumb_Pressed = buttonState.RightThumb;
            LeftStickX = (buttonState.LeftThumbX / 32768.0) * 25;
            LeftStickY = -(buttonState.LeftThumbY / 32768.0) * 25;
            RightStickX = (buttonState.RightThumbX / 32768.0) * 25;
            RightStickY = -(buttonState.RightThumbY / 32768.0) * 25;
        });
    }

    private async void ToggleRemapping()
    {
        if (SelectedDevice == null)
            return;

        if (IsRemappingActive)
        {
            await _remappingService.StopRemappingAsync(SelectedDevice.InstanceId);
            IsRemappingActive = false;
            ConnectionStatus = "Remapping Stopped";
        }
        else
        {
            var profiles = _configService.LoadProfiles();
            var profile = profiles.FirstOrDefault(p => p.Name == SelectedDevice.CurrentProfile);
            
            await _remappingService.StartRemappingAsync(SelectedDevice, profile);
            IsRemappingActive = true;
            ConnectionStatus = "Remapping Active";
        }
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
            var isRemapping = _remappingService.IsRemapping(SelectedDevice.InstanceId);
            IsRemappingActive = isRemapping;
            ConnectionStatus = SelectedDevice.IsConnected 
                ? (isRemapping ? "Connected - Remapping Active" : "Connected") 
                : "Disconnected";
        }
        else
        {
            ConnectionStatus = "No controller selected";
            IsRemappingActive = false;
        }
    }
}
