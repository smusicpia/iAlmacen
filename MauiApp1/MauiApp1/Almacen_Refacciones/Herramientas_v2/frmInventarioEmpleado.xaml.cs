using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmInventarioEmpleado : ContentPage
    {
        public ObservableCollection<Item_ArticuloEnResguardo> Items { get; set; }
        public Command LoadItemsCommand_articuloenresguardo { get; set; }
        private ItemsViewModel_ArticuloEnResguardo viewModel_ArticuloEnResguardo;

        //private ObservableCollection<clsArticuloEnResguardo> RegArticulos = new ObservableCollection<clsArticuloEnResguardo>();
        private async Task cargar()
        { }

        public frmInventarioEmpleado()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            this.Title = "Articulos en Resguardo: " + Global.ResgEmpleado.articulos.ToString();

            Items = new ObservableCollection<Item_ArticuloEnResguardo>();
            LoadItemsCommand_articuloenresguardo = new Command(async () => await cargar());
            BindingContext = viewModel_ArticuloEnResguardo = new ItemsViewModel_ArticuloEnResguardo();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_ArticuloEnResguardo.LoadItemsCommand_articuloenresguardo.Execute(null);
        }

        private async void btnExtraviado_Clicked(Object sender, EventArgs e)
        {
            Item_ArticuloEnResguardo Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ArticuloEnResguardo;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "¿Desea marcar como EXTRAVIADO la herramienta seleccionada?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = "fechainventario=getdate(),inventario=1,condicion='Extraviado',aplicado=0,cerrado=0";
            string Condicion = $"id='{Item_.id}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "detalle_salidas_resguardo_herramientas", Condicion, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
            viewModel_ArticuloEnResguardo.LoadItemsCommand_articuloenresguardo.Execute(null);
        }

        private async void btnDevolucion_Clicked(Object sender, EventArgs e)
        {
            Item_ArticuloEnResguardo Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ArticuloEnResguardo;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "¿Desea marcar como DEVOLUCION la herramienta seleccionada?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = "fechainventario=getdate(),inventario=1,condicion='Devolucion',aplicado=0,cerrado=0";
            string Condicion = $"id='{Item_.id}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "detalle_salidas_resguardo_herramientas", Condicion, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
            viewModel_ArticuloEnResguardo.LoadItemsCommand_articuloenresguardo.Execute(null);
        }

        private async void btnResguardo_Clicked(Object sender, EventArgs e)
        {
            Item_ArticuloEnResguardo Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ArticuloEnResguardo;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "¿Desea marcar como EN RESGUARDO la herramienta seleccionada?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = "fechainventario=getdate(),inventario=1,condicion='Resguardo',aplicado=0,cerrado=0 ";
            string Condicion = $"id='{Item_.id}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "detalle_salidas_resguardo_herramientas", Condicion, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
            viewModel_ArticuloEnResguardo.LoadItemsCommand_articuloenresguardo.Execute(null);
        }
    }
}