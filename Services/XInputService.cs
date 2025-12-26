using ControllerManager.Models;
using SharpDX.XInput;

namespace ControllerManager.Services;

public class XInputService : IXInputService
{
    private readonly Controller[] _controllers = new Controller[4];

    public XInputService()
    {
        for (int i = 0; i < 4; i++)
        {
            _controllers[i] = new Controller((UserIndex)i);
        }
    }

    public List<ControllerDevice> DetectDevices()
    {
        var devices = new List<ControllerDevice>();

        for (int i = 0; i < 4; i++)
        {
            if (_controllers[i].IsConnected)
            {
                var device = new ControllerDevice
                {
                    InstanceId = $"XInput_{i}",
                    Name = GetControllerName(i),
                    Type = DetermineControllerType(i),
                    InputType = InputType.XInput,
                    ConnectionType = ConnectionType.Unknown,
                    IsConnected = true,
                    ConnectedTime = DateTime.Now,
                    UserIndex = i,
                    BatteryLevel = GetBatteryLevel(i)
                };

                devices.Add(device);
            }
        }

        return devices;
    }

    public bool IsDeviceConnected(int userIndex)
    {
        if (userIndex < 0 || userIndex >= 4)
            return false;

        return _controllers[userIndex].IsConnected;
    }

    private string GetControllerName(int userIndex)
    {
        try
        {
            var capabilities = _controllers[userIndex].GetCapabilities(DeviceQueryType.Any);
            if (capabilities.SubType == DeviceSubType.Gamepad)
            {
                return $"Xbox Controller {userIndex + 1}";
            }
            
            return $"XInput Device {userIndex + 1}";
        }
        catch
        {
            return $"XInput Device {userIndex + 1}";
        }
    }

    private ControllerType DetermineControllerType(int userIndex)
    {
        try
        {
            var capabilities = _controllers[userIndex].GetCapabilities(DeviceQueryType.Any);
            
            return capabilities.SubType switch
            {
                DeviceSubType.Gamepad => ControllerType.XboxOne,
                DeviceSubType.Wheel => ControllerType.Generic,
                DeviceSubType.ArcadeStick => ControllerType.Generic,
                DeviceSubType.FlightStick => ControllerType.Generic,
                DeviceSubType.DancePad => ControllerType.Generic,
                DeviceSubType.Guitar => ControllerType.Generic,
                DeviceSubType.DrumKit => ControllerType.Generic,
                _ => ControllerType.Unknown
            };
        }
        catch
        {
            return ControllerType.Unknown;
        }
    }

    private int GetBatteryLevel(int userIndex)
    {
        try
        {
            var batteryInfo = _controllers[userIndex].GetBatteryInformation(BatteryDeviceType.Gamepad);
            
            return batteryInfo.BatteryLevel switch
            {
                BatteryLevel.Empty => 0,
                BatteryLevel.Low => 25,
                BatteryLevel.Medium => 50,
                BatteryLevel.Full => 100,
                _ => -1
            };
        }
        catch
        {
            return -1;
        }
    }
}
