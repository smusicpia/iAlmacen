using CommunityToolkit.Maui;

using Controls.UserDialogs.Maui;

using iAlmacen.Clases;
using iAlmacen.Handlers;
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

			//// 1. Registrar el DelegatingHandler como Transitorio
			//builder.Services.AddTransient<RefreshTokenHandler>();

			//// 2. Registrar el HttpClient y asociarle el Handler
			//builder.Services.AddHttpClient("api", client =>
			//{
			//	client.BaseAddress = new Uri(ConfigAPI.Servidor);
			//})
			//.AddHttpMessageHandler<RefreshTokenHandler>();

			// Registrar tus servicios o páginas que usen el cliente
			builder.Services.AddTransient<ConfigAPI>();
			//builder.Services.AddTransient<SesionApi>();
			//builder.Services.AddTransient<APIService>();
			builder.Services.AddTransient<Funciones>();
#if DEBUG
			builder.Logging.AddDebug();
#endif
            //builder.Services.AddSingleton<ItemsViewModel_Recoleccion>();
			return builder;
        }
    }
}