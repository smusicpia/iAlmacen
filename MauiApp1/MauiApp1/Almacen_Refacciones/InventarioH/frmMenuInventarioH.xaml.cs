namespace iAlmacen.Almacen_Refacciones.InventarioH
{
    public partial class frmMenuInventarioH : ContentPage
    {
        public frmMenuInventarioH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
        }

        private async void btnNuevaPlantilla_Clicked(Object sender, EventArgs e)      //CREAR PLANTILLA NUEVA
        {
            await Navigation.PushAsync(new frmCapturaPlantillaH());
        }

        private async void btnCaptura_Clicked(Object sender, EventArgs e)     //CAPTURAR INVENTARIO
        {
            //await Navigation.PushAsync(new frmInventariosDisponiblesH());
        }

        private async void btnAplicacion_Clicked(Object sender, EventArgs e)      //APLICAR INVENTARIO
        {
            //await Navigation.PushAsync(new frmInventariosCerradosH());
        }
    }
}