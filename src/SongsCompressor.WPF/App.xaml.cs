using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Windows;

namespace SongsCompressor.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ConfigureLogging();
        }

        private void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}",
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget, "*");

            var fileTarget = new FileTarget
            {
                Name = "file",
                FileName = "${basedir}/logs/errors.log",
            };
            config.AddRule(LogLevel.Error, LogLevel.Fatal, fileTarget, "*");

            LogManager.Configuration = config;

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Logger log = LogManager.GetCurrentClassLogger();
                log.Error(args.ExceptionObject as Exception);
            };
        }
    }
}
