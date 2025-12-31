using System.Collections.Concurrent;
using ControllerManager.Models;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace ControllerManager.Services;

public class VirtualOutputService : IVirtualOutputService, IDisposable
{
    private readonly ConcurrentDictionary<string, IXbox360Controller> _virtualControllers = new();
    private ViGEmClient? _client;
    private bool _isAvailable;

    public bool IsAvailable => _isAvailable;

    public VirtualOutputService()
    {
        try
        {
            _client = new ViGEmClient();
            _isAvailable = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ViGEm initialization failed: {ex.Message}");
            _isAvailable = false;
        }
    }

    public bool CreateVirtualController(string sourceDeviceId, ControllerType targetType)
    {
        if (!_isAvailable || _client == null)
            return false;

        try
        {
            if (_virtualControllers.ContainsKey(sourceDeviceId))
                return true;
            var controller = _client.CreateXbox360Controller();
            controller.Connect();
            
            _virtualControllers[sourceDeviceId] = controller;
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to create virtual controller: {ex.Message}");
            return false;
        }
    }

    public void DestroyVirtualController(string sourceDeviceId)
    {
        if (_virtualControllers.TryRemove(sourceDeviceId, out var controller))
        {
            try
            {
                controller.Disconnect();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error destroying virtual controller: {ex.Message}");
            }
        }
    }

    public void UpdateVirtualController(string sourceDeviceId, ButtonState buttonState)
    {
        if (!_virtualControllers.TryGetValue(sourceDeviceId, out var controller))
            return;

        try
        {
            controller.SetButtonState(Xbox360Button.A, buttonState.A);
            controller.SetButtonState(Xbox360Button.B, buttonState.B);
            controller.SetButtonState(Xbox360Button.X, buttonState.X);
            controller.SetButtonState(Xbox360Button.Y, buttonState.Y);
            
            controller.SetButtonState(Xbox360Button.Up, buttonState.DPadUp);
            controller.SetButtonState(Xbox360Button.Down, buttonState.DPadDown);
            controller.SetButtonState(Xbox360Button.Left, buttonState.DPadLeft);
            controller.SetButtonState(Xbox360Button.Right, buttonState.DPadRight);
            
            controller.SetButtonState(Xbox360Button.LeftShoulder, buttonState.LeftBumper);
            controller.SetButtonState(Xbox360Button.RightShoulder, buttonState.RightBumper);
            
            controller.SetButtonState(Xbox360Button.LeftThumb, buttonState.LeftThumb);
            controller.SetButtonState(Xbox360Button.RightThumb, buttonState.RightThumb);
            
            controller.SetButtonState(Xbox360Button.Start, buttonState.Start);
            controller.SetButtonState(Xbox360Button.Back, buttonState.Back);
            controller.SetButtonState(Xbox360Button.Guide, buttonState.Guide);
            
            controller.SetSliderValue(Xbox360Slider.LeftTrigger, buttonState.LeftTrigger);
            controller.SetSliderValue(Xbox360Slider.RightTrigger, buttonState.RightTrigger);
            
            controller.SetAxisValue(Xbox360Axis.LeftThumbX, buttonState.LeftThumbX);
            controller.SetAxisValue(Xbox360Axis.LeftThumbY, buttonState.LeftThumbY);
            controller.SetAxisValue(Xbox360Axis.RightThumbX, buttonState.RightThumbX);
            controller.SetAxisValue(Xbox360Axis.RightThumbY, buttonState.RightThumbY);
            
            controller.SubmitReport();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating virtual controller: {ex.Message}");
        }
    }

    public void SetVibration(string sourceDeviceId, byte leftMotor, byte rightMotor)
    {
        if (_virtualControllers.TryGetValue(sourceDeviceId, out var controller))
        {
            
        }
    }

    public void Dispose()
    {
        foreach (var kvp in _virtualControllers)
        {
            try
            {
                kvp.Value.Disconnect();
            }
            catch { }
        }
        _virtualControllers.Clear();
        _client?.Dispose();
    }
}
