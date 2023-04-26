namespace SongsCompressor.Common.Interfaces
{
    public interface IBackupHandler
    {
        Task BackupFile(FileInfo file);
    }
}