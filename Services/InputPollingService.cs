using System.Collections.Concurrent;
using ControllerManager.Models;
using SharpDX.XInput;

namespace ControllerManager.Services;

public class InputPollingService : IInputPollingService, IDisposable
{
    private readonly ConcurrentDictionary<string, PollingContext> _pollingContexts = new();
    private readonly IXInputService _xInputService;
    
    public event EventHandler<InputStateEventArgs>? InputStateChanged;

    public InputPollingService(IXInputService xInputService)
    {
        _xInputService = xInputService;
    }

    public void StartPolling(ControllerDevice device)
    {
        if (_pollingContexts.ContainsKey(device.InstanceId))
            return;

        var context = new PollingContext
        {
            Device = device,
            CancellationTokenSource = new CancellationTokenSource()
        };

        _pollingContexts[device.InstanceId] = context;
        Task.Run(() => PollLoop(context), context.CancellationTokenSource.Token);
    }

    public void StopPolling(string deviceId)
    {
        if (_pollingContexts.TryRemove(deviceId, out var context))
        {
            context.CancellationTokenSource.Cancel();
            context.CancellationTokenSource.Dispose();
        }
    }

    public ButtonState? GetCurrentState(string deviceId)
    {
        if (_pollingContexts.TryGetValue(deviceId, out var context))
        {
            return context.LastState?.Clone();
        }
        return null;
    }

    private async Task PollLoop(PollingContext context)
    {
        var token = context.CancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            try
            {
                ButtonState? state = context.Device.InputType switch
                {
                    Models.InputType.XInput => ReadXInputState(context.Device),
                    Models.InputType.DirectInput => ReadDirectInputState(context.Device),
                    Models.InputType.HID => ReadHidState(context.Device),
                    _ => null
                };

                if (state != null)
                {
                    context.LastState = state;
                    InputStateChanged?.Invoke(this, new InputStateEventArgs 
                    { 
                        DeviceId = context.Device.InstanceId, 
                        State = state 
                    });
                }

                await Task.Delay(8, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Polling error for {context.Device.InstanceId}: {ex.Message}");
                await Task.Delay(100, token);
            }
        }
    }

    private ButtonState? ReadXInputState(ControllerDevice device)
    {
        if (!device.UserIndex.HasValue)
            return null;

        try
        {
            var controller = new Controller((UserIndex)device.UserIndex.Value);
            if (!controller.IsConnected)
                return null;

            var xinputState = controller.GetState();
            var gamepad = xinputState.Gamepad;

            return new ButtonState
            {
                A = gamepad.Buttons.HasFlag(GamepadButtonFlags.A),
                B = gamepad.Buttons.HasFlag(GamepadButtonFlags.B),
                X = gamepad.Buttons.HasFlag(GamepadButtonFlags.X),
                Y = gamepad.Buttons.HasFlag(GamepadButtonFlags.Y),
                DPadUp = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp),
                DPadDown = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown),
                DPadLeft = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft),
                DPadRight = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight),
                LeftBumper = gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder),
                RightBumper = gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder),
                LeftThumb = gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb),
                RightThumb = gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb),
                Start = gamepad.Buttons.HasFlag(GamepadButtonFlags.Start),
                Back = gamepad.Buttons.HasFlag(GamepadButtonFlags.Back),
                LeftTrigger = gamepad.LeftTrigger,
                RightTrigger = gamepad.RightTrigger,
                LeftThumbX = gamepad.LeftThumbX,
                LeftThumbY = gamepad.LeftThumbY,
                RightThumbX = gamepad.RightThumbX,
                RightThumbY = gamepad.RightThumbY
            };
        }
        catch
        {
            return null;
        }
    }

    private ButtonState? ReadDirectInputState(ControllerDevice device)
    {
        return new ButtonState();
    }

    private ButtonState? ReadHidState(ControllerDevice device)
    {
        return new ButtonState();
    }

    public void Dispose()
    {
        foreach (var context in _pollingContexts.Values)
        {
            context.CancellationTokenSource.Cancel();
            context.CancellationTokenSource.Dispose();
        }
        _pollingContexts.Clear();
    }

    private class PollingContext
    {
        public ControllerDevice Device { get; set; } = null!;
        public CancellationTokenSource CancellationTokenSource { get; set; } = null!;
        public ButtonState? LastState { get; set; }
    }
}
