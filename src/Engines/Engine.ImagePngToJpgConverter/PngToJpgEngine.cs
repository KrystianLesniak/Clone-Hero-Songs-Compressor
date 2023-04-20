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

        private readonly string _albumFileName = "album.png";

        //Progress Description
        private readonly EngineProgressStatus _progress = new() { WorkDescription = "Starting conversion of images from PNG to JPG format" };

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
            return Task.FromResult(_progress);
        }

        public override async Task Start()
        {
            var convertingTasks = new HashSet<Task>();
            var pngs = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);

            int index = 0;
            foreach (var pngFile in pngs)
            {
                convertingTasks.Add(ConvertPngToJpg(pngFile));
                index++;

                if (index % (Environment.ProcessorCount / 2) == 0)
                {
                    _progress.WorkDescription = $"Converting file {pngFile.Directory?.Name}\\{pngFile.Name} into JPEG format";
                    _progress.PercentageComplete = (int)Math.Round((double)(100 * index) / pngs.Length);

                    await Task.WhenAll(convertingTasks);
                }
            }

            await Task.WhenAll(convertingTasks);

            _progress.PercentageComplete = 100;
            _progress.WorkDescription = "Compressing images from PNG to JPEG format finished";
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