using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using System.Collections.ObjectModel;

namespace iAlmacen.Almacen_Refacciones.Entrada_Almacen;

public partial class almacen_entrada_referencia : ContentPage
{
    public ObservableCollection<Item_entrada_vigilancia> Items { get; set; }
    public Command LoadItemsCommand_Vigilancia { get; set; }
    private ItemsViewModel_Vigilancia viewModel_Vigilancia;

    public almacen_entrada_referencia()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Atras");
        Global.cfiltro_ = "R";

        Items = new ObservableCollection<Item_entrada_vigilancia>();
        LoadItemsCommand_Vigilancia = new Command(async () => await cargar());
        BindingContext = viewModel_Vigilancia = new ItemsViewModel_Vigilancia();
    }

    private async Task cargar()
    { }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        viewModel_Vigilancia.LoadItemsCommand_vigilancia.Execute(null);
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as Item_entrada_vigilancia;
        if (item == null)
            return;

        Global.folio_orden_ = item.folio_orden_;
        Global.cidsql_ = int.Parse(item.id_.ToString());
        Global.folio_entrada_ = int.Parse(item.captura_.ToString());
        await Navigation.PushAsync(new Page_Head_OrdenCompra());
    }

    private void clicked_folio_manual(object sender, EventArgs e)
    {
    }
}