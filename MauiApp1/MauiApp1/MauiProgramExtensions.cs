using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;

using iAlmacen.Clases;
using iAlmacen.WebApi;

using Microsoft.Extensions.Logging;

using ZXing.Net.Maui.Controls;

namespace iAlmacen
{
    public static class MauiProgramExtensions
    {
		public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                //.UseBarcodeScanning() // Initialize the scanner
                .UseMauiCommunityToolkit()
                .UseUserDialogs()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font-Awesome-5-Free-Solid-900.otf", "AwesomeSolid");
                })
                .UseBarcodeReader();

            // Registrar el interceptor de autorización para manejar respuestas 401
            builder.Services.AddTransient<UnauthorizeInterceptorHandler>();

            //         builder.Services.AddSingleton(new HttpClient
            //         {
            //             BaseAddress = new Uri(ConfigAPI.Servidor) // Cambia esto por la URL de tu API
            //});

            builder.Services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri(ConfigAPI.Servidor); // Cambia esto por la URL de tu API
            })
              .AddHttpMessageHandler<UnauthorizeInterceptorHandler>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            //builder.Services.AddSingleton<ItemsViewModel_Recoleccion>();
			return builder;
        }
    }
}