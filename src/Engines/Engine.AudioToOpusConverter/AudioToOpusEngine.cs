using FFMpegCore;
using SongsCompressor.Common.Base_Classes;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;

namespace Engine.AudioToOpusConverter
{
    public class AudioToOpusEngine : BaseEngine
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly IDirectoryBackupHandler backupHandler;
        private readonly IEnumerable<OptionsEnum> options;

        //Progress Description
        private int audioFilesCount;
        private int audioFilesConvertedCount;
        private readonly EngineProgressStatus _progress = new();

        private AudioToOpusEngine(DirectoryInfo directoryInfo, IEnumerable<OptionsEnum> options, IDirectoryBackupHandler backupHandler)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.backupHandler = backupHandler ?? throw new ArgumentNullException(nameof(backupHandler));
        }
        public static ICompressionEngine? Create(IList<OptionsEnum> options, DirectoryInfo directoryInfo, IDirectoryBackupHandler backupHandler)
        {
            if (options.Contains(OptionsEnum.ConvertAudioToOpus))
                return new AudioToOpusEngine(directoryInfo, options, backupHandler);

            return null;
        }

        public override Task<EngineProgressStatus> GetCurrentProgress()
        {
            _progress.PercentageComplete = (int)Math.Round(100 * audioFilesConvertedCount / (audioFilesCount == 0 ? 1.0 : audioFilesCount));

            return Task.FromResult(_progress);
        }

        public override async Task Start()
        {
            _progress.WorkDescription = "Retrieving Audio files";

            var audioFiles = await GetAudioFilesInfo();

            await Parallel.ForEachAsync(audioFiles, async (audioFile, ct) =>
            {
                if (audioFilesConvertedCount % Environment.ProcessorCount == 0)
                    _progress.WorkDescription = $"Converting file {audioFile.Directory?.Name}\\{audioFile.Name} into Opus format";

                await ConvertAudioToOpus(audioFile);

                audioFilesConvertedCount++;
            });

            _progress.WorkDescription = "Compressing Audio files into Opus format finished";
        }

        private Task<HashSet<FileInfo>> GetAudioFilesInfo()
        {
            var audioFiles = new HashSet<FileInfo>();

            if (options.Contains(OptionsEnum.ConvertAudioFromOgg))
                audioFiles.UnionWith(directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories));

            if (options.Contains(OptionsEnum.ConvertAudioFromMp3))
                audioFiles.UnionWith(directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories));

            audioFilesCount = audioFiles.Count;

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

            if (File.Exists(outputPath)) //TODO: This logic may be moved to common handler or backup handler
            {
                audioFile.IsReadOnly = false;
                audioFile.Delete();
            }
        }
    }
}