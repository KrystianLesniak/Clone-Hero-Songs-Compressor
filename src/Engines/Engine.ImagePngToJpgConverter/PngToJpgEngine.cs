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
        private readonly IDirectoryBackupHandler backupHandler;
        private readonly IEnumerable<OptionsEnum> options;

        private readonly string _albumFileName = "album.png";

        //Progress Description
        private readonly EngineProgressStatus _progress = new() { WorkDescription = "Starting conversion of images from PNG to JPG format" };
        private int pngFilesCount;
        private int pngFilesConvertedCount;

        private PngToJpgEngine(IEnumerable<OptionsEnum> options, DirectoryInfo directoryInfo, IDirectoryBackupHandler backupHandler)
        {
            this.directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.backupHandler = backupHandler ?? throw new ArgumentNullException(nameof(backupHandler));
        }

        public static ICompressionEngine? Create(IList<OptionsEnum> options, DirectoryInfo directoryInfo, IDirectoryBackupHandler backupHandler)
        {
            if (options.Contains(OptionsEnum.ConvertPngToJpg))
                return new PngToJpgEngine(options, directoryInfo, backupHandler);

            return null;
        }

        public override Task<EngineProgressStatus> GetCurrentProgress()
        {
            _progress.PercentageComplete = (int)Math.Round(100 * pngFilesConvertedCount / (pngFilesCount == 0 ? 1.0 : pngFilesCount));

            return Task.FromResult(_progress);
        }

        public override async Task Start()
        {
            var convertingTasks = new HashSet<Task>();
            var pngs = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
            pngFilesCount = pngs.Length;

            await Parallel.ForEachAsync(pngs, async (pngFile, ct) =>
            {
                _progress.WorkDescription = $"Converting file {pngFile.Directory?.Name}\\{pngFile.Name} into JPEG format";

                await ConvertPngToJpg(pngFile);

                pngFilesConvertedCount++;
            });

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

            void GetImageOptions(FFMpegArgumentOptions ffmpegOptions)
            {
                //if album.png scale to 500px
                if (options.Contains(OptionsEnum.ResizeAlbum) &&
                    pngFileInfo.Name.Equals(_albumFileName, StringComparison.OrdinalIgnoreCase))
                {
                    ffmpegOptions.WithVideoFilters(x => x.Scale(new Size(500, -1)));
                }
            }

            if (File.Exists(outputPath))
            {
                pngFileInfo.IsReadOnly = false;
                pngFileInfo.Delete();
            }
        }
    }
}