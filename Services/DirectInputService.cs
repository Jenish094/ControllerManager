using ControllerManager.Models;
using SharpDX.DirectInput;

namespace ControllerManager.Services;

public class DirectInputService : IDirectInputService, IDisposable
{
    private readonly DirectInput _directInput;
    private readonly List<Joystick> _activeJoysticks;

    public DirectInputService()
    {
        _directInput = new DirectInput();
        _activeJoysticks = new List<Joystick>();
    }

    public List<ControllerDevice> DetectDevices()
    {
        var devices = new List<ControllerDevice>();

        try
        {
            // Fetch all connected controllers
            var deviceInstances = _directInput.GetDevices(
                DeviceClass.GameControl,
                DeviceEnumerationFlags.AttachedOnly);

            foreach (var deviceInstance in deviceInstances)
            {
                try
                {
                    var device = new ControllerDevice
                    {
                        InstanceId = $"DInput_{deviceInstance.InstanceGuid}",
                        Name = deviceInstance.ProductName,
                        Type = DetermineControllerType(deviceInstance),
                        InputType = InputType.DirectInput,
                        ConnectionType = DetermineConnectionType(deviceInstance),
                        IsConnected = true,
                        ConnectedTime = DateTime.Now,
                        DeviceGuid = deviceInstance.InstanceGuid,
                        VendorId = deviceInstance.ProductGuid.GetHashCode(),
                        ProductId = deviceInstance.InstanceGuid.GetHashCode(),
                        BatteryLevel = -1
                    };

                    devices.Add(device);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing DirectInput device: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DirectInput enumeration error: {ex.Message}");
        }

        return devices;
    }

    private ControllerType DetermineControllerType(DeviceInstance deviceInstance)
    {
        var productName = deviceInstance.ProductName.ToLower();

        if (productName.Contains("xbox"))
            return ControllerType.XboxOne;
        if (productName.Contains("playstation") || productName.Contains("dualshock") || productName.Contains("ps3") || productName.Contains("ps4"))
            return ControllerType.PS4;
        if (productName.Contains("dualsense") || productName.Contains("ps5"))
            return ControllerType.PS5;
        if (productName.Contains("switch") || productName.Contains("pro controller"))
            return ControllerType.Switch;

        //check type
        return deviceInstance.Type switch
        {
            DeviceType.Gamepad => ControllerType.Generic,
            DeviceType.Joystick => ControllerType.Generic,
            _ => ControllerType.Unknown
        };
    }

    private ConnectionType DetermineConnectionType(DeviceInstance deviceInstance)
    {
        var productName = deviceInstance.ProductName.ToLower();

        if (productName.Contains("bluetooth") || productName.Contains("wireless"))
            return ConnectionType.Bluetooth;
        if (productName.Contains("usb"))
            return ConnectionType.USB;

        //default all wired devices to usb connection type
        return ConnectionType.USB;
    }

    public void Dispose()
    {
        foreach (var joystick in _activeJoysticks)
        {
            joystick?.Dispose();
        }
        _activeJoysticks.Clear();
        _directInput?.Dispose();
    }
}
