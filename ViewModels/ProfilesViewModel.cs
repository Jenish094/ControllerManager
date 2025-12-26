using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControllerManager.Models;
using ControllerManager.Services;

namespace ControllerManager.ViewModels;

public class ProfilesViewModel : ViewModelBase
{
    private readonly IConfigurationService _configService;
    private GameProfile? _selectedProfile;
    private string _newProfileName = string.Empty;

    public ObservableCollection<GameProfile> Profiles { get; }

    public GameProfile? SelectedProfile
    {
        get => _selectedProfile;
        set => SetProperty(ref _selectedProfile, value);
    }

    public string NewProfileName
    {
        get => _newProfileName;
        set => SetProperty(ref _newProfileName, value);
    }

    public ICommand AddProfileCommand { get; }
    public ICommand DeleteProfileCommand { get; }
    public ICommand EditProfileCommand { get; }

    public ProfilesViewModel(IConfigurationService configService)
    {
        _configService = configService;
        Profiles = new ObservableCollection<GameProfile>();

        AddProfileCommand = new RelayCommand(AddProfile, CanAddProfile);
        DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile != null);
        EditProfileCommand = new RelayCommand(EditProfile, () => SelectedProfile != null);

        LoadProfiles();
    }

    private void LoadProfiles()
    {
        Profiles.Clear();
        var profiles = _configService.LoadProfiles();
        foreach (var profile in profiles)
        {
            Profiles.Add(profile);
        }
    }

    private bool CanAddProfile() => !string.IsNullOrWhiteSpace(NewProfileName);

    private void AddProfile()
    {
        var profile = new GameProfile
        {
            Name = NewProfileName,
            GameName = NewProfileName
        };

        _configService.SaveProfile(profile);
        Profiles.Add(profile);
        NewProfileName = string.Empty;
    }

    private void DeleteProfile()
    {
        if (SelectedProfile != null)
        {
            _configService.DeleteProfile(SelectedProfile.Id);
            Profiles.Remove(SelectedProfile);
            SelectedProfile = null;
        }
    }

    private void EditProfile()
    {
        // TODO: Profile Editor
        System.Diagnostics.Debug.WriteLine($"Edit profile: {SelectedProfile?.Name}");
    }
}
