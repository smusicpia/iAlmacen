using iAlmacen.Views;

using System.Net;

namespace iAlmacen.Clases
{
	internal class UnauthorizeInterceptorHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			//Optional: Agregar el Token a la cabecera aqui si lo tienes guardado en algun lugar, por ejemplo:
			//var token = await SecureStorage.GetAsync("auth_token");
			var token = Global.tokenAPI;
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			}

			var response = base.SendAsync(request, cancellationToken);

			// Manejar la respuesta 401 aquí, por ejemplo, redirigir al usuario a la pantalla de inicio de sesión
			// Puedes usar un evento o una acción para notificar a la aplicación sobre la necesidad de autenticación
			if (response.Result.StatusCode == HttpStatusCode.Unauthorized)
			{
				//1. Limpiar los topkens expirados
				Global.tokenAPI = string.Empty;
				Global.refreshTokenAPI = string.Empty;
				Global.guid = string.Empty;
				Preferences.Default.Set("tokenAPI", string.Empty);
				Preferences.Default.Set("refreshTokenAPI", string.Empty);
				Preferences.Default.Set("Guid", string.Empty);
				Preferences.Default.Set("clave_usuario", string.Empty);
				Preferences.Default.Set("Password", string.Empty);

				//2. Ejecutar la redireccion al login en el hilo principal de la aplicación
				MainThread.BeginInvokeOnMainThread(() =>
				{
					// Aquí puedes usar tu lógica de navegación para redirigir al usuario a la pantalla de inicio de sesión
					// Por ejemplo, si estás usando Shell:
					App.Current.MainPage = new LoginView();
				});
				//OnUnauthorized?.Invoke(this, EventArgs.Empty);
			}
			return response;
		}

		public event EventHandler OnUnauthorized;
	}
}
