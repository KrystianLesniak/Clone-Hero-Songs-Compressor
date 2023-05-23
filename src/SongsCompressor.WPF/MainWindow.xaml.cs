using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using SongCompressor.MainManager;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Services.Services;
using System.Threading.Tasks;
using System;
using System.Windows;

namespace SongsCompressor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        public MainWindow()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddMudServices();

            services.AddLogging(ConfigureLogging);

            services.AddTransient<IFolderPicker, Services.FolderPicker>();
            services.AddScoped<ISettingsStorage, SettingsStorage>();
            services.AddSingleton<ICompressionManager, CompressionManager>();

            _serviceProvider = services.BuildServiceProvider();
            Resources.Add("services", _serviceProvider);
        }

        private void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, consoleTarget, "*");

            var fileTarget = new FileTarget
            {
                Name = "file",
                FileName = "${basedir}/logs/errors.log",
            };
            config.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, fileTarget, "*");

            loggingBuilder.AddNLog(config);

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            _serviceProvider.GetRequiredService<ILogger<MainWindow>>().LogError(exception, source);
        }
    }
}
