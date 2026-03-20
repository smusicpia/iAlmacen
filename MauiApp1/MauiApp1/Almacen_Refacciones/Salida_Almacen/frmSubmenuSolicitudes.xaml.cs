namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class frmSubmenuSolicitudes : ContentPage
{
    public frmSubmenuSolicitudes()
    {
        InitializeComponent();
    }

    private async void NuevaSolicitudR_Clicked(Object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmListaOrdenesRecoleccion("N"));
    }

    private async void SoicitudesxEntregar_Clicked(Object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmListaOrdenesRecoleccion("RL"));
    }

    private async void HistorialR_Clicked(Object sender, EventArgs e)
    {
        await Navigation.PushAsync(new frmListaOrdenesRecoleccion("H"));
    }
}