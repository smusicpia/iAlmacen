namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmMenuCapturasH : ContentPage
    {
        public frmMenuCapturasH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
        }

        private async void btnCaptura_Clicked(Object sender, EventArgs e)    //UBICACIONES Y EXISTENCIAS (CONSULTAS)
        {
            await Navigation.PushAsync(new frmConsultaH());
        }

        private async void btnAplicacion_Clicked(Object sender, EventArgs e)    //RESGUARDOS DE EMPLEADOS (CAPTURAS)
        {
            //await Navigation.PushAsync(new frmResguardosEmpleados());
        }

        private async void btnAplica_Clicked(Object sender, EventArgs e)    //APLICAR INVENTARIO (APLICACION)
        {
            //await Navigation.PushAsync(new frmResguardosEmpleadosCerrados());
        }
    }
}