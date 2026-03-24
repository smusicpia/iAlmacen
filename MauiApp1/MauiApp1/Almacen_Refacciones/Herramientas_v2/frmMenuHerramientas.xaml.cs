using iAlmacen.Almacen_Refacciones.InventarioH;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmMenuHerramientas : ContentPage
    {
        public frmMenuHerramientas()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
        }

        private async void ImageButton_Clicked(Object sender, EventArgs e)      //HERRAMIENTAS
        {
            await Navigation.PushAsync(new frmMenuCapturasH());
        }

        private async void btnInventario_Clicked(Object sender, EventArgs e)      //INVENTARIO
        {
            await Navigation.PushAsync(new frmMenuInventarioH());
        }
    }
}