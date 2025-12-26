namespace ControllerManager.Models;

public class AppSettings
{
    public bool StartWithWindows { get; set; }
    public bool MinimizeToTray { get; set; }
    public bool ShowNotifications { get; set; }
    public bool AutoDetectControllers { get; set; } = true;
    public bool EnableAdaptiveTriggers { get; set; } = true;
    public int DefaultVibrationIntensity { get; set; } = 100;
    public string DefaultLedColor { get; set; } = "#0078D4";
    public string ThemeMode { get; set; } = "Dark";
}
