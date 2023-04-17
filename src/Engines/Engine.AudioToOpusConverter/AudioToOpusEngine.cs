using FFMpegCore;
using SongsCompressor.Common.Base_Classes;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;
using System.Reflection;

namespace Engine.AudioToOpusConverter
{
    public class AudioToOpusEngine : BaseEngine
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly IBackupHandler backupHandler;
        private readonly IEnumerable<OptionsEnum> options;

        //Progress Description
        private readonly EngineProgressStatus _progress = new();

        private AudioToOpusEngine(DirectoryInfo directoryInfo, IEnumerable<OptionsEnum> options, IBackupHandler backupHandler)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.backupHandler = backupHandler ?? throw new ArgumentNullException(nameof(backupHandler));

            GlobalFFOptions.Configure(options => options.BinaryFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty);
        }
        public static ICompressionEngine? Create(IList<OptionsEnum> options, DirectoryInfo directoryInfo, IBackupHandler backupHandler)
        {
            if (options.Contains(OptionsEnum.ConvertAudioToOpus))
                return new AudioToOpusEngine(directoryInfo, options, backupHandler);

            return null;
        }

        public override Task<EngineProgressStatus> GetCurrentProgress()
        {
            return Task.FromResult(_progress);
        }

        public override async Task Start()
        {
            var convertingTasks = new HashSet<Task>();
            _progress.WorkDescription = "Retrieving Audio files";

            var audioFiles = await GetAudioFilesInfo();

            int index = 0;
            foreach (var audioFile in audioFiles)
            {
                convertingTasks.Add(ConvertAudioToOpus(audioFile));

                _progress.WorkDescription = $"Converting file {audioFile.Directory?.Name}\\{audioFile.Name} into Opus format";
                index++;

                if (index % Environment.ProcessorCount == 0)
                {
                    await Task.WhenAll(convertingTasks);
                    _progress.PercentageComplete = (int)Math.Round((double)(100 * index) / audioFiles.Count);
                }
            }

            await Task.WhenAll(convertingTasks);

            _progress.PercentageComplete = 100;
            _progress.WorkDescription = "Compressing Audio files into Opus format finished";
        }

        private Task<HashSet<FileInfo>> GetAudioFilesInfo()
        {
            var audioFiles = new HashSet<FileInfo>();

            if (options.Contains(OptionsEnum.ConvertAudioFromOgg))
                audioFiles.UnionWith(directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories));

            if (options.Contains(OptionsEnum.ConvertAudioFromMp3))
                audioFiles.UnionWith(directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories));

            return Task.FromResult(audioFiles);
        }

        private async Task ConvertAudioToOpus(FileInfo audioFile)
        {
            await backupHandler.BackupFile(audioFile);

            var outputPath = Path.ChangeExtension(audioFile.FullName, ".opus");

            await FFMpegArguments
                .FromFileInput(audioFile)
                .OutputToFile(outputPath, true, options => options
                    .WithAudioCodec("libopus")
                    .WithAudioBitrate(96))
                .ProcessAsynchronously();

            audioFile.Delete();
        }
    }
}