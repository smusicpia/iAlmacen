using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.InventarioH
{
    public partial class frmInventariosDisponiblesH : ContentPage
    {
        //private ObservableCollection<clsInventario> RegInventarios = new ObservableCollection<clsInventario>();
        public ObservableCollection<Item_Inventario> Items { get; set; }

        public Command LoadItemsCommand_Inventario { get; set; }
        private ItemsViewModel_Inventario viewModel_Inventario;

        public frmInventariosDisponiblesH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");

            Items = new ObservableCollection<Item_Inventario>();
            LoadItemsCommand_Inventario = new Command(async () => await cargar());
            BindingContext = viewModel_Inventario = new ItemsViewModel_Inventario($"{Global.clave_usuario},false,true");
        }

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_Inventario.LoadItemsCommand_inventario.Execute($"{Global.clave_usuario},false,true");
        }

        private async void btnVerificar_Clicked(Object sender, EventArgs e)
        {
            Item_Inventario Item_ = (sender as MenuItem).BindingContext as Item_Inventario;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "Desea cerrar el inventario seleccionado ¿Desea Continuar?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = "Cerrado = 1, Capturado = 1";
            string Condicion = $"FolioInventario='{Item_.Folio}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "InventarioAlmacen", Condicion, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }

            if (sResponce == "OK")
            {
                await DisplayAlertAsync("Informacion", "Inventario cerrado correctamente", "OK");
                //CargarInventariosDisponibles();
                Items.Clear();
            }
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_Inventario;
            if (item == null)
                return;
            Global.FolioInventario = item.Folio;
            await Navigation.PushAsync(new frmArticulosInventarioH());
        }
    }
}