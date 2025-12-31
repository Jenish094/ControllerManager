using ControllerManager.Models;
using HidSharp;

namespace ControllerManager.Services;

public interface ILedControlService
{
    void SetLedColor(ControllerDevice device, byte r, byte g, byte b);
    bool SupportsLedControl(ControllerDevice device);
}

public class LedControlService : ILedControlService
{
    public bool SupportsLedControl(ControllerDevice device)
    {
        return device.Type is ControllerType.PS4 or ControllerType.PS5;
    }

    public void SetLedColor(ControllerDevice device, byte r, byte g, byte b)
    {
        if (!SupportsLedControl(device))
            return;

        try
        {
            switch (device.Type)
            {
                case ControllerType.PS4:
                    SetPS4LedColor(device, r, g, b);
                    break;
                case ControllerType.PS5:
                    SetPS5LedColor(device, r, g, b);
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LED control error: {ex.Message}");
        }
    }

    private void SetPS4LedColor(ControllerDevice device, byte r, byte g, byte b)
    {
        try
        {
            var hidDevices = DeviceList.Local.GetHidDevices(device.VendorId, device.ProductId);
            var hidDevice = hidDevices.FirstOrDefault();

            if (hidDevice == null || !hidDevice.TryOpen(out var stream))
                return;

            using (stream)
            {
                var report = new byte[32];
                report[0] = 0x05;
                report[1] = 0xFF;
                report[4] = 0x00;
                report[5] = 0x00;
                report[6] = r;
                report[7] = g;
                report[8] = b;
                
                stream.Write(report, 0, report.Length);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PS4 LED error: {ex.Message}");
        }
    }

    private void SetPS5LedColor(ControllerDevice device, byte r, byte g, byte b)
    {
        try
        {
            var hidDevices = DeviceList.Local.GetHidDevices(device.VendorId, device.ProductId);
            var hidDevice = hidDevices.FirstOrDefault();

            if (hidDevice == null || !hidDevice.TryOpen(out var stream))
                return;

            using (stream)
            {
                var report = new byte[48];
                report[0] = 0x02;
                report[1] = 0xFF;
                report[2] = 0x07;
                report[45] = r;
                report[46] = g;
                report[47] = b;
                
                stream.Write(report, 0, report.Length);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PS5 LED error: {ex.Message}");
        }
    }
}
