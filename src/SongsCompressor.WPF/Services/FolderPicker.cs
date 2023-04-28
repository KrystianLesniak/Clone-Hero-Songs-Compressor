using Ookii.Dialogs.Wpf;
using SongsCompressor.Common.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace SongsCompressor.WPF.Services
{
    public class FolderPicker : IFolderPicker
    {
        readonly string defaultFolder = Debugger.IsAttached ? string.Empty : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Clone Hero", "Songs");

        public string[] PickFolders()
        {
            //TODO: .net 8 will provide native OpenFolderDialog method https://github.com/dotnet/wpf/issues/438 
            VistaFolderBrowserDialog dlg = new()
            {
                SelectedPath = defaultFolder,
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
