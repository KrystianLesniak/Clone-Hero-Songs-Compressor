using SongsCompressor.Common.Models;

namespace SongsCompressor.Common.Interfaces.Services
{
    public interface ISettingsStorage
    {
        Task<UserSettings> GetSettings();
        Task SaveSettings(UserSettings settings);
    }
}
