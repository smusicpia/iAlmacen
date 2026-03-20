using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using System.Collections.ObjectModel;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class frmListaOrdenesRecoleccion : ContentPage
{
    private Item_Virtual_Recoleccion item_rechazar_;
    private string Origen_;
    public ObservableCollection<Item_Virtual_Recoleccion> Items { get; set; }
    public Command LoadItemsCommand_Recoleccion { get; set; }
    private ItemsViewModel_Recoleccion viewModel_Recoleccion;

    public frmListaOrdenesRecoleccion(string origen)
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Atras");
        //Clases.Global.cfiltro_ = "V";

        Origen_ = origen;
        Items = new ObservableCollection<Item_Virtual_Recoleccion>();
        LoadItemsCommand_Recoleccion = new Command(async () => await cargar());
        switch (Origen_)
        {
            case "N":
                this.Title += " (Nuevos)";
                BindingContext = viewModel_Recoleccion = new ItemsViewModel_Recoleccion("", "'SP'");
                break;

            case "RL":
                this.Title += " (Por Entregar)";
                BindingContext = viewModel_Recoleccion = new ItemsViewModel_Recoleccion("", "'SL','SI'");
                break;

            case "H":
                this.Title += " (Historial)";
                BindingContext = viewModel_Recoleccion = new ItemsViewModel_Recoleccion("", "'SX','SR'", true);
                break;
        }
    }

    private async Task cargar()
    { }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Global.regArticulosSalida != null)
        {
            if (Global.regArticulosSalida.Count > 0)
            {
                Global.regArticulosSalida.Clear();
            }
        }
        viewModel_Recoleccion.LoadItemsCommand_recoleccion.Execute(null);
    }

    private void ListItems_Refreshing(object sender, EventArgs e)
    {
        viewModel_Recoleccion.LoadItemsCommand_recoleccion.Execute(null);
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
        if (Origen_ == "N")
        {
            var item = args.SelectedItem as Item_Virtual_Recoleccion;
            if (item == null)
                return;

            Global.folio_orden_ = item.folio_orden_;
            Global.cidsql_ = int.Parse(item.id_.ToString());
            Global.folio_requisicion_ = item.folio_requisicion_.ToString();
            Global.folio_cotizacion_ = item.folio_cotizacion_.ToString();
            await Navigation.PushAsync(new Orden_Recoleccion());
        }
        else if (Origen_ == "RL" || Origen_ == "H")
        {
            var item = args.SelectedItem as Item_Virtual_Recoleccion;
            if (item == null)
                return;

            Global.folio_orden_ = item.folio_orden_;
            Global.cidsql_ = int.Parse(item.id_.ToString());
            Global.folio_requisicion_ = item.folio_requisicion_.ToString();
            Global.folio_cotizacion_ = item.folio_cotizacion_.ToString();
            if (Origen_ == "RL")
            {
                await Navigation.PushAsync(new Salida_Almacen.Almacen_Salidas(true));
            }
            else if (Origen_ == "H")
            {
                await Navigation.PushAsync(new Salida_Almacen.Almacen_Salidas(true, true));
            }
        }
    }

    private async void OnCancel_entrada(object sender, EventArgs e)
    {
        if (Origen_ == "H")
            return;
        item_rechazar_ = (sender as MenuItem).BindingContext as Item_Virtual_Recoleccion;

        var item = item_rechazar_.folio_orden_;
        if (item == null)
            return;

        var answer = await DisplayAlertAsync("Informacion?", "Desea cancelar la Orden de Recoleccion seleccionada", "Si", "No");
        if (answer == false)
            return;

        string sResponce;
        string Parametros = "StatusPedido='SX'";
        string Condicion = $"id='{item_rechazar_.id_}' and FolioOrden='{item_rechazar_.folio_orden_}'";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "OrdenRecoleccion", Condicion, "UPDATE");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                sResponce = "OK";
            }
        }

        Parametros = "status_requisicion='SX',status_orden_compra='SX',status_cotizacion='SX',status_OrdenRecoleccion='SX'";
        Condicion = $"folio_OrdenRecoleccion='{item_rechazar_.id_}' and folio_orden_compra='{item_rechazar_.folio_orden_}' and folio_requisicion='{item_rechazar_.folio_requisicion_}' and folio_cotizacion={item_rechazar_.folio_cotizacion_}";
        response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "Requisiciones_En_Cotizacion", Condicion, "UPDATE");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Clases.Global.cmodo_prueba = true;
            }
        }

        string result = await DisplayPromptAsync("Motivo de Cancelacion", "Es necesario indicar el concepto de cancelacion de la orden de Recoleccion.", "OK", null, "Motivo de cancelacion", -1);
        if (!string.IsNullOrEmpty(result))
        {
            Rechazar_OrdenRecoleccion(result);
        }
    }

    private void Rechazar_OrdenRecoleccion(string Concepto)
    {
        string concepto_ = Concepto;

        viewModel_Recoleccion.LoadItemsCommand_recoleccion.Execute(null);
        //ItemsListView.SelectedItem = null;
    }

    private void Actualizar_lista_event(object sender, EventArgs e)
    {
        viewModel_Recoleccion.LoadItemsCommand_recoleccion.Execute(null);
        //ItemsListView.SelectedItem = null;
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Origen_ == "N")
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_Virtual_Recoleccion;
            if (item == null)
                return;

            Global.folio_orden_ = item.folio_orden_;
            Global.cidsql_ = int.Parse(item.id_.ToString());
            Global.folio_requisicion_ = item.folio_requisicion_.ToString();
            Global.folio_cotizacion_ = item.folio_cotizacion_.ToString();
            //await Navigation.PushAsync(new iAlmacen.Almacen_Refacciones.Salida_Almacen.Orden_Recoleccion());
        }
        else if (Origen_ == "RL" || Origen_ == "H")
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_Virtual_Recoleccion;
            if (item == null)
                return;

            Global.folio_orden_ = item.folio_orden_;
            Global.cidsql_ = int.Parse(item.id_.ToString());
            Global.folio_requisicion_ = item.folio_requisicion_.ToString();
            Global.folio_cotizacion_ = item.folio_cotizacion_.ToString();
            if (Origen_ == "RL")
            {
                //await Navigation.PushAsync(new iAlmacen.Almacen_Refacciones.Salida_Almacen.Almacen_Salidas(true));
            }
            else if (Origen_ == "H")
            {
                //await Navigation.PushAsync(new iAlmacen.Almacen_Refacciones.Salida_Almacen.Almacen_Salidas(true, true));
            }
        }
    }
}