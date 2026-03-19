using iAlmacen.Views;

namespace iAlmacen
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Register all router
            Routing.RegisterRoute("loginview", typeof(LoginView));
            Routing.RegisterRoute("mainview", typeof(MainView));
            //Routing.RegisterRoute("almacen_entradas_compra", typeof(almacen_entradas_compra));
            Routing.RegisterRoute("cv_almacen_entradas_ocompra", typeof(cv_Almacen_Entradas_OCompra));
            Routing.RegisterRoute("page_head_ordencompra", typeof(Page_Head_OrdenCompra));
            Routing.RegisterRoute("page_detail_ordencompra", typeof(Page_Detail_OrdenCompra));
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Cerrar sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí, continuar", "No, volver");
            if (answer)
            {
                Preferences.Remove("logueado", string.Empty);
                Preferences.Default.Remove("tokenAPI", string.Empty);
                Preferences.Default.Remove("refreshTokenAPI", string.Empty);
                Application.Current.MainPage = new LoginView();
            }
        }
    }
}