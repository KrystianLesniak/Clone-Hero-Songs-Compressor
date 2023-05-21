using Engine.AudioToOpusConverter;
using Engine.FFmpegProvider;
using Engine.ImagePngToJpgConverter;
using Moq;
using SongCompressor.MainManager;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Models;

namespace SongsCompressor.MainManager.Tests
{
    [TestFixture]
    public class CompressiongManagerTests
    {
        private readonly IList<string> _directories = new List<string> {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir1"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir2") };

        [SetUp]
        public void SetUp()
        {
            foreach(var directory in _directories)
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }

                Directory.CreateDirectory(directory);
            }
        }

        [Test]
        public void WhenStart_DirectoryNotFound_ThrowDirectoryNotFoundException()
        {
            var settingsStorageMock = new Mock<ISettingsStorage>();
            var settings = new UserSettings
            {
                Directories = new List<string> { "not_existing_dir", "same" },
                Options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertPngToJpg }
            };
            settingsStorageMock.Setup(x => x.SaveSettings(settings)).Returns(Task.CompletedTask);

            var compressionManager = new CompressionManager(settingsStorageMock.Object);

            Assert.ThrowsAsync<DirectoryNotFoundException>(
                async () => await compressionManager.Initialize(settings));
        }

        [Test]
        public void WhenStart_NoDirectories_ThrowArgumentException()
        {
            var settingsStorageMock = new Mock<ISettingsStorage>();
            var settings = new UserSettings
            {
                Directories = new List<string>(),
                Options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertPngToJpg }
            };
            settingsStorageMock.Setup(x => x.SaveSettings(settings)).Returns(Task.CompletedTask);

            var compressionManager = new CompressionManager(settingsStorageMock.Object);

            Assert.ThrowsAsync<ArgumentException>(
                               async () => await compressionManager.Initialize(settings));
        }

        [Test]
        public void WhenStart_NoOptions_ThrowArgumentException()
        {
            var settingsStorageMock = new Mock<ISettingsStorage>();
            var settings = new UserSettings
            {
                Directories = _directories,
                Options = new List<OptionsEnum>()
            };
            settingsStorageMock.Setup(x => x.SaveSettings(settings)).Returns(Task.CompletedTask);

            var compressionManager = new CompressionManager(settingsStorageMock.Object);

            Assert.ThrowsAsync<ArgumentException>(
                                              async () => await compressionManager.Initialize(settings));
        }


        [Test]
        public async Task InitializeEnginesTest()
        {
            var settingsStorageMock = new Mock<ISettingsStorage>();
            var settings = new UserSettings
            {
                Directories = _directories,
                Options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertPngToJpg }
            };
            settingsStorageMock.Setup(x => x.SaveSettings(settings)).Returns(Task.CompletedTask);

            var compressionManager = new CompressionManager(settingsStorageMock.Object);
            await compressionManager.Initialize(settings);

            var progress = await compressionManager.GetCurrentProgressStatus();

            Assert.That(compressionManager.Engines.Any(x => x is ProvideFFmpegEngine));
            Assert.That(compressionManager.Engines.Any(x => x is PngToJpgEngine));
            Assert.That(compressionManager.Engines.Any(x => x is AudioToOpusEngine));
            Assert.That(compressionManager.Engines.All(x => x.Completed == false));
            Assert.That(progress.EnginesFinished, Is.EqualTo(0));
            Assert.That(progress.OverallEnginePercentageComplete, Is.EqualTo(0));
        }

        [Test]
        public async Task StartTest()
        {
            var settingsStorageMock = new Mock<ISettingsStorage>();
            var settings = new UserSettings
            {
                Directories = _directories,
                Options = new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus, OptionsEnum.ConvertPngToJpg }
            };
            settingsStorageMock.Setup(x => x.SaveSettings(settings)).Returns(Task.CompletedTask);

            var compressionManager = new CompressionManager(settingsStorageMock.Object);
            await compressionManager.Initialize(settings);

            await compressionManager.Start();

            var progress = await compressionManager.GetCurrentProgressStatus();

            Assert.That(compressionManager.Engines.Any(x => x is ProvideFFmpegEngine));
            Assert.That(compressionManager.Engines.Any(x => x is PngToJpgEngine));
            Assert.That(compressionManager.Engines.Any(x => x is AudioToOpusEngine));
            Assert.That(compressionManager.Engines.All(x => x.Completed));
            Assert.That(progress.EnginesFinished, Is.EqualTo(compressionManager.Engines.Count));
            Assert.That(progress.OverallEnginePercentageComplete, Is.EqualTo(100));
        }

    }
}