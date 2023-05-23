namespace SongsCompressor.Common.Interfaces
{
    public interface IDirectoryBackupHandler
    {
        Task BackupFile(FileInfo file);
    }
}