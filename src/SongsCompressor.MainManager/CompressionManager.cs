using Engine.AudioToOpusConverter;
using Engine.FFmpegProvider;
using Engine.ImagePngToJpgConverter;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;
using SongsCompressor.Common.Services;

namespace SongCompressor.MainManager
{
    public class CompressionManager : ICompressionManager
    {
        public IList<ICompressionEngine> Engines { get; set; } = new List<ICompressionEngine>();

        //Progress status
        private ICompressionEngine? _currentlyRunningEngine;

        public Task Initialize(IList<string> directories, IList<OptionsEnum> options)
        {
            InitializeEngines(directories.Select(x => new DirectoryInfo(x)), options);

            return Task.CompletedTask;
        }

        public async Task Start()
        {
            foreach (var engine in Engines.OrderBy(x => x.ExecutionOrder))
            {
                _currentlyRunningEngine = engine;
                await engine.Start();

                await engine.Complete();
            }
        }

        public async Task<OverallProgressStatus> GetCurrentProgressStatus()
        {
            return new OverallProgressStatus
            {
                TotalEngines = Engines.Count,
                EnginesFinished = Engines.Count(x => x.Completed),
                EngineProgress = _currentlyRunningEngine is not null ? await _currentlyRunningEngine.GetCurrentProgress() : new EngineProgressStatus()
            };
        }

        private void InitializeEngines(IEnumerable<DirectoryInfo> directories, IList<OptionsEnum> options)
        {
            addEngine(ProvideFFmpegEngine.Create(options));

            foreach (var directory in directories)
            {
                var backupHandler = new BackupHandler(directory, options);

                addEngine(PngToJpgEngine.Create(options, directory, backupHandler));
                addEngine(AudioToOpusEngine.Create(options, directory, backupHandler));
            }

            void addEngine(ICompressionEngine? x)
            {
                if (x != null)
                    Engines.Add(x);
            }
        }
    }
}
