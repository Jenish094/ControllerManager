using System.Collections.Concurrent;
using ControllerManager.Models;

namespace ControllerManager.Services;

public class RemappingService : IRemappingService, IDisposable
{
    private readonly IInputPollingService _inputPolling;
    private readonly IMappingEngine _mappingEngine;
    private readonly IVirtualOutputService _virtualOutput;
    private readonly ConcurrentDictionary<string, RemappingContext> _activeRemappings = new();

    public RemappingService(
        IInputPollingService inputPolling,
        IMappingEngine mappingEngine,
        IVirtualOutputService virtualOutput)
    {
        _inputPolling = inputPolling;
        _mappingEngine = mappingEngine;
        _virtualOutput = virtualOutput;

        _inputPolling.InputStateChanged += OnInputStateChanged;
    }

    public Task StartRemappingAsync(ControllerDevice device, GameProfile? profile = null)
    {
        if (_activeRemappings.ContainsKey(device.InstanceId))
            return Task.CompletedTask;
        if (!_virtualOutput.CreateVirtualController(device.InstanceId, ControllerType.XboxOne))
        {
            System.Diagnostics.Debug.WriteLine($"Failed to create virtual controller for {device.InstanceId}");
            return Task.CompletedTask;
        }

        var context = new RemappingContext
        {
            Device = device,
            Profile = profile
        };

        _activeRemappings[device.InstanceId] = context;
        if (profile != null)
        {
            _mappingEngine.LoadProfile(profile);
        }
        _inputPolling.StartPolling(device);

        return Task.CompletedTask;
    }

    public Task StopRemappingAsync(string deviceId)
    {
        if (_activeRemappings.TryRemove(deviceId, out var context))
        {
            _inputPolling.StopPolling(deviceId);
            _virtualOutput.DestroyVirtualController(deviceId);
        }

        return Task.CompletedTask;
    }

    public bool IsRemapping(string deviceId)
    {
        return _activeRemappings.ContainsKey(deviceId);
    }

    public void SetProfile(string deviceId, GameProfile profile)
    {
        if (_activeRemappings.TryGetValue(deviceId, out var context))
        {
            context.Profile = profile;
            _mappingEngine.LoadProfile(profile);
        }
    }

    private void OnInputStateChanged(object? sender, InputStateEventArgs e)
    {
        if (!_activeRemappings.ContainsKey(e.DeviceId))
            return;
        var transformedState = _mappingEngine.TransformInput(e.State, e.DeviceId);
        _virtualOutput.UpdateVirtualController(e.DeviceId, transformedState);
    }

    public void Dispose()
    {
        foreach (var deviceId in _activeRemappings.Keys.ToList())
        {
            StopRemappingAsync(deviceId).Wait();
        }
    }

    private class RemappingContext
    {
        public ControllerDevice Device { get; set; } = null!;
        public GameProfile? Profile { get; set; }
    }
}
