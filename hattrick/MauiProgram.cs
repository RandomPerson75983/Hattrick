using Microsoft.Extensions.Logging;
using Hattrick.Core.Services;

namespace Hattrick;

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

		// Register infrastructure services
		builder.Services.AddSingleton<IRandomProvider, RandomProvider>();
		builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
		builder.Services.AddSingleton<ISaveGameService, SaveGameService>();
		builder.Services.AddSingleton<ISaveSlotService, SaveSlotService>();
		builder.Services.AddSingleton<IGameStateService, GameStateService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
