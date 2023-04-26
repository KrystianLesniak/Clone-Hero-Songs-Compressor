using SongsCompressor.Common.Interfaces;
using System.Threading.Tasks;

namespace SongsCompressor.WPF.Services
{
    public class FolderPicker : IFolderPicker
    {
        public async Task<string> PickFolder()
        {
            //TODO:
            return "path";
        }
    }
}
