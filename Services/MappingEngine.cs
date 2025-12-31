using ControllerManager.Models;

namespace ControllerManager.Services;

public class MappingEngine : IMappingEngine
{
    private GameProfile? _currentProfile;
    private readonly Dictionary<string, Func<ButtonState, ButtonState, ButtonState>> _buttonMappers = new();

    public GameProfile? CurrentProfile => _currentProfile;

    public void LoadProfile(GameProfile profile)
    {
        _currentProfile = profile;
        _buttonMappers.Clear();
        foreach (var mapping in profile.ButtonMappings.Values)
        {
            if (!mapping.IsEnabled)
                continue;

            var mapper = CreateMapper(mapping);
            if (mapper != null)
            {
                _buttonMappers[mapping.SourceButton] = mapper;
            }
        }
    }

    public void ClearProfile()
    {
        _currentProfile = null;
        _buttonMappers.Clear();
    }

    public ButtonState TransformInput(ButtonState input, string deviceId)
    {
        if (_currentProfile == null || _buttonMappers.Count == 0)
        {
            return input.Clone();
        }

        var output = new ButtonState();
        foreach (var kvp in _buttonMappers)
        {
            output = kvp.Value(input, output);
        }

        if (!_buttonMappers.ContainsKey("LeftThumbX"))
            output.LeftThumbX = input.LeftThumbX;
        if (!_buttonMappers.ContainsKey("LeftThumbY"))
            output.LeftThumbY = input.LeftThumbY;
        if (!_buttonMappers.ContainsKey("RightThumbX"))
            output.RightThumbX = input.RightThumbX;
        if (!_buttonMappers.ContainsKey("RightThumbY"))
            output.RightThumbY = input.RightThumbY;
        if (!_buttonMappers.ContainsKey("LeftTrigger"))
            output.LeftTrigger = input.LeftTrigger;
        if (!_buttonMappers.ContainsKey("RightTrigger"))
            output.RightTrigger = input.RightTrigger;

        return output;
    }

    private Func<ButtonState, ButtonState, ButtonState>? CreateMapper(ButtonMapping mapping)
    {
        return (input, output) =>
        {
            var sourceValue = GetButtonValue(input, mapping.SourceButton);
            SetButtonValue(output, mapping.TargetButton, sourceValue);
            return output;
        };
    }

    private object? GetButtonValue(ButtonState state, string buttonName)
    {
        return buttonName switch
        {
            "A" => state.A,
            "B" => state.B,
            "X" => state.X,
            "Y" => state.Y,
            "DPadUp" => state.DPadUp,
            "DPadDown" => state.DPadDown,
            "DPadLeft" => state.DPadLeft,
            "DPadRight" => state.DPadRight,
            "LeftBumper" => state.LeftBumper,
            "RightBumper" => state.RightBumper,
            "LeftThumb" => state.LeftThumb,
            "RightThumb" => state.RightThumb,
            "Start" => state.Start,
            "Back" => state.Back,
            "Guide" => state.Guide,
            "LeftTrigger" => state.LeftTrigger,
            "RightTrigger" => state.RightTrigger,
            "LeftThumbX" => state.LeftThumbX,
            "LeftThumbY" => state.LeftThumbY,
            "RightThumbX" => state.RightThumbX,
            "RightThumbY" => state.RightThumbY,
            _ => null
        };
    }

    private void SetButtonValue(ButtonState state, string buttonName, object? value)
    {
        if (value == null) return;

        switch (buttonName)
        {
            case "A": state.A = (bool)value; break;
            case "B": state.B = (bool)value; break;
            case "X": state.X = (bool)value; break;
            case "Y": state.Y = (bool)value; break;
            case "DPadUp": state.DPadUp = (bool)value; break;
            case "DPadDown": state.DPadDown = (bool)value; break;
            case "DPadLeft": state.DPadLeft = (bool)value; break;
            case "DPadRight": state.DPadRight = (bool)value; break;
            case "LeftBumper": state.LeftBumper = (bool)value; break;
            case "RightBumper": state.RightBumper = (bool)value; break;
            case "LeftThumb": state.LeftThumb = (bool)value; break;
            case "RightThumb": state.RightThumb = (bool)value; break;
            case "Start": state.Start = (bool)value; break;
            case "Back": state.Back = (bool)value; break;
            case "Guide": state.Guide = (bool)value; break;
            case "LeftTrigger": state.LeftTrigger = Convert.ToByte(value); break;
            case "RightTrigger": state.RightTrigger = Convert.ToByte(value); break;
            case "LeftThumbX": state.LeftThumbX = Convert.ToInt16(value); break;
            case "LeftThumbY": state.LeftThumbY = Convert.ToInt16(value); break;
            case "RightThumbX": state.RightThumbX = Convert.ToInt16(value); break;
            case "RightThumbY": state.RightThumbY = Convert.ToInt16(value); break;
        }
    }
}
