using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SongCompressor.MainManager;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Services.Services;
using System.Diagnostics.Metrics;

namespace SongsCompressor.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();

            services.AddMudServices();

            services.AddTransient<IFolderPicker, Services.FolderPicker>();
            services.AddScoped<ISettingsStorage, SettingsStorage>();
            services.AddSingleton<ICompressionManager, CompressionManager>();

            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<Main>("#app");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}