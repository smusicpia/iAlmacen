namespace iAlmacen.Views;

public partial class MainView : ContentPage
{
    public MainView()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Atras");
    }

    private async void EntradaA_Clicked(object sender, EventArgs e)      //Entradas Almacen
    {
        //await Navigation.PushAsync(new almacen_entradas_compra());  //cv_Almacen_Entradas_OCompra
        await Navigation.PushAsync(new cv_Almacen_Entradas_OCompra());  //cv_Almacen_Entradas_OCompra
    }

    private async void SalidaA_Clicked(Object sender, EventArgs e)     //Salidas Almacen
    {
        //await Navigation.PushAsync(new frmMenuSolicitudes());
    }

    private async void entradas_remision_Clicked(object sender, EventArgs e)     //Entradas Sin Factura
    {
        //await Navigation.PushAsync(new almacen_entrada_referencia());
    }

    private async void inventario_reposicion_Clicked(object sender, EventArgs e)      //Inventario Reposicion
    {
        //await Navigation.PushAsync(new inventario_reposicion());
    }

    private async void invHerramientas_Clicked(object sender, EventArgs e)      //Herramientas
    {
        //await Navigation.PushAsync(new frmMenuHerramientas());
    }

    private async void invArticulos_Clicked(object sender, EventArgs e)      //Inventario Refaccion
    {
        //await Navigation.PushAsync(new InvRefaccion());
    }

    private async void btn_salidas(object sender, EventArgs e)       //
    {
        //await Navigation.PushAsync(new Almacen_Salidas());
    }
}