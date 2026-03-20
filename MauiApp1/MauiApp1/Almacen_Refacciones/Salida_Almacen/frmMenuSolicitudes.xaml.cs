namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class frmMenuSolicitudes : ContentPage
{
    public frmMenuSolicitudes()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Atras");
    }

    private async void Solicitudes_Clicked(Object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmSubmenuSolicitudes());
    }

    private async void Salidas_Clicked(Object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Almacen_Salidas());
    }
}