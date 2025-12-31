using ControllerManager.Models;
using SharpDX.XInput;

namespace ControllerManager.Services;

public interface IVibrationService
{
    void SetVibration(ControllerDevice device, byte leftMotor, byte rightMotor);
    void TestVibration(ControllerDevice device);
    void StopVibration(ControllerDevice device);
}

public class VibrationService : IVibrationService
{
    public void SetVibration(ControllerDevice device, byte leftMotor, byte rightMotor)
    {
        if (device.InputType == Models.InputType.XInput && device.UserIndex.HasValue)
        {
            try
            {
                var controller = new Controller((UserIndex)device.UserIndex.Value);
                if (controller.IsConnected)
                {
                    var vibration = new Vibration
                    {
                        LeftMotorSpeed = (ushort)(leftMotor * 257), // Scale 0-255 to 0-65535
                        RightMotorSpeed = (ushort)(rightMotor * 257)
                    };
                    controller.SetVibration(vibration);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
            }
        }
    }

    public void TestVibration(ControllerDevice device)
    {
        Task.Run(async () =>
        {
            SetVibration(device, 200, 200);
            await Task.Delay(500);
            StopVibration(device);
        });
    }

    public void StopVibration(ControllerDevice device)
    {
        SetVibration(device, 0, 0);
    }
}
