using System.Text.Json;
using LicenseKeyGenerator.Maui.Models;

namespace LicenseKeyGenerator.Maui.Services;

public static class ConfigurationService
{
    private static readonly string ConfigFileName = "salt_config.json";

    public static List<SaltItem> LoadSaltList()
    {
        try
        {
            string configPath = Path.Combine(FileSystem.AppDataDirectory, ConfigFileName);
            
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var list = JsonSerializer.Deserialize<List<SaltItem>>(json);
                return list ?? new List<SaltItem>();
            }
            
            return new List<SaltItem>();
        }
        catch
        {
            return new List<SaltItem>();
        }
    }

    public static void SaveSaltList(List<SaltItem> saltList)
    {
        try
        {
            string configPath = Path.Combine(FileSystem.AppDataDirectory, ConfigFileName);
            string json = JsonSerializer.Serialize(saltList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }
        catch
        {
        }
    }
}
