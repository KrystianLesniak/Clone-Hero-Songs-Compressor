using SongCompressor.AudioToOpusConverter;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;
using SongsCompressor.Common.Services;
using SongsCompressor.ImagePngToJpgConverter;
using System.IO;

namespace SongCompressor.MainManager
{
    public class CompressionManager : ICompressionManager
    {
        public IList<ICompressionEngine> Engines { get; set; } = new List<ICompressionEngine>();

        //Progress status
        private ICompressionEngine? _currentlyRunningEngine;
        private ProgressStatusEnum _progressStatus = ProgressStatusEnum.Standby;
        private int _enginesPercentageComplete;

        public Task Initialize(IList<string> directories, IList<OptionsEnum> options)
        {
            _progressStatus = ProgressStatusEnum.Initializing;

            foreach (var directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                InitializeEngines(directoryInfo, options);
            }

            _progressStatus = ProgressStatusEnum.Initialized;
            return Task.CompletedTask;
        }

        public async Task Start()
        {
            _progressStatus = ProgressStatusEnum.InProgress;
            int index = 0;
            foreach(var engine in Engines)
            {
                _currentlyRunningEngine = engine;
                _enginesPercentageComplete = (int)Math.Round((double)(100 * index) / Engines.Count);

                await engine.Start();

                index++;
            }

            _enginesPercentageComplete = 100;
            _progressStatus = ProgressStatusEnum.Complete;
        }

        public async Task<OverallProgressStatus> GetCurrentProgressStatus()
        {
            var engineProgess = _currentlyRunningEngine is null ? new EngineProgressStatus() : await _currentlyRunningEngine.GetCurrentProgress();

            var enginePercentageForOverral = engineProgess.PercentageComplete * (1 / (double)Engines.Count);

            return new OverallProgressStatus
            {
                Status = _progressStatus,
                OverallPercentageComplete = _enginesPercentageComplete != 100 ? _enginesPercentageComplete + (int)enginePercentageForOverral : _enginesPercentageComplete,
                EngineProgress = engineProgess
            };
        }

        private void InitializeEngines(DirectoryInfo directory, IList<OptionsEnum> options)
        {
            var backupHandler = new BackupHandler(directory, options);

            addEngine(PngToJpgEngine.Create(options, directory, backupHandler));
            addEngine(AudioToOpusEngine.Create(options, directory, backupHandler));

            void addEngine(ICompressionEngine? x)
            {
                if (x != null)
                    Engines.Add(x);
            }
        }
    }
}
