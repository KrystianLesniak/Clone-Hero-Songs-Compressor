using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SongCompressor.MainManager;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Services.Services;
using System.Windows;

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
