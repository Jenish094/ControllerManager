using ControllerManager.Models;
using HidSharp;

namespace ControllerManager.Services;

public class HidService : IHidService
{
    //Known ProductIDs/VendorIds
    private readonly Dictionary<int, string> _knownVendors = new()
    {
        { 0x054C, "Sony" },           //PS
        { 0x045E, "Microsoft" },      //XB
        { 0x057E, "Nintendo" },       //Switch
    };

    private readonly Dictionary<(int vendorId, int productId), ControllerType> _knownControllers = new()
    {
        //Playstration
        { (0x054C, 0x0268), ControllerType.PS3 },
        { (0x054C, 0x05C4), ControllerType.PS4 },
        { (0x054C, 0x09CC), ControllerType.PS4 },
        { (0x054C, 0x0CE6), ControllerType.PS5 },
        
        //Xbox
        { (0x045E, 0x028E), ControllerType.Xbox360 },
        { (0x045E, 0x02D1), ControllerType.XboxOne },
        { (0x045E, 0x02DD), ControllerType.XboxOne },
        { (0x045E, 0x0B13), ControllerType.XboxSeries },
        
        // Nintendo
        { (0x057E, 0x2009), ControllerType.Switch }
    };

    public List<ControllerDevice> DetectDevices()
    {
        var devices = new List<ControllerDevice>();

        try
        {
            var hidDevices = DeviceList.Local.GetHidDevices();

            foreach (var hidDevice in hidDevices)
            {
                // Filter for game controllers
                if (hidDevice.GetMaxInputReportLength() > 0 && IsGameController(hidDevice))
                {
                    try
                    {
                        var device = CreateControllerDevice(hidDevice);
                        if (device != null)
                        {
                            devices.Add(device);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing HID device: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HID enumeration error: {ex.Message}");
        }

        return devices;
    }

    private bool IsGameController(HidDevice hidDevice)
    {
        try
        {
            var reports = hidDevice.GetReportDescriptor();
            if (_knownVendors.ContainsKey(hidDevice.VendorID))
                return true;
            if (hidDevice.GetMaxInputReportLength() >= 8)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    private ControllerDevice? CreateControllerDevice(HidDevice hidDevice)
    {
        var vendorId = hidDevice.VendorID;
        var productId = hidDevice.ProductID;
        
        //Skip if XInput
        if (vendorId == 0x045E && IsXInputDevice(productId))
            return null;

        var controllerType = _knownControllers.TryGetValue((vendorId, productId), out var type)
            ? type
            : ControllerType.Generic;

        string name;
        try
        {
            name = hidDevice.GetProductName() ?? "Unknown HID Device";
        }
        catch
        {
            name = $"HID Device {vendorId:X4}:{productId:X4}";
        }

        var device = new ControllerDevice
        {
            InstanceId = $"HID_{hidDevice.DevicePath}",
            Name = name,
            Type = controllerType,
            InputType = InputType.HID,
            ConnectionType = DetermineConnectionType(hidDevice),
            VendorId = vendorId,
            ProductId = productId,
            IsConnected = true,
            ConnectedTime = DateTime.Now,
            BatteryLevel = -1
        };

        return device;
    }

    private bool IsXInputDevice(int productId)
    {
        var xInputProductIds = new[] { 0x028E, 0x02D1, 0x02DD, 0x02E3, 0x02EA, 0x0B13 };
        return xInputProductIds.Contains(productId);
    }

    private ConnectionType DetermineConnectionType(HidDevice hidDevice)
    {
        try
        {
            var path = hidDevice.DevicePath.ToLower();
            
            if (path.Contains("bluetooth") || path.Contains("bth"))
                return ConnectionType.Bluetooth;
            if (path.Contains("usb"))
                return ConnectionType.USB;

            return ConnectionType.Unknown;
        }
        catch
        {
            return ConnectionType.Unknown;
        }
    }
}
