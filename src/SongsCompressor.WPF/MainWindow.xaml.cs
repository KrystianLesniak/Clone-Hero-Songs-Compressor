using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SongCompressor.MainManager;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Services.Services;
using MudBlazor.Services;
using System.Threading.Tasks;
using System.Reflection;
using Ookii.Dialogs.Wpf;
using System;
using System.Diagnostics;
using System.Security.Policy;
using SongsCompressor.Common.Consts;

namespace SongsCompressor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _ = CheckForUpdates();

            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddMudServices();

            services.AddTransient<IFolderPicker, Services.FolderPicker>();
            services.AddScoped<ISettingsStorage, SettingsStorage>();
            services.AddSingleton<ICompressionManager, CompressionManager>();

            Resources.Add("services", services.BuildServiceProvider());
        }

        private async Task CheckForUpdates()
        {
            var localVersion = Assembly.GetEntryAssembly()?.GetName().Version;
            if(localVersion == null )
                return;

            var updateChecker = new UpdateChecker(localVersion);

            if (await updateChecker.CheckForNewGitHubVersion())
            {
                using TaskDialog dialog = new();

                dialog.WindowTitle = $"Update to {updateChecker.LatestVersionString}";
                dialog.MainInstruction = "New version available. Would you like to go to the download page now?";
                dialog.Content = $"We have detected a newer version of the application: {updateChecker.LatestVersionString}. Your current version is {localVersion}. It's always a good idea to stay up to date with the latest versions of our application as they come with enhanced stability and performance improvements.";
                dialog.EnableHyperlinks = true;
                TaskDialogButton okButton = new(ButtonType.Ok);
                TaskDialogButton cancelButton = new(ButtonType.Cancel);

                dialog.Buttons.Add(okButton);
                dialog.Buttons.Add(cancelButton);
                TaskDialogButton button = dialog.ShowDialog(this);

                if (button == okButton)
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = AppInfoConsts.RepositoryReleasesUrl,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
            }
        }
    }
}
