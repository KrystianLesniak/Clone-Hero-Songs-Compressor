using ImageMagick;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;

namespace SongsCompressor.ImagePngToJpgConverter
{
    public class PngToJpgEngine : ICompressionEngine
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

        public Task<EngineProgressStatus> GetCurrentProgress()
        {
            return Task.FromResult(
                new EngineProgressStatus
                {
                    WorkDescription = _currentWorkDescription,
                    PercentageComplete = _percentageComplete
                });
        }

        public async Task Start()
        {
            var pngs = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
            var pngsCount = pngs.Length;

            int index = 0;
            foreach (var pngFile in pngs)
            {
                _convertingTasks.Add(ConvertPngToJpg(pngFile));
                _currentWorkDescription = $"Converting file {pngFile.Directory?.Name}\\{pngFile.Name} into JPEG format";
                index++;

                if (index % (Environment.ProcessorCount / 2 ) == 0)
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

            using (var image = new MagickImage(pngFileInfo.FullName))
            {
                image.Format = MagickFormat.Jpeg;
                image.Quality = 80;

                if(pngFileInfo.Name == _albumFileName) //Resize albums cover to 500px
                    image.Resize(500, 0);

                await image.WriteAsync(Path.ChangeExtension(pngFileInfo.FullName, ".jpg"));
            }

            pngFileInfo.Delete();
        }
    }
}