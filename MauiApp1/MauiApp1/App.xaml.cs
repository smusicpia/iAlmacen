using iAlmacen.Clases;
using iAlmacen.Views;
using System.Net;

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
            Global.clave_usuario = Preferences.Default.Get("clave_usuario", string.Empty);
            Global.pass = Preferences.Default.Get("Password", string.Empty);
            if (string.IsNullOrEmpty(Global.clave_usuario) || (string.IsNullOrEmpty(logueado) && string.IsNullOrEmpty(Global.tokenAPI) && string.IsNullOrEmpty(Global.refreshTokenAPI)))
            {
                MainPage = new LoginView();
            }
            else
            {
                try
                {
                    HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        MainPage = new LoginView();

                    }
                    MainPage = new AppShell();
                }
                catch (Exception)
                {
                    MainPage = new LoginView();
                }
            }
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    //return new Window(new AppShell());
        //    return new Window(new LoginView());
        //}
    }
}