namespace iAlmacen
{
    public partial class almacen_orden_compra : ContentPage
    {
        public almacen_orden_compra()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "Atras");

            //MasterBehavior = MasterBehavior.Popover;
        }
    }
}