using iAlmacen.Clases;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Data;
using System.Net;

namespace iAlmacen.Handlers
{
	public class RefreshTokenHandler : DelegatingHandler
	{
		//private readonly IServiceProvider _serviceProvider;
		//private bool _isRefreshing;
		//private readonly SemaphoreSlim _semaphore = new(1, 1);

		//public RefreshTokenHandler(IServiceProvider serviceProvider)
		//{
		//	_serviceProvider = serviceProvider;
		//}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// 1. Envía la petición original con el token actual
			var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

			// 2. Si responde 401 Unauthorized, intenta refrescar
			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				if (!request.RequestUri.LocalPath.Contains("Authenticate") && Global.isAutoRefreshToken)
				{
					//await _semaphore.WaitAsync(cancellationToken);
					//try
					//{
					//	// Evitar múltiples peticiones simultáneas de refresco
					//	if (!_isRefreshing)
					//	{
					//		_isRefreshing = true;
					//		var refreshed = await RenewTokensAsync();
					//		_isRefreshing = false;

					//		if (!refreshed)
					//		{
					//			// Si falla el refresh, redirigir al login
					//			MainThread.BeginInvokeOnMainThread(async () =>
					//			{
					//				await Shell.Current.GoToAsync("//LoginPage");
					//			});
					//			return response;
					//		}
					//	}
					//}
					//finally
					//{
					//	_semaphore.Release();
					//}

					//// Reintentar con el nuevo token actualizado
					////var newAccessToken = await Funciones.GetTokenAPIAsync();
					//request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Global.tokenAPI);
					//return await base.SendAsync(request, cancellationToken);

					// Evita bucles si la ruta de refresco también da 401
					if (!request.Headers.Contains("X-Retry-Refresh"))
					{
						request.Headers.Add("X-Retry-Refresh", "true");

						var refreshed = await RenewTokensAsync(); // Tu lógica segura
																  //if (!string.IsNullOrEmpty(newToken))
																  //{
																  //	request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

						//	// 3. Reintenta la solicitud con el nuevo token
						//	return await base.SendAsync(request, cancellationToken);
						//}

						if (!refreshed)
						{
							// Si falla el refresh, redirigir al login
							MainThread.BeginInvokeOnMainThread(async () =>
							{
								await Shell.Current.GoToAsync("loginview");
							});
							return response;
						}
					}
				}
			}
			return response;
		}

		private static HttpRequestMessage HttpRequestHeader(HttpRequestMessage httpRequest, string Content = "")
		{
			httpRequest.Headers.Add("ContentType", "application/json");
			httpRequest.Headers.Add("aes1", $"{Convert.ToBase64String(Global.Key)}");
			httpRequest.Headers.Add("aes2", $"{Convert.ToBase64String(Global.IV)}");

			var content = new StringContent(Content, null, "application/json");
			httpRequest.Content = content;

			return httpRequest;
		}

		private async Task<bool> RenewTokensAsync()
		{
			//var refreshToken = await Funciones.GetRefreshTokenAPIAsync();
			if (string.IsNullOrEmpty(Global.refreshTokenAPI)) return false;

			// Crear HttpClient limpio para evitar loops con el Handler
			var client = new HttpClient();
			var request = new HttpRequestMessage();
			string Parametros = $"{Convert.ToBase64String(SecurityManager.Encrypt(Global.clave_usuario, Global.Key, Global.IV))},{Global.pass}";
			request = new HttpRequestMessage(HttpMethod.Get, ConfigAPI.Servidor + $"/api/login/RefreshJWTTokens/?tProyecto={ConfigAPI.TipoProyecto}&Metodo={"Select"}&parametros={Parametros}");
			HttpRequestHeader(request);
			var response = await client.SendAsync(request).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();
			if (response.IsSuccessStatusCode)
			{
				using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
				{
					if (response.StatusCode == HttpStatusCode.NotFound) return false;
					string resp = reader.ReadToEnd();
					DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
					foreach (DataRow r in dt.Rows)
					{
						//TODO: Authorization y Authentication tokenAPI
						Global.tokenAPI = r["TokenApi"].ToString();
						//TODO: Refresh tokenAPI
						Global.refreshTokenAPI = r["RefreshTokenApi"].ToString();
						//TODO Agregar Guid
						Global.guid = r["Guid"].ToString();
						Preferences.Default.Set("tokenAPI", Global.tokenAPI);
						Preferences.Default.Set("refreshTokenAPI", Global.refreshTokenAPI);
						//TODO Agregar Guid
						Preferences.Default.Set("Guid", Global.guid);
						Preferences.Default.Set("clave_usuario", Global.clave_usuario);
						Preferences.Default.Set("Password", Global.pass);
						if (Global.tokenAPI != "")
							break;
					}
					return true;
				}
			}
			return false;
		}
	}
}