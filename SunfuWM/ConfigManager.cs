using System;
using System.IO;
using System.Text.Json;

namespace SunfuWM
{
    public static class ConfigManager
    {
        private const string ConfigFileName = "sunfuwm.json";

        public static AppConfig Load()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            
            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                catch
                {
                    // Return default if error
                    return new AppConfig();
                }
            }
            else
            {
                // Create default if missing
                var config = new AppConfig();
                Save(config); // Save it so user knows it exists
                return config;
            }
        }

        public static void Save(AppConfig config)
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configPath, json);
            }
            catch { /* Ignore save errors */ }
        }
    }
}
