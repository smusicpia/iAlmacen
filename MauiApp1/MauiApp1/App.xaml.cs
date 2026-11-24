using iAlmacen.Clases;
using iAlmacen.Views;
using iAlmacen.WebApi;

namespace iAlmacen
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var logueado = Preferences.Get("logueado", string.Empty);
			Global.tokenAPI = Preferences.Default.Get("tokenAPI", string.Empty);
			Global.refreshTokenAPI = Preferences.Default.Get("refreshTokenAPI", string.Empty);
			Global.guid = Preferences.Default.Get("Guid", string.Empty);
			Global.clave_usuario = Preferences.Default.Get("clave_usuario", string.Empty);
			Global.pass = Preferences.Default.Get("Password", string.Empty);

			if (string.IsNullOrEmpty(Global.clave_usuario) || (string.IsNullOrEmpty(logueado) && string.IsNullOrEmpty(Global.tokenAPI) && string.IsNullOrEmpty(Global.refreshTokenAPI) && string.IsNullOrEmpty(Global.guid)))
			{
				MainPage = new LoginView();
			}
			else
			{
				MainPage = new ContentPage
				{
					Content = new ActivityIndicator
					{
						IsRunning = true,
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center
					}
				};
				_ = ReloguearAsync();
			}
		}

		private async Task ReloguearAsync()
		{
			Page destino;

			HttpResponseMessage response;
			try
			{
				destino = new AppShell();
				MainPage = destino;
				response = await SesionApi.GetAsync($"api/catalogo/Areas?tProyecto={ConfigAPI.TipoProyecto}&Sucursal=M");

				//HttpStatusCode httpStatusCode = await Funciones.Login(Global.clave_usuario, Global.pass);
				//destino = httpStatusCode == HttpStatusCode.OK ? new AppShell() : new NavigationPage(new LoginView());
			}
			catch (Exception)
			{
				destino = new LoginView();
			}
			//MainPage = destino;
		}
	}
}