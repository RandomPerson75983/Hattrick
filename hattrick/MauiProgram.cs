using Hattrick.Core;
using Hattrick.Core.Services;
using Microsoft.Extensions.Logging;

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
		builder.Services.AddHattrickCoreServices();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Seed development data on startup
		var devSeedService = app.Services.GetRequiredService<IDevSeedService>();
		devSeedService.SeedAsync().GetAwaiter().GetResult();

		return app;
	}
}
