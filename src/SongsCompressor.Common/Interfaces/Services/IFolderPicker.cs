namespace SongsCompressor.Common.Interfaces
{
    public interface IFolderPicker
    {
        Task<string> PickFolder();
    }
}
