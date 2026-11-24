using iAlmacen.Clases;
using iAlmacen.ViewModels;

using System.Net;

namespace iAlmacen.Views;

public partial class LoginView : ContentPage
{
	public Command LoadItemsCommand_Login { get; set; }
	private LoginViewModel viewModel_Login;

	private readonly TaskCompletionSource<bool>? _tcs;

	// Modo modal: se muestra sobre la pagina actual y al autenticarse regresa a ella.
	public LoginView(TaskCompletionSource<bool> tcs) : this()
	{
		_tcs = tcs;
	}

	protected override bool OnBackButtonPressed() => _tcs != null || base.OnBackButtonPressed();

	public LoginView()
    {
        InitializeComponent();
        txt_user.Text = "";
        txt_pass.Text = "";
		//Global.clave_usuario = "";
		//Global.pass = "";
		//Global.tokenAPI = "";
		//Global.refreshTokenAPI = "";
		//Global.guid = "";
		//Preferences.Default.Set("tokenAPI", Global.tokenAPI);
		//Preferences.Default.Set("refreshTokenAPI", Global.refreshTokenAPI);
		////TODO Agregar Guid
		//Preferences.Default.Set("Guid", Global.guid);
		//Preferences.Default.Set("clave_usuario", Global.clave_usuario);
		//Preferences.Default.Set("Password", Global.pass);
		txt_user.Focus();
		BindingContext = viewModel_Login = new LoginViewModel();
		Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        while (!txt_user.Focus()) { await Task.Delay(50); }
		viewModel_Login.CheckCameraPermissionCommand.Execute(null);
		viewModel_Login.CheckPhotosPermissionCommand.Execute(null);
		viewModel_Login.CheckPhotosAddOnlyPermissionCommand.Execute(null);
		viewModel_Login.CheckConnectedCommand.Execute(null);
	}

	private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
	{
		if (e.NetworkAccess == NetworkAccess.None)
			DisplayAlertAsync("Sin conexión", "No se detecta conexión a internet. Algunas funciones pueden no estar disponibles.", "OK");
		else
			DisplayAlertAsync("Conexión restablecida", "Se ha restablecido la conexión a internet.", "OK");
	}

	private async void btnIniciarSesion_Clicked(object sender, EventArgs e)
    {
		btnIniciarSesion.IsEnabled = false;
        if (string.IsNullOrEmpty(txt_user.Text) || string.IsNullOrEmpty(txt_pass.Text))
        {
            await DisplayAlertAsync("Advertencia", "Falta ingresar usuario o contraseña", "OK");
            return;
        }
        if (Funciones.ChkConnected())
        {
            try
            {
				HttpStatusCode httpStatusCode = await Funciones.Login(txt_user.Text.ToLower(), txt_pass.Text.ToLower());
				if (httpStatusCode != HttpStatusCode.OK)
				{
					if (httpStatusCode == HttpStatusCode.Unauthorized)
					{
						Global.nombre_usuario = Global.nombre_usuario.Trim();
						if (Global.nombre_usuario == "")
						{
							await DisplayAlertAsync("Advertencia", "Acceso Invalido", "OK");
							txt_user.Text = "";
							txt_pass.Text = "";
							btnIniciarSesion.IsEnabled = true;
							return;
						}
					}
					else if (httpStatusCode == HttpStatusCode.NotFound)
					{
						await DisplayAlertAsync("Advertencia", "Acceso Invalido", "OK");
						txt_user.Text = "";
						txt_pass.Text = "";
						btnIniciarSesion.IsEnabled = true;
						return;
					}
					else
					{
						if (Global.cierra_sistema == false)
						{
							await DisplayAlertAsync("Error", httpStatusCode.ToString(), "OK");
							txt_user.Text = "";
							txt_pass.Text = "";
							btnIniciarSesion.IsEnabled = true;
							return;
						}
						else
						{
							System.Threading.Thread.CurrentThread.Abort();
						}
					}
				}

				Preferences.Set("logueado", "si");
				if (_tcs != null)
				{
					await Navigation.PopModalAsync();
					_tcs.TrySetResult(true);
					return;
				}
				Application.Current.MainPage = new AppShell();
			}
            catch (Exception ex)
            {
				btnIniciarSesion.IsEnabled = true;
				return;
            }
        }
    }

	private void chkAutoRefreshToken_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Global.isAutoRefreshToken = chkAutoRefreshToken.IsChecked;
		//viewModel_Login.AutoRefreshToken = chkAutoRefreshToken.IsChecked;
	}
}