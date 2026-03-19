using iAlmacen.Clases;
using iAlmacen.Views;

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
            if (string.IsNullOrEmpty(logueado) && string.IsNullOrEmpty(Global.tokenAPI) && string.IsNullOrEmpty(Global.refreshTokenAPI))
            {
                MainPage = new LoginView();
            }
            else
            {
                MainPage = new AppShell();
            }
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    //return new Window(new AppShell());
        //    return new Window(new LoginView());
        //}
    }
}