using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen
{
    public partial class cv_Almacen_Entradas_OCompra : ContentPage
    {
        private Item_entrada_vigilancia item_rechazar_;
        public ObservableCollection<Item_entrada_vigilancia> Items { get; set; }
        public Command LoadItemsCommand_Vigilancia { get; set; }
        private ItemsViewModel_Vigilancia viewModel_Vigilancia;

        public cv_Almacen_Entradas_OCompra()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            Global.cfiltro_ = "V";

            Items = new ObservableCollection<Item_entrada_vigilancia>();
            LoadItemsCommand_Vigilancia = new Command(async () => await cargar());
            BindingContext = viewModel_Vigilancia = new ItemsViewModel_Vigilancia();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_Vigilancia.LoadItemsCommand_vigilancia.Execute(null);
            //ItemsListView.SelectedItem = null;
            buscar_folio_entrada();
        }

        private bool ValidarFolio(string folio_orden)
        {
            bool existe = true;

            string Parametros = $"{folio_orden},V";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_compra");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) existe = false;
                string resp = reader.ReadToEnd();
                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);

                foreach (DataRow r in dt.Rows)
                {
                    if (r[1].ToString() == "0")
                    {
                        DisplayAlertAsync("Advertencia", "Folio Incorrecto", "OK");
                        existe = false;
                        break;
                    }

                    if (r[2].ToString() == "1")
                    {
                        DisplayAlertAsync("Advertencia", "Folio Cancelado", "OK");
                        existe = false;
                        break;
                    }

                    if (r[1].ToString() == "OC")
                    {
                        DisplayAlertAsync("Advertencia", "Folio Cancelado", "OK");
                        existe = false;
                        break;
                    }
                    if (Boolean.TryParse(r[3]?.ToString(), out bool val) == true)
                    {
                        DisplayAlertAsync("Advertencia", "Folio ya se encuentra Surtido en su totalidad", "OK");
                        existe = false;
                        break;
                    }
                }
            }

            return existe;
        }

        private async void clicked_folio_manual(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Informacion", "Indique el Folio de Orden de Compra", placeholder: "", maxLength: 7, keyboard: Keyboard.Numeric);
            if (!string.IsNullOrEmpty(result))
            {
                string folio_orden = result.PadLeft(7, '0');
                if (ValidarFolio(folio_orden))
                {
                    Global.folio_orden_ = folio_orden;
                    await Navigation.PushAsync(new Page_Head_OrdenCompra());
                }
            }
        }

        private async Task cargar()
        { }

        private void buscar_folio_entrada()
        {
            string Parametros = $"0,F";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_compra");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        Global.folio_entrada_ = int.Parse(r[0].ToString());
                    }
                }

                if (Global.folio_entrada_ == 0)
                {
                    DisplayAlertAsync("Advertencia", "Error al buscar el folio entrada", "OK");
                    return;
                }
            }
        }

        private void Actualizar_lista_event(object sender, EventArgs e)
        {
            viewModel_Vigilancia.LoadItemsCommand_vigilancia.Execute(null);
            //ItemsListView.SelectedItem = null;
        }

        private async void OnCancel_entrada(object sender, EventArgs e)
        {
            item_rechazar_ = (sender as MenuItem).BindingContext as Item_entrada_vigilancia;
            var item = item_rechazar_?.folio_orden_;
            if (item == null)
                return;

            var answer = await DisplayAlertAsync("Informacion?", "Desea cancelar la Recepción del producto de la orden seleccionada", "Si", "No");
            if (answer == false)
                return;

            //TODO UIAlertView With Dismissed and AlertViewStyle
            string result = await DisplayPromptAsync("Motivo de Cancelacion", "Es necesario indicar el concepto del rechazo de los articulos.", placeholder: "", maxLength: 100, keyboard: Keyboard.Plain);
            if (!string.IsNullOrEmpty(result))
            {
                string concepto_ = result;
                string Parametros = $"{item_rechazar_?.folio_orden_},{Global.nombre_usuario},C,{int.Parse(item_rechazar_.id_.ToString())},{concepto_}";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_cancelar_vigilancia");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        //TODO UIAlertView
                        await DisplayAlertAsync("Advertencia", "Error al cancelar la recepción", "OK");
                        return;
                    }
                }
                viewModel_Vigilancia.LoadItemsCommand_vigilancia.Execute(null);
                //ItemsListView.SelectedItem = null;
            }
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_entrada_vigilancia;
            if (item == null)
                return;
            if (ValidarFolio(item.folio_orden_))
            {
                Global.folio_orden_ = item.folio_orden_;
                await Navigation.PushAsync(new Page_Head_OrdenCompra());
            }
        }
    }
}