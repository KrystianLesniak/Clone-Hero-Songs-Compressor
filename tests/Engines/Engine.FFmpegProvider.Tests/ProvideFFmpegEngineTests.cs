using SongsCompressor.Common.Enums;
using System.Reflection;

namespace Engine.FFmpegProvider.Tests
{
    [TestFixture]
    public class ProvideFFmpegEngineTests
    {
        [Test]
        public async Task ProvideFFmpegEngineSuccessfullyProvideExe()
        {
            var ffmpegProvideEngine = ProvideFFmpegEngine.Create(new List<OptionsEnum> { OptionsEnum.ConvertAudioToOpus });

            await ffmpegProvideEngine!.Start();

            var exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "ffmpeg.exe");

            Assert.That(File.Exists(exePath), Is.True);
        }

        [Test]
        public void EngineNotInitializedProperly()
        {
            var ffmpegProvideEngine = ProvideFFmpegEngine.Create(new List<OptionsEnum> { OptionsEnum.CreateBackup, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ResizeAlbum });

            Assert.That(ffmpegProvideEngine, Is.Null);
        }
    }
}