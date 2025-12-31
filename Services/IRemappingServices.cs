using ControllerManager.Models;

namespace ControllerManager.Services;

public interface IVirtualOutputService
{
    bool CreateVirtualController(string sourceDeviceId, ControllerType targetType);
    void DestroyVirtualController(string sourceDeviceId);
    void UpdateVirtualController(string sourceDeviceId, ButtonState buttonState);
    void SetVibration(string sourceDeviceId, byte leftMotor, byte rightMotor);
    bool IsAvailable { get; }
}

public interface IInputPollingService
{
    event EventHandler<InputStateEventArgs>? InputStateChanged;
    void StartPolling(ControllerDevice device);
    void StopPolling(string deviceId);
    ButtonState? GetCurrentState(string deviceId);
}

public interface IMappingEngine
{
    void LoadProfile(GameProfile profile);
    void ClearProfile();
    ButtonState TransformInput(ButtonState input, string deviceId);
    GameProfile? CurrentProfile { get; }
}

public interface IRemappingService
{
    Task StartRemappingAsync(ControllerDevice device, GameProfile? profile = null);
    Task StopRemappingAsync(string deviceId);
    bool IsRemapping(string deviceId);
    void SetProfile(string deviceId, GameProfile profile);
}

public class InputStateEventArgs : EventArgs
{
    public string DeviceId { get; set; } = string.Empty;
    public ButtonState State { get; set; } = new();
}

public class ButtonState
{
    public bool A { get; set; }
    public bool B { get; set; }
    public bool X { get; set; }
    public bool Y { get; set; }
    public bool DPadUp { get; set; }
    public bool DPadDown { get; set; }
    public bool DPadLeft { get; set; }
    public bool DPadRight { get; set; }
    public bool LeftBumper { get; set; }
    public bool RightBumper { get; set; }
    public byte LeftTrigger { get; set; }
    public byte RightTrigger { get; set; }
    public bool LeftThumb { get; set; }
    public bool RightThumb { get; set; }
    public bool Start { get; set; }
    public bool Back { get; set; }
    public bool Guide { get; set; }
    public short LeftThumbX { get; set; }
    public short LeftThumbY { get; set; }
    public short RightThumbX { get; set; }
    public short RightThumbY { get; set; }
    public ButtonState Clone()
    {
        return (ButtonState)MemberwiseClone();
    }
}
