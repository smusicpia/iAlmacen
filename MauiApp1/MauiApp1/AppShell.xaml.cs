using iAlmacen.Views;

namespace iAlmacen
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register all router
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(MainView), typeof(MainView));
            //Routing.RegisterRoute("almacen_entradas_compra", typeof(almacen_entradas_compra));
            Routing.RegisterRoute(nameof(cv_Almacen_Entradas_OCompra), typeof(cv_Almacen_Entradas_OCompra));
            Routing.RegisterRoute(nameof(Page_Head_OrdenCompra), typeof(Page_Head_OrdenCompra));
            Routing.RegisterRoute(nameof(Page_Detail_OrdenCompra), typeof(Page_Detail_OrdenCompra));
		}

		private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Cerrar sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí, continuar", "No, volver");
            if (answer)
            {
                Preferences.Remove("logueado", string.Empty);
                Preferences.Default.Remove("tokenAPI", string.Empty);
                Preferences.Default.Remove("refreshTokenAPI", string.Empty);
                Application.Current.MainPage = new NavigationPage(new LoginView());
                //await Shell.Current.GoToAsync("loginview");
			}
        }
    }
}