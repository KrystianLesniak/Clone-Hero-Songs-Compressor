using LiteDB;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Models;

namespace SongsCompressor.Services.Services
{
    public class SettingsStorage : ISettingsStorage
    {
        private readonly string _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clone Hero Songs Compressor", "UserSettings.db");

        public SettingsStorage()
        {
            var path = Path.GetDirectoryName(_dbPath);
            Directory.CreateDirectory(path!);
        }

        public Task<UserSettings> GetSettings()
        {
            using var db = new LiteDatabase(_dbPath);
            var col = db.GetCollection<UserSettings>(typeof(UserSettings).Name);

            return Task.FromResult(col.FindOne(Query.All()) ?? new());
        }

        public Task SaveSettings(UserSettings settings)
        {
            using var db = new LiteDatabase(_dbPath);
            var col = db.GetCollection<UserSettings>(typeof(UserSettings).Name);

            col.DeleteAll();
            col.Insert(settings);
            return Task.CompletedTask;
        }

    }
}
