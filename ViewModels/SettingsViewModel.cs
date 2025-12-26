using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IConfigurationService _configService;
    private AppSettings _settings;

    public bool StartWithWindows
    {
        get => _settings.StartWithWindows;
        set
        {
            if (_settings.StartWithWindows != value)
            {
                _settings.StartWithWindows = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public bool MinimizeToTray
    {
        get => _settings.MinimizeToTray;
        set
        {
            if (_settings.MinimizeToTray != value)
            {
                _settings.MinimizeToTray = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public bool ShowNotifications
    {
        get => _settings.ShowNotifications;
        set
        {
            if (_settings.ShowNotifications != value)
            {
                _settings.ShowNotifications = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public bool AutoDetectControllers
    {
        get => _settings.AutoDetectControllers;
        set
        {
            if (_settings.AutoDetectControllers != value)
            {
                _settings.AutoDetectControllers = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public bool EnableAdaptiveTriggers
    {
        get => _settings.EnableAdaptiveTriggers;
        set
        {
            if (_settings.EnableAdaptiveTriggers != value)
            {
                _settings.EnableAdaptiveTriggers = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public int DefaultVibrationIntensity
    {
        get => _settings.DefaultVibrationIntensity;
        set
        {
            if (_settings.DefaultVibrationIntensity != value)
            {
                _settings.DefaultVibrationIntensity = value;
                OnPropertyChanged();
                SaveSettings();
            }
        }
    }

    public ICommand ResetSettingsCommand { get; }

    public SettingsViewModel(IConfigurationService configService)
    {
        _configService = configService;
        _settings = _configService.LoadSettings();

        ResetSettingsCommand = new RelayCommand(ResetSettings);
    }

    private void SaveSettings()
    {
        _configService.SaveSettings(_settings);
    }

    private void ResetSettings()
    {
        _settings = new AppSettings();
        _configService.SaveSettings(_settings);

        OnPropertyChanged(nameof(StartWithWindows));
        OnPropertyChanged(nameof(MinimizeToTray));
        OnPropertyChanged(nameof(ShowNotifications));
        OnPropertyChanged(nameof(AutoDetectControllers));
        OnPropertyChanged(nameof(EnableAdaptiveTriggers));
        OnPropertyChanged(nameof(DefaultVibrationIntensity));
    }
}
