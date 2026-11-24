using iAlmacen.WebApi;

namespace iAlmacen.Clases
{
	public static class HttpClienteInstancia
	{
		// Instancia única y estática para toda la aplicación
		private static readonly HttpClient _client;

		//private static readonly HttpClient? _client = new CrearCliente();

		static HttpClienteInstancia()
		{
#if DEBUG
			// Solo desarrollo: acepta el certificado HTTPS de desarrollo (dev-certs).
			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
			};
			_client = new HttpClient(handler);
#else
			_client = new HttpClient();
#endif
			// Configura la URL base
			_client.BaseAddress = new Uri(ConfigAPI.Servidor);
			_client.Timeout = TimeSpan.FromSeconds(20);

			//// Limpia y configura las cabeceras por defecto (JSON)
			//_client.DefaultRequestHeaders.Accept.Clear();
			//_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			//var logueado = Preferences.Get("logueado", string.Empty);
			//if (!string.IsNullOrEmpty(Global.tokenAPI) && !string.IsNullOrEmpty(Global.refreshTokenAPI) && !string.IsNullOrEmpty(Global.guid) && !string.IsNullOrEmpty(logueado))
			//{
			//	_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
			//	_client.DefaultRequestHeaders.Add("RefreshToken", $"{Global.refreshTokenAPI}");
			//}
		}

		//		private static HttpClient CrearCliente()
		//		{
		//#if DEBUG
		//			// Solo desarrollo: acepta el certificado HTTPS de desarrollo (dev-certs).
		//			var handler = new HttpClientHandler
		//			{
		//				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
		//			};
		//			var client = new HttpClient(handler);
		//#else
		//			var client = new HttpClient();
		//#endif
		//			client.BaseAddress = new Uri(ConfigAPI.Servidor);
		//			client.Timeout = TimeSpan.FromSeconds(20);

		//			client.DefaultRequestHeaders.Accept.Clear();
		//			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		//			var logueado = Preferences.Get("logueado", string.Empty);
		//			if (!string.IsNullOrEmpty(Global.tokenAPI) && !string.IsNullOrEmpty(Global.refreshTokenAPI) && !string.IsNullOrEmpty(Global.guid) && !string.IsNullOrEmpty(logueado))
		//			{
		//				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
		//				client.DefaultRequestHeaders.Add("RefreshToken", $"{Global.refreshTokenAPI}");
		//			}
		//			return client;
		//		}

		public static HttpClient ClientLogin
		{
			get
			{
				// Limpia y configura las cabeceras por defecto (JSON)
				_client.DefaultRequestHeaders.Accept.Clear();
				_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				return _client;
			}
		}

		public static HttpClient Client
		{
			get {
				// Limpia y configura las cabeceras por defecto (JSON)
				_client.DefaultRequestHeaders.Clear();
				_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				var logueado = Preferences.Get("logueado", string.Empty);
				if (!string.IsNullOrEmpty(Global.tokenAPI) && !string.IsNullOrEmpty(Global.refreshTokenAPI) && !string.IsNullOrEmpty(Global.guid) && !string.IsNullOrEmpty(logueado))
				{
					_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
					_client.DefaultRequestHeaders.Add("RefreshToken", $"{Global.refreshTokenAPI}");
				}
				return _client; 
			}
		}
	}
}
