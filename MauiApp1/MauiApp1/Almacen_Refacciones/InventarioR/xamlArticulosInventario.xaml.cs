using iAlmacen.Almacen_Refacciones.Herramientas_v2;
using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.InventarioR
{
    public partial class xamlArticulosInventario : ContentPage
    {
        //private ObservableCollection<RegArticulo> RegArticulos = new ObservableCollection<RegArticulo>();
        public ObservableCollection<Item_RegArticulo> Items { get; set; }
        public Command LoadItemsCommand_RegArticulo { get; set; }
        private ItemsViewModel_RegArticulo viewModel_RegArticulo;

        public xamlArticulosInventario()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            this.Title = "Articulos Del Inventario: " + Global.FolioInventario;

            Items = new ObservableCollection<Item_RegArticulo>();
            LoadItemsCommand_RegArticulo = new Command(async () => await cargar());
            BindingContext = viewModel_RegArticulo = new ItemsViewModel_RegArticulo($"{Global.FolioInventario}");
        }

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //CargarArticulos();
            viewModel_RegArticulo.LoadItemsCommand_regArticulo.Execute($"{Global.FolioInventario}");
        }

        //private void CargarArticulos()
        //{
        //    RegArticulos.Clear();
        //    string Parametros = $"{Global.FolioInventario}";
        //    HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_DetalleInventarioAlmacen");
        //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        //    {
        //        if (response.StatusCode == HttpStatusCode.NotFound) return;
        //        string resp = reader.ReadToEnd();
        //        if (resp == "[]") return;
        //        DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
        //        foreach (DataRow r in dt.Rows)
        //        {
        //            RegArticulos.Add(new RegArticulo
        //            {
        //                ID = int.Parse(r[0].ToString()),
        //                CodigoActual = r[1].ToString(),
        //                CodigoAnterior = "",
        //                Descripcion = r[2].ToString(),
        //                desc_familia = r[3].ToString(),
        //                desc_linea = r[4].ToString(),
        //                desc_grupo = r[5].ToString(),
        //                DescMarca = r[6].ToString(),
        //                DescMedida = r[7].ToString(),
        //                DescParte = r[8].ToString(),
        //                existencia = double.Parse(r[9].ToString()),
        //                UnidadControl = r[10].ToString(),
        //                Costo = double.Parse(r[11].ToString()),
        //                Fisico = double.Parse(r[13].ToString()),
        //                ClaveFamilia = r[14].ToString(),
        //                ClaveLinea = r[15].ToString(),
        //                ClaveGrupo = r[16].ToString(),
        //                Inventario = "0",
        //                Aplicado = "0",
        //                Fecha_ = DateTime.Now.ToShortDateString().ToString(),
        //                Seccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[18].ToString().Trim() : "",
        //                DescSeccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[19].ToString().Trim() : "",
        //                Pasillo = int.Parse(r[17].ToString().Trim()) == 1 ? r[20].ToString().Trim() : "",
        //                Estanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[21].ToString().Trim() : "",
        //                DescEstanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[22].ToString().Trim() : "",
        //                Nivel = int.Parse(r[17].ToString().Trim()) == 1 ? r[23].ToString().Trim() : "",
        //                Tarima = int.Parse(r[17].ToString().Trim()) == 1 ? r[24].ToString().Trim() : "",
        //                Contenedor = int.Parse(r[17].ToString().Trim()) == 1 ? r[25].ToString().Trim() : ""
        //            });
        //        }
        //    }
        //    ItemsListView.ItemsSource = RegArticulos;
        //}

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_RegArticulo;
            if (item == null)
                return;
            Global.ArticuloEnInventario = item;
            await Navigation.PushAsync(new CapturaInventario_v2());
        }
    }
}