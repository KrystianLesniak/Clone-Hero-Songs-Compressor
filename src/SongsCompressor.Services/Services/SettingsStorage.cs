using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Models;
using System.Text.Json;

namespace SongsCompressor.Services.Services
{
    public class SettingsStorage : ISettingsStorage
    {
        //private readonly string _jsonPath = Path.Combine(FileSystem.Current.AppDataDirectory, "UserSettings.json");
        private readonly string _jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clone Hero Songs Compressor", "UserSettings.json");

        public SettingsStorage()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath) ?? string.Empty);
        }

        public async Task<UserSettings> GetSettings()
        {
            if (!File.Exists(_jsonPath))
            {
                return new();
            }

            return new UserSettings();
        }

        public Task SaveSettings(UserSettings settings)
        {
            return Task.CompletedTask;
        }

    }
}
