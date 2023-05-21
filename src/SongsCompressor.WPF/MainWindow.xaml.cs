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
    }
}
