using Engine.AudioToOpusConverter;
using Engine.FFmpegProvider;
using Engine.ImagePngToJpgConverter;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Models;
using SongsCompressor.Common.Services;

namespace SongCompressor.MainManager
{
    public class CompressionManager : ICompressionManager
    {
        public IList<ICompressionEngine> Engines { get; set; } = new List<ICompressionEngine>();

        //Progress status
        private ICompressionEngine? currentlyRunningEngine;
        private readonly OverallProgressStatus progress = new();

        private readonly ISettingsStorage settingsStorage;

        public CompressionManager(ISettingsStorage settingsStorage)
        {
            this.settingsStorage = settingsStorage;
        }

        public async Task Initialize(UserSettings settings)
        {
            ValidateSettings(settings);
            await settingsStorage.SaveSettings(settings);

            InitializeEngines(settings.Directories, settings.Options);
        }



        public async Task Start()
        {
            //TODO: Add exception handling & logging
            foreach (var engine in Engines.OrderBy(x => x.ExecutionOrder))
            {
                currentlyRunningEngine = engine;
                await engine.Start();
                await engine.Complete();

                progress.EnginesFinished = Engines.Count(x => x.Completed);
            }
        }

        public async Task<OverallProgressStatus> GetCurrentProgressStatus()
        {
            var engineProgress = currentlyRunningEngine is not null ? await currentlyRunningEngine.GetCurrentProgress() : new EngineProgressStatus();
            progress.UpdateProgress(engineProgress);

            return progress;
        }

        private static void ValidateSettings(UserSettings settings)
        {
            if (settings.Directories.Count == 0)
                throw new ArgumentException("Directories list is empty");

            if (settings.Options.Count == 0)
                throw new ArgumentException("Options list is empty");

            foreach (var directory in settings.Directories)
            {
                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException($"Directory {directory} not found");
                }
            }
        }

        private void InitializeEngines(IEnumerable<string> directories, IList<OptionsEnum> options)
        {
            addEngine(ProvideFFmpegEngine.Create(options));

            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);

                var backupHandler = new DirectoryBackupHandler(directoryInfo, options);

                addEngine(PngToJpgEngine.Create(options, directoryInfo, backupHandler));
                addEngine(AudioToOpusEngine.Create(options, directoryInfo, backupHandler));
            }

            progress.TotalEngines = Engines.Count;

            void addEngine(ICompressionEngine? x)
            {
                if (x != null)
                    Engines.Add(x);
            }
        }
    }
}
