using Ookii.Dialogs.Wpf;
using SongsCompressor.Common.Interfaces;
using System;
using System.IO;

namespace SongsCompressor.WPF.Services
{
    public class FolderPicker : IFolderPicker
    {
        readonly string defaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Clone Hero", "Songs");

        public string[] PickFolders()
        {
            //TODO: .net 8 will provide native OpenFolderDialog method https://github.com/dotnet/wpf/issues/438 
            VistaFolderBrowserDialog dlg = new()
            {
#if RELEASE
            dlg.SelectedPath = defaultFolder;
#endif
                ShowNewFolderButton = true,
                Multiselect = true
            };

            if (dlg.ShowDialog() ?? false)
            {
                return dlg.SelectedPaths;
            }
            return Array.Empty<string>();
        }
    }
}
