using SongsCompressor.Common.Interfaces;

namespace SongsCompressor.WinForms.Services
{
    public class FolderPicker : IFolderPicker
    {
        public async Task<string> PickFolder()
        {
            return "path";
        }
    }
}
