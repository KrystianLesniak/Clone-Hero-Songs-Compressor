using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SongCompressor.MainManager;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Interfaces.Services;
using SongsCompressor.Services.Services;

namespace CloneHeroSongsCompressor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();

        #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        #endif

#if WINDOWS
        builder.Services.AddTransient<IFolderPicker, CloneHeroSongsCompressor.Platforms.Windows.Services.FolderPicker>();
#elif MACCATALYST //TODO: This service for macOS
        //builder.Services.AddTransient<IFolderPicker, CloneHeroSongsCompressor.Platforms.MacCatalyst.Services.FolderPicker>();
#endif

        builder.Services.AddScoped<ISettingsStorage, SettingsStorage>();
        builder.Services.AddSingleton<ICompressionManager, CompressionManager>();

        return builder.Build();
    }
}
