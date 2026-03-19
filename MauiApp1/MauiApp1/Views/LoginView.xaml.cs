using iAlmacen.Clases;
using System.Net;

namespace iAlmacen.Views;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        InitializeComponent();
        txt_user.Text = "";
        txt_pass.Text = "";
        txt_user.Focus();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        while (!txt_user.Focus()) { await Task.Delay(50); }
    }

    private async void btnIniciarSesion_Clicked(object sender, EventArgs e)
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

        //var detailPage = new Window(new AppShell());
        //await Navigation.PushAsync(new MainView());
        //await Shell.Current.GoToAsync("mainview");
    }
}