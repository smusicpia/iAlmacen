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
    public partial class frmResguardosEmpleados : ContentPage
    {
        public ObservableCollection<Item_ResgEmpleado> Items { get; set; }
        public Command LoadItemsCommand_ResgEmpleado { get; set; }
        private ItemsViewModel_ResgEmpleado viewModel_ResgEmpleado;

        private async Task cargar()
        { }

        public frmResguardosEmpleados()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");

            Items = new ObservableCollection<Item_ResgEmpleado>();
            LoadItemsCommand_ResgEmpleado = new Command(async () => await cargar());
            BindingContext = viewModel_ResgEmpleado = new ItemsViewModel_ResgEmpleado();

            CargarAreas();
        }

        private void CargarAreas()
        {
            cbArea.ItemsSource = Funciones.LlenarAreaH();
        }

        private void btnCancelar_Clicked(Object sender, EventArgs e)
        {
            string Parametros = $"";
            viewModel_ResgEmpleado.LoadItemsCommand_resgempleado.Execute(null);
        }

        private void btnBuscar_Clicked(Object sender, EventArgs e)
        {
            if (cbArea.SelectedIndex == -1)
            { return; }
            ;

            Buscar();
        }

        private void Buscar()
        {
            //RegEmpleados.Clear();

            string[] sArea;
            sArea = cbArea.SelectedItem.ToString().Split('-');

            string Parametros = $"{sArea[0].ToString().Trim()}";
            viewModel_ResgEmpleado.Parametros = Parametros;
            viewModel_ResgEmpleado.LoadItemsCommand_resgempleado.Execute(null);
        }

        private async void btnCerrar_Clicked(Object sender, EventArgs e)
        {
            Item_ResgEmpleado Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ResgEmpleado;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "Las herramientas no inventariadas seran marcadas como EXTRAVIADAS automaticamente ¿Desea CERRAR al empleado seleccionado?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = $"{Item_.clave}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_CerrarResguardo");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    sResponce = r[1].ToString();
                }
            }
            Buscar();
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_ResgEmpleado;
            if (item == null)
                return;
            Global.ResgEmpleado = item;
            await Navigation.PushAsync(new frmInventarioEmpleado());
        }
    }
}