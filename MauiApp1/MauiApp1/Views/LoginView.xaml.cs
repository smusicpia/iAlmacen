using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;

using System.Net;

namespace iAlmacen.Views;

public partial class LoginView : ContentPage
{
	public Command LoadItemsCommand_Login { get; set; }
	private LoginViewModel viewModel_Login;

	public LoginView()
    {
        InitializeComponent();
        txt_user.Text = "";
        txt_pass.Text = "";
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
        if (string.IsNullOrEmpty(txt_user.Text) || string.IsNullOrEmpty(txt_pass.Text))
        {
            await DisplayAlertAsync("Advertencia", "Falta ingresar usuario o contraseña", "OK");
            return;
        }
        if (Funciones.ChkConnected())
        {
            HttpStatusCode httpStatusCode = Funciones.Login(txt_user.Text.ToLower(), txt_pass.Text.ToLower());
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
                        return;
                    }
                }
                else if (httpStatusCode == HttpStatusCode.NotFound)
                {
                    await DisplayAlertAsync("Advertencia", "Acceso Invalido", "OK");
                    txt_user.Text = "";
                    txt_pass.Text = "";
                    return;
                }
                else
                {
                    if (Global.cierra_sistema == false)
                    {
                        await DisplayAlertAsync("Error", httpStatusCode.ToString(), "OK");
                        txt_user.Text = "";
                        txt_pass.Text = "";
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.Abort();
                    }
                }
            }

            Preferences.Set("logueado", "si");
            Application.Current.MainPage = new AppShell();
        }
    }
}