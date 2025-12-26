namespace ControllerManager.Models;

public enum ControllerType
{
    Unknown,
    Xbox360,
    XboxOne,
    XboxSeries,
    PS3,
    PS4,
    PS5,
    Switch,
    Generic
}

public enum ConnectionType
{
    Unknown,
    USB,
    Bluetooth,
    Wireless
}

public enum InputType
{
    XInput,
    DirectInput,
    HID
}

public class ControllerDevice
{
    public string InstanceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ControllerType Type { get; set; }
    public InputType InputType { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public int VendorId { get; set; }
    public int ProductId { get; set; }
    public bool IsConnected { get; set; }
    public DateTime ConnectedTime { get; set; }
    public int BatteryLevel { get; set; } // -1 if unknown
    public string CurrentProfile { get; set; } = "Default";
    
    // XInput
    public int? UserIndex { get; set; }
    
    // DInput/HID
    public Guid DeviceGuid { get; set; }
    
    public string DisplayName => $"{Name} ({InputType})";
    
    public string ConnectionStatus => IsConnected ? "Connected" : "Disconnected";
}
