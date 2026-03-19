using Microsoft.Extensions.Logging;

namespace iAlmacen
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font-Awesome-5-Free-Solid-900.otf", "AwesomeSolid");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}