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
    public partial class xamlArticulosCerrados : ContentPage
    {
        //private ObservableCollection<RegArticulo> RegArticulos = new ObservableCollection<RegArticulo>();

        public ObservableCollection<Item_RegArticulo> Items { get; set; }
        public Command LoadItemsCommand_RegArticulo { get; set; }
        private ItemsViewModel_RegArticulo viewModel_RegArticulo;

        private int cnivel_limite = 1;

        public xamlArticulosCerrados()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            this.Title = "Articulos Del Inventario Capturado: " + Global.FolioInventario;

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
        //                Fecha_ = DateTime.Now.ToShortDateString().ToString()
        //            });
        //        }
        //    }
        //    ItemsListView.ItemsSource = RegArticulos;
        //}

        private async void btnGuardar_Clicked(Object sender, EventArgs e)
        {
            var answer = await DisplayAlertAsync("Informaciòn", "Desea aplicar el inventario seleccionado ¿Desea Continuar?", "Si", "No");
            if (answer == false)
            { return; }

            // ############ VALIDAR AUTORIZACION
            if (cnivel_limite == 0)
                return;

            string Titulo = string.Empty;
            switch (cnivel_limite)
            {
                case 1:
                    Titulo = "Supervisor";
                    break;

                case 2:
                    Titulo = "Administrador";
                    break;

                case 3:
                    Titulo = "Limites Superados";
                    break;
            }

            string result = await DisplayPromptAsync(Titulo, "Ingrese la clave de Autorizacion, para guardar la plantilla.", "OK", "Cancelar", "Clave de Autorizacion", -1, keyboard: Keyboard.Password);
            if (!string.IsNullOrEmpty(result))
            {
                Verificar_autorizacion(result);
            }
        }

        private async void Verificar_autorizacion(string Clave)
        {
            string clave_aut_ = Clave;

            if (clave_aut_.Trim() == "")
                return;
            double cnivel_autorizacion_ = 0;
            string cautorizador_ = "";

            string Parametros = $"{clave_aut_}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion/GET", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]")
                {
                    await DisplayAlertAsync("Advertencia", "Clave Ingresada Incorrecta", "OK");
                    return;
                }

                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    cnivel_autorizacion_ = double.Parse(dt.Rows[0][1].ToString());
                    cautorizador_ = dt.Rows[0][0].ToString();
                }
            }

            if (cnivel_autorizacion_ < cnivel_limite)
            {
                await DisplayAlertAsync("Advertencia", "Nivel de Autorizacion Insuficiente", "OK");
                return;
            }

            Parametros = $"{Global.FolioInventario}";
            response = ConfigAPI.GetAPI("GET", "api/Operacion/GET", Parametros, "ws_fnSetAplicarInventario");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await DisplayAlertAsync("Informacion", "Inventario aplicado correctamente", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}