using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Services;
using System.IO;
using System.Runtime.CompilerServices;
using Tests.Common;

namespace SongsCompressor.Services.Tests
{
    public class DirectoryBackupHandlerTests
    {
        private static readonly string _resourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
        private static readonly string _uniqueDirectoriesParent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test_Backup");

        private const string _backupFolderModifier = " - Backup";


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(_uniqueDirectoriesParent, true);
        }

        private Task<DirectoryInfo> PrepareUniqureDirectoryForTest([CallerMemberName] string caller = "")
        {
            var directoryPath = Path.Combine(_uniqueDirectoriesParent, caller);

            TestHelpers.CopyDirectory(_resourceDirectory, directoryPath);

            return Task.FromResult(new DirectoryInfo(directoryPath));
        }

        [Test]
        public async Task When_NoBackupOption_DoNotCreateBackup()
        {
            var directory = await PrepareUniqureDirectoryForTest();
            var resourceFiles = directory.GetFiles("*", SearchOption.AllDirectories);
            var options = new List<OptionsEnum> { OptionsEnum.ResizeAlbum, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg };

            var backupHandler = new DirectoryBackupHandler(directory, options);
            var backupDirectory = Path.Combine(directory.Parent?.FullName ?? string.Empty, directory.Name + _backupFolderModifier);

            foreach (var backupFile in resourceFiles)
            {
                await backupHandler.BackupFile(backupFile);

                Assert.That(Directory.Exists(backupDirectory), Is.False);
            }
        }

        [Test]
        public async Task When_BackupOption_CreateBackup()
        {
            var directory = await PrepareUniqureDirectoryForTest();
            var resourceFiles = directory.GetFiles("*", SearchOption.AllDirectories);

            var options = new List<OptionsEnum> { OptionsEnum.ResizeAlbum, OptionsEnum.ConvertAudioFromMp3, OptionsEnum.ConvertAudioFromOgg, OptionsEnum.CreateBackup };
            var backupHandler = new DirectoryBackupHandler(directory, options);

            var backupDirectory = Path.Combine(directory.Parent?.FullName ?? string.Empty, directory.Name + _backupFolderModifier);

            foreach (var resourceFile in resourceFiles)
            {

                await backupHandler.BackupFile(resourceFile);

                var backupFile = resourceFile.FullName.Replace(directory.FullName, backupDirectory); ;
                Assert.That(File.Exists(backupFile));
                Assert.That(File.ReadAllBytes(resourceFile.FullName), Is.EqualTo(File.ReadAllBytes(backupFile)));
            }
            Assert.That(Directory.Exists(backupDirectory), Is.True);
        }
    }
}