using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;
using ControllerManager.ViewModels;

namespace ControllerManager.Views;

public partial class ControllerPage : Page
{
    private Point _joystickStart;
    private Ellipse? _currentJoystick;
    private Dictionary<string, (FrameworkElement, Color)> _buttonMap = new();

    public ControllerPage(ControllerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _buttonMap = new()
        {
            { "LTrigger", (LTrigger, Color.FromArgb(255, 255, 165, 0)) },
            { "LBumper", (LBumper, Color.FromArgb(255, 136, 136, 136)) },
            { "RTrigger", (RTrigger, Color.FromArgb(255, 255, 165, 0)) },
            { "RBumper", (RBumper, Color.FromArgb(255, 136, 136, 136)) },
            { "MenuButton", (MenuButton, Color.FromArgb(255, 153, 153, 153)) },
            { "ViewButton", (ViewButton, Color.FromArgb(255, 153, 153, 153)) },
            { "ButtonA", (ButtonA, Color.FromArgb(255, 0, 255, 0)) },
            { "ButtonB", (ButtonB, Color.FromArgb(255, 255, 0, 0)) },
            { "ButtonX", (ButtonX, Color.FromArgb(255, 0, 204, 255)) },
            { "ButtonY", (ButtonY, Color.FromArgb(255, 255, 255, 0)) },
            { "DPadUp", (DPadUp, Color.FromArgb(255, 136, 136, 136)) },
            { "DPadDown", (DPadDown, Color.FromArgb(255, 136, 136, 136)) },
            { "DPadLeft", (DPadLeft, Color.FromArgb(255, 136, 136, 136)) },
            { "DPadRight", (DPadRight, Color.FromArgb(255, 136, 136, 136)) },
        };
        if (viewModel != null)
        {
            viewModel.PropertyChanged += (s, e) =>
            {
                var vm = s as ControllerViewModel;
                if (vm == null) return;

                switch (e.PropertyName)
                {
                    case nameof(ControllerViewModel.LT_Pressed):
                        UpdateButtonState("LTrigger", vm.LT_Pressed);
                        break;
                    case nameof(ControllerViewModel.LB_Pressed):
                        UpdateButtonState("LBumper", vm.LB_Pressed);
                        break;
                    case nameof(ControllerViewModel.RT_Pressed):
                        UpdateButtonState("RTrigger", vm.RT_Pressed);
                        break;
                    case nameof(ControllerViewModel.RB_Pressed):
                        UpdateButtonState("RBumper", vm.RB_Pressed);
                        break;
                    case nameof(ControllerViewModel.A_Pressed):
                        UpdateButtonState("ButtonA", vm.A_Pressed);
                        break;
                    case nameof(ControllerViewModel.B_Pressed):
                        UpdateButtonState("ButtonB", vm.B_Pressed);
                        break;
                    case nameof(ControllerViewModel.X_Pressed):
                        UpdateButtonState("ButtonX", vm.X_Pressed);
                        break;
                    case nameof(ControllerViewModel.Y_Pressed):
                        UpdateButtonState("ButtonY", vm.Y_Pressed);
                        break;
                    case nameof(ControllerViewModel.DPadUp_Pressed):
                        UpdateButtonState("DPadUp", vm.DPadUp_Pressed);
                        break;
                    case nameof(ControllerViewModel.DPadDown_Pressed):
                        UpdateButtonState("DPadDown", vm.DPadDown_Pressed);
                        break;
                    case nameof(ControllerViewModel.DPadLeft_Pressed):
                        UpdateButtonState("DPadLeft", vm.DPadLeft_Pressed);
                        break;
                    case nameof(ControllerViewModel.DPadRight_Pressed):
                        UpdateButtonState("DPadRight", vm.DPadRight_Pressed);
                        break;
                    case nameof(ControllerViewModel.Menu_Pressed):
                        UpdateButtonState("MenuButton", vm.Menu_Pressed);
                        break;
                    case nameof(ControllerViewModel.View_Pressed):
                        UpdateButtonState("ViewButton", vm.View_Pressed);
                        break;
                    case nameof(ControllerViewModel.LeftStickX):
                    case nameof(ControllerViewModel.LeftStickY):
                        var leftJoystick = FindName("LeftJoystickInner") as Ellipse;
                        if (leftJoystick != null)
                            UpdateJoystickPosition(leftJoystick, vm.LeftStickX, vm.LeftStickY, vm.LeftThumb_Pressed);
                        break;
                    case nameof(ControllerViewModel.RightStickX):
                    case nameof(ControllerViewModel.RightStickY):
                        var rightJoystick = FindName("RightJoystickInner") as Ellipse;
                        if (rightJoystick != null)
                            UpdateJoystickPosition(rightJoystick, vm.RightStickX, vm.RightStickY, vm.RightThumb_Pressed);
                        break;
                    case nameof(ControllerViewModel.LeftThumb_Pressed):
                        var leftStick = FindName("LeftJoystickInner") as Ellipse;
                        if (leftStick != null)
                            UpdateJoystickPosition(leftStick, vm.LeftStickX, vm.LeftStickY, vm.LeftThumb_Pressed);
                        break;
                    case nameof(ControllerViewModel.RightThumb_Pressed):
                        var rightStick = FindName("RightJoystickInner") as Ellipse;
                        if (rightStick != null)
                            UpdateJoystickPosition(rightStick, vm.RightStickX, vm.RightStickY, vm.RightThumb_Pressed);
                        break;
                }
            };
        }
    }

    private void UpdateButtonState(string buttonName, bool isPressed)
    {
        if (!_buttonMap.TryGetValue(buttonName, out var entry))
            return;

        var element = entry.Item1;
        var originalColor = entry.Item2;

        if (isPressed)
        {
            if (element is TextBlock textBlock)
            {
                textBlock.Background = new SolidColorBrush(originalColor);
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (element is Ellipse ellipse)
            {
                ellipse.Fill = new SolidColorBrush(originalColor);
            }
            else if (element is Polygon polygon)
            {
                polygon.Fill = new SolidColorBrush(originalColor);
            }
        }
        else
        {
            if (element is TextBlock textBlock)
            {
                textBlock.Background = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68));
                textBlock.Foreground = new SolidColorBrush(originalColor);
            }
            else if (element is Ellipse ellipse)
            {
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68));
            }
            else if (element is Polygon polygon)
            {
                polygon.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68));
            }
        }
    }

    private void UpdateJoystickPosition(Ellipse joystick, double x, double y, bool isPressed)
    {
        var transform = new TranslateTransform(x, y);
        joystick.RenderTransform = transform;
        if (isPressed)
        {
            if (joystick.Name == "LeftJoystickInner")
            {
                joystick.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255)); // Brighter blue
            }
            else if (joystick.Name == "RightJoystickInner")
            {
                joystick.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 153, 0)); // Brighter orange
            }
        }
        else
        {
            if (joystick.Name == "LeftJoystickInner")
            {
                joystick.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 102, 255)); // #0066FF
            }
            else if (joystick.Name == "RightJoystickInner")
            {
                joystick.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 102, 0)); // #FF6600
            }
        }
    }

    private void OnControlMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnControlMouseUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnJoystickMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Ellipse joystick)
        {
            _currentJoystick = joystick;
            _joystickStart = e.GetPosition(joystick.Parent as Grid);
            joystick.CaptureMouse();
            e.Handled = true;
        }
    }

    private void OnJoystickMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_currentJoystick != null)
        {
            _currentJoystick.ReleaseMouseCapture();

            Canvas.SetLeft(_currentJoystick, double.NaN);
            Canvas.SetTop(_currentJoystick, double.NaN);
            
            _currentJoystick = null;
            e.Handled = true;
        }
    }
}
