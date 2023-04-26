using SongsCompressor.Common.Interfaces;
using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

namespace CloneHeroSongsCompressor.Platforms.Windows.Services
{

    public class FolderPicker : IFolderPicker
    {
        public async Task<string> PickFolder()
        {
            var folderPicker = new WindowsFolderPicker();
            // Make it work for Windows 10
            folderPicker.FileTypeFilter.Add("*");
            // Get the current window's HWND by passing in the Window object
            var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var result = await folderPicker.PickSingleFolderAsync();

            if(result?.Path is null)
                return null;

            if (!Directory.Exists(result?.Path))
                throw new DirectoryNotFoundException($"Directory not found: {result?.Path}");

            return result?.Path;
        }
    }
}
