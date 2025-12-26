using System.IO;
using ControllerManager.Models;
using Newtonsoft.Json;

namespace ControllerManager.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly string _settingsPath;
    private readonly string _profilesPath;

    public ConfigurationService()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ControllerManager");

        Directory.CreateDirectory(appDataPath);

        _settingsPath = Path.Combine(appDataPath, "settings.json");
        _profilesPath = Path.Combine(appDataPath, "profiles.json");
    }

    public AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }

        return new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public List<GameProfile> LoadProfiles()
    {
        try
        {
            if (File.Exists(_profilesPath))
            {
                var json = File.ReadAllText(_profilesPath);
                return JsonConvert.DeserializeObject<List<GameProfile>>(json) ?? new List<GameProfile>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading profiles: {ex.Message}");
        }

        return new List<GameProfile>();
    }

    public void SaveProfile(GameProfile profile)
    {
        try
        {
            var profiles = LoadProfiles();
            var existing = profiles.FirstOrDefault(p => p.Id == profile.Id);

            if (existing != null)
            {
                profiles.Remove(existing);
            }

            profile.ModifiedDate = DateTime.Now;
            profiles.Add(profile);

            var json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            File.WriteAllText(_profilesPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving profile: {ex.Message}");
        }
    }

    public void DeleteProfile(string profileId)
    {
        try
        {
            var profiles = LoadProfiles();
            profiles.RemoveAll(p => p.Id == profileId);

            var json = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            File.WriteAllText(_profilesPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting profile: {ex.Message}");
        }
    }
}
