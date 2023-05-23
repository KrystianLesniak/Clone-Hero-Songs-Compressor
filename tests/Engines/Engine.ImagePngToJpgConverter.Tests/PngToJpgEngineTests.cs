using Engine.FFmpegProvider;
using Moq;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Services;
using System.IO;

namespace Engine.ImagePngToJpgConverter.Tests
{
    [TestFixture]
    public class PngToJpgEngineTests
    {
        private readonly string _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "dir1");

        private readonly IList<OptionsEnum> _baseOptionsEnum = new List<OptionsEnum>
        {
            OptionsEnum.ConvertPngToJpg
        };

        private readonly IList<OptionsEnum> _resizeOptionsEnums = new List<OptionsEnum>
        {
            OptionsEnum.ConvertPngToJpg,
            OptionsEnum.ResizeAlbum
        };

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            Directory.CreateDirectory(_directory);

            var ffmpegProvideEngine = ProvideFFmpegEngine.Create(_baseOptionsEnum);
            await ffmpegProvideEngine!.Start();
        }

        [TearDown]
        public void TearDown()
        {

            foreach (string sFile in Directory.GetFiles(_directory, "*.jpg"))
            {
                File.Delete(sFile);
            }
        }

        [Test]
        [NonParallelizable]
        public async Task SuccessfullyConvertedNumberOfFiles()
        {
            //Arrange
            var directoryInfo = new DirectoryInfo(_directory);
            var originalPngFilesLength = Directory.GetFiles(_directory, "*.png", SearchOption.AllDirectories).Length;
            var engine =  PngToJpgEngine.Create(_baseOptionsEnum, directoryInfo,  new BackupHandler(directoryInfo, _baseOptionsEnum));

            //Act
            await engine!.Start();
            //TODO: WTF check why this delay is needed
            await Task.Delay(200);
            var progress = await engine.GetCurrentProgress();
            //Assert
            var jpgFilesLength = Directory.GetFiles(_directory, "*.jpg", SearchOption.AllDirectories).Length;
            var pngFilesLength = Directory.GetFiles(_directory, "*.png", SearchOption.AllDirectories).Length;

            Assert.That(progress.PercentageComplete, Is.EqualTo(100));
            Assert.That(progress.WorkDescription, Is.EqualTo("Compressing images from PNG to JPEG format finished"));
            Assert.That(jpgFilesLength, Is.EqualTo(originalPngFilesLength));
            Assert.That(pngFilesLength, Is.EqualTo(0));
        }

        [Test]
        [NonParallelizable]
        public async Task ConvertedImagesAreSmallerThanOriginal()
        {
            //Arrange
            var directoryInfo = new DirectoryInfo(_directory);
            var originalPngFiles = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories);
            var engine = PngToJpgEngine.Create(_baseOptionsEnum, directoryInfo, new BackupHandler(directoryInfo, _baseOptionsEnum));

            //Act
            await engine!.Start();

            //Assert
            var jpgFiles = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories);

            foreach(var jpgFile in jpgFiles)
            {
                var originalPng = originalPngFiles.First(x => x.Name == jpgFile.Name.Replace(".jpg", ".png"));

                Assert.That(jpgFile.Length, Is.LessThan(originalPng.Length));
            }
        }

        //TODO: Add test for resizing
        //TODO: Add more files

    }
}