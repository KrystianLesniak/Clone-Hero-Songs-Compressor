using Engine.FFmpegProvider;
using Engine.Tests.Common;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Services;
using System.Runtime.CompilerServices;

namespace Engine.ImagePngToJpgConverter.Tests
{
    [TestFixture]
    public class PngToJpgEngineTests
    {
        private readonly string _resourcesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "dir1");
        private readonly string _uniqueDirectoriesParent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test_Directory");


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
            var ffmpegProvideEngine = ProvideFFmpegEngine.Create(_baseOptionsEnum);
            await ffmpegProvideEngine!.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(_uniqueDirectoriesParent, true);
        }

        private Task<DirectoryInfo> PrepareUniqureDirectoryForTest([CallerMemberName] string caller = "")
        {
            var directoryPath = Path.Combine(_uniqueDirectoriesParent, caller);

            EngineTestHelpers.CopyDirectory(_resourcesDirectory, directoryPath);

            return Task.FromResult(new DirectoryInfo(directoryPath));
        }

        [Test]
        public async Task EngineNotInitializedProperly()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();

            //Act
            var engine = PngToJpgEngine.Create(new List<OptionsEnum> { OptionsEnum.ResizeAlbum, OptionsEnum.CreateBackup, OptionsEnum.ConvertAudioToOpus }, directoryInfo, new BackupHandler(directoryInfo, _baseOptionsEnum));

            //Assert
            Assert.That(engine, Is.Null);
        }


        [Test]
        public async Task SuccessfullyConvertedNumberOfFiles()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();
            var originalPngFilesLength = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories).Length;
            var engine =  PngToJpgEngine.Create(_baseOptionsEnum, directoryInfo,  new BackupHandler(directoryInfo, _baseOptionsEnum));

            //Act
            await engine!.Start();
            var progress = await engine.GetCurrentProgress();

            //Assert
            var jpgFilesLength = directoryInfo.GetFiles("*.jpg", SearchOption.AllDirectories).Length;
            var pngFilesLength = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories).Length;

            Assert.That(progress.PercentageComplete, Is.EqualTo(100));
            Assert.That(progress.WorkDescription, Is.EqualTo("Compressing images from PNG to JPEG format finished"));
            Assert.That(jpgFilesLength, Is.EqualTo(originalPngFilesLength));
            Assert.That(pngFilesLength, Is.EqualTo(0));
        }

        [Test]
        public async Task ConvertedImagesAreSmallerThanOriginal()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();
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
    }
}