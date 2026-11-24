using iAlmacen.Models;
using iAlmacen.Views;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Data;
using System.Net;

namespace iAlmacen.Clases
{
	public static class SesionApi
	{
		private static readonly SemaphoreSlim _semaforo = new(1, 1);

		public static readonly string BaseAddress = HttpClienteInstancia.Client.BaseAddress.ToString();

		// GET con reautenticacion: ante 401 intenta refrescar tokens, luego pide login modal y reintenta una vez.
		public static async Task<HttpResponseMessage> GetAsync(string url)
		{
			try
			{
				var response = await HttpClienteInstancia.Client.GetAsync(url);
				if (response.StatusCode != HttpStatusCode.Unauthorized)
					return response;

				if (!await RenovarOLoginAsync())
					return response;

				response.Dispose();
				return await HttpClienteInstancia.Client.GetAsync(url);
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public static async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
		{
			var response = await HttpClienteInstancia.Client.PutAsync(url, content);
			if (response.StatusCode != HttpStatusCode.Unauthorized)
				return response;

			if (!await RenovarOLoginAsync())
				return response;

			response.Dispose();
			return await HttpClienteInstancia.Client.PutAsync(url, content);
		}

		public static async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
		{
			var response = await HttpClienteInstancia.Client.PostAsync(url, content);
			if (response.StatusCode != HttpStatusCode.Unauthorized)
				return response;

			if (!await RenovarOLoginAsync())
				return response;

			response.Dispose();
			return await HttpClienteInstancia.Client.PostAsync(url, content);
		}

		public static async Task<HttpResponseMessage> DeleteAsync(string url, HttpContent content)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, url);
			request.Content = content;
			var response = await HttpClienteInstancia.Client.SendAsync(request);
			if (response.StatusCode != HttpStatusCode.Unauthorized)
				return response;

			if (!await RenovarOLoginAsync())
				return response;

			response.Dispose();
			return await HttpClienteInstancia.Client.SendAsync(request);
		}

		private static async Task<bool> RenovarOLoginAsync()
		{
			string tokenAnterior = Global.tokenAPI;
			await _semaforo.WaitAsync();
			try
			{
				// Otra peticion ya renovo la sesion mientras esperabamos.
				if (Global.tokenAPI != tokenAnterior && !string.IsNullOrEmpty(Global.tokenAPI))
					return true;

				if (await RenovarTokensAsync())
					return true;

				return await MostrarLoginModalAsync();
			}
			finally
			{
				_semaforo.Release();
			}
		}

		private static Task<bool> MostrarLoginModalAsync()
		{
			var tcs = new TaskCompletionSource<bool>();
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				try
				{
					await Shell.Current.Navigation.PushModalAsync(new LoginView(tcs));
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex);
					tcs.TrySetResult(false);
				}
			});
			return tcs.Task;
		}

		public static async Task<bool> RenovarTokensAsync()
		{
			try
			{
				if (string.IsNullOrEmpty(Global.refreshTokenAPI)) return false;

				UserLogin usuario = new UserLogin();
				usuario.Username = Convert.ToBase64String(SecurityManager.Encrypt(Global.clave_usuario, Global.Key, Global.IV));
				usuario.Password = Global.pass;
				HttpClient client = HttpClienteInstancia.ClientLogin;
				HttpResponseMessage response;
				try
				{
					response = await client.PostAsync($"api/login/Authenticate?tProyecto={ConfigAPI.TipoProyecto}", Funciones.jsonContent(usuario));
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Refresh fallo ({client.BaseAddress}): {ex}");
					return false;
				}

				if (!response.IsSuccessStatusCode) return false;

				string resp = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				DataTable dt = JsonConvert.DeserializeObject<DataTable>(resp);
				foreach (DataRow r in dt.Rows)
				{
					Global.tokenAPI = r["TokenApi"].ToString();
					Global.refreshTokenAPI = r["RefreshTokenApi"].ToString();
					Global.guid = r["Guid"].ToString();
					Preferences.Default.Set("tokenAPI", Global.tokenAPI);
					Preferences.Default.Set("refreshTokenAPI", Global.refreshTokenAPI);
					Preferences.Default.Set("Guid", Global.guid);
					Preferences.Default.Set("clave_usuario", Global.clave_usuario);
					Preferences.Default.Set("Password", Global.pass);
					if (Global.tokenAPI != "")
						break;
				}
				return !string.IsNullOrEmpty(Global.tokenAPI);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Refresh fallo: {ex.Message}");
				return false;
			}
		}
	}
}