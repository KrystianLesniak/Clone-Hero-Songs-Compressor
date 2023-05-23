using Engine.FFmpegProvider;
using Engine.Tests.Common;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Services;
using System.Runtime.CompilerServices;

namespace Engine.AudioToOpusConverter.Tests
{
    [TestFixture]
    public class AudioToOpusEngineTests
    {
        private readonly string _resourcesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
        private readonly string _uniqueDirectoriesParent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test_Directory");


        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var ffmpegProvideEngine = ProvideFFmpegEngine.Create(new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus });
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
            var options = new List<OptionsEnum> { OptionsEnum.ResizeAlbum, OptionsEnum.CreateBackup, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Assert
            Assert.That(engine, Is.Null);
        }


        [Test]
        public async Task EngineInitializedProperly()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();

            //Act
            var options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Assert
            Assert.That(engine, Is.Not.Null);
        }

        [Test]
        public async Task SuccessfullyConvertedOnlyMp3()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();

            var originalOggLength = directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length;
            var originalMp3Length = directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            var options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertAudioFromMp3 };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Act
            await engine!.Start();
            var progress = await engine.GetCurrentProgress();

            //Assert
            var opusFilesLength = directoryInfo.GetFiles("*.opus", SearchOption.AllDirectories).Length;

            var oggFilesLength = directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length;
            var mp3FilesLength = directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            Assert.That(progress.PercentageComplete, Is.EqualTo(100));
            Assert.That(progress.WorkDescription, Is.EqualTo("Compressing Audio files into Opus format finished"));
            Assert.That(opusFilesLength, Is.EqualTo(originalMp3Length));
            Assert.That(originalOggLength, Is.EqualTo(oggFilesLength));
            Assert.That(mp3FilesLength, Is.EqualTo(0));
        }

        [Test]
        public async Task SuccessfullyConvertedOnlyOgg()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();

            var originalOggLength = directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length;
            var originalMp3Length = directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            var options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertAudioFromOgg };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Act
            await engine!.Start();
            var progress = await engine.GetCurrentProgress();

            //Assert
            var opusFilesLength = directoryInfo.GetFiles("*.opus", SearchOption.AllDirectories).Length;

            var oggFilesLength = directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length;
            var mp3FilesLength = directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            Assert.That(progress.PercentageComplete, Is.EqualTo(100));
            Assert.That(progress.WorkDescription, Is.EqualTo("Compressing Audio files into Opus format finished"));
            Assert.That(opusFilesLength, Is.EqualTo(originalOggLength));
            Assert.That(originalMp3Length, Is.EqualTo(mp3FilesLength));
            Assert.That(oggFilesLength, Is.EqualTo(0));
        }

        [Test]
        public async Task SuccessfullyConvertedNumberOfFiles()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();

            var originalFilesLength =
                directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length
                + directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            var options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Act
            await engine!.Start();
            var progress = await engine.GetCurrentProgress();

            //Assert
            var opusFilesLength = directoryInfo.GetFiles("*.opus", SearchOption.AllDirectories).Length;

            var mp3oggFilesLength =
                directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories).Length
                + directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories).Length;

            Assert.That(progress.PercentageComplete, Is.EqualTo(100));
            Assert.That(progress.WorkDescription, Is.EqualTo("Compressing Audio files into Opus format finished"));
            Assert.That(opusFilesLength, Is.EqualTo(originalFilesLength));
            Assert.That(mp3oggFilesLength, Is.EqualTo(0));
        }

        [Test]
        public async Task ConvertedAudiosAreSmallerThanOriginal()
        {
            //Arrange
            var directoryInfo = await PrepareUniqureDirectoryForTest();
            var originalFiles =
                directoryInfo.GetFiles("*.mp3", SearchOption.AllDirectories)
                .Union(directoryInfo.GetFiles("*.ogg", SearchOption.AllDirectories));

            var options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg };
            var engine = AudioToOpusEngine.Create(options, directoryInfo, new BackupHandler(directoryInfo, options));

            //Act
            await engine!.Start();

            //Assert
            var opusFiles = directoryInfo.GetFiles("*.opus", SearchOption.AllDirectories);

            foreach (var opusFile in opusFiles)
            {
                var originalFile =
                    originalFiles.FirstOrDefault(x => x.Name == opusFile.Name.Replace(".opus", ".mp3"))
                    ?? originalFiles.First(x => x.Name == opusFile.Name.Replace(".opus", ".ogg"));

                Assert.That(opusFile.Length, Is.LessThan(originalFile.Length));
            }
        }
    }
}