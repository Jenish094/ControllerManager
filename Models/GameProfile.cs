namespace ControllerManager.Models;

public class GameProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public Dictionary<string, ButtonMapping> ButtonMappings { get; set; } = new();
    public bool AdaptiveTriggersEnabled { get; set; }
    public int VibrationIntensity { get; set; } = 100;
    public string LedColor { get; set; } = "#0078D4"; //Controller Colour
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}

public class ButtonMapping
{
    public string SourceButton { get; set; } = string.Empty;
    public string TargetButton { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public enum ControllerButton
{
    A, B, X, Y,
    Cross, Circle, Square, Triangle, // Playstation
    DPadUp, DPadDown, DPadLeft, DPadRight,
    LeftBumper, RightBumper,
    LeftTrigger, RightTrigger,
    LeftStick, RightStick,
    Start, Back, Guide,
    Share, Options, PS, //Playstation
    Menu, View, Xbox, //Xbox
    LeftStickX, LeftStickY,
    RightStickX, RightStickY
}
