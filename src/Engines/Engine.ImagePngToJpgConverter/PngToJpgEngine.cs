using FFMpegCore;
using SongsCompressor.Common.Base_Classes;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;
using System.Drawing;

namespace Engine.ImagePngToJpgConverter
{
    public class PngToJpgEngine : BaseEngine
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly IBackupHandler backupHandler;

        private readonly HashSet<Task> _convertingTasks = new();
        private readonly string _albumFileName = "album.png";

        //Progress Description
        private string _currentWorkDescription = "Starting conversion of images from PNG to JPG format";
        private int _percentageComplete;

        private PngToJpgEngine(DirectoryInfo directoryInfo, IBackupHandler backupHandler)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            this.backupHandler = backupHandler ?? throw new ArgumentNullException(nameof(backupHandler));
        }

        public static ICompressionEngine? Create(IList<OptionsEnum> options, DirectoryInfo directoryInfo, IBackupHandler backupHandler)
        {
            if (options.Contains(OptionsEnum.ConvertPngToJpg))
                return new PngToJpgEngine(directoryInfo, backupHandler);

            return null;
        }

        public override Task<EngineProgressStatus> GetCurrentProgress()
        {
            return Task.FromResult(
                new EngineProgressStatus
                {
                    WorkDescription = _currentWorkDescription,
                    PercentageComplete = _percentageComplete
                });
        }

        public override async Task Start()
        {
            var pngs = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
            var pngsCount = pngs.Length;

            int index = 0;
            foreach (var pngFile in pngs)
            {
                _convertingTasks.Add(ConvertPngToJpg(pngFile));
                _currentWorkDescription = $"Converting file {pngFile.Directory?.Name}\\{pngFile.Name} into JPEG format";
                index++;

                if (index % (Environment.ProcessorCount / 2) == 0)
                {
                    await Task.WhenAll(_convertingTasks);
                    _percentageComplete = (int)Math.Round((double)(100 * index) / pngsCount);
                }
            }

            await Task.WhenAll(_convertingTasks);

            _percentageComplete = 100;
            _currentWorkDescription = "Compressing images from PNG to JPEG format finished";
        }

        private async Task ConvertPngToJpg(FileInfo pngFileInfo)
        {
            await backupHandler.BackupFile(pngFileInfo);

            var outputPath = Path.ChangeExtension(pngFileInfo.FullName, ".jpg");

            await FFMpegArguments
                .FromFileInput(pngFileInfo)
                .OutputToFile(outputPath, true, GetImageOptions)
            .ProcessAsynchronously();

            void GetImageOptions(FFMpegArgumentOptions options)
            {
                //if album.png scale to 500px
                if (pngFileInfo.Name.Equals(_albumFileName, StringComparison.OrdinalIgnoreCase))
                    options.WithVideoFilters(x => x.Scale(new Size(500, -1)));
            }

            pngFileInfo.Delete();
        }
    }
}