using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;

namespace SongsCompressor.Common.Services
{
    public class DirectoryBackupHandler : IDirectoryBackupHandler
    {
        private readonly DirectoryInfo originalDirectory;
        private readonly DirectoryInfo backupDirectory;

        private readonly bool IsBackupActivated;

        private const string _backupFolderModifier = " - Backup";

        public DirectoryBackupHandler(DirectoryInfo originalDirectory, IList<OptionsEnum> options)
        {
            ArgumentNullException.ThrowIfNull(originalDirectory, nameof(originalDirectory));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            IsBackupActivated = options.Contains(OptionsEnum.CreateBackup);

            this.originalDirectory = originalDirectory;
            this.backupDirectory = PrepareBackupDirectoryPath(originalDirectory);
        }

        private DirectoryInfo PrepareBackupDirectoryPath(DirectoryInfo originalDirectory)
        {

            var backupPath = Path.Combine(originalDirectory.Parent?.FullName ?? string.Empty, originalDirectory.Name + _backupFolderModifier);

            return new DirectoryInfo(backupPath);
        }

        public async Task BackupFile(FileInfo file)
        {
            if (IsBackupActivated == false)
                return;

            string backupFilePath = file.FullName.Replace(originalDirectory.FullName, backupDirectory.FullName);

            Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath) ?? string.Empty);
            File.Copy(file.FullName, backupFilePath, true);

            await Task.CompletedTask;
        }
    }
}
