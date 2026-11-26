using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmResguardosEmpleados : ContentPage
    {
        public ObservableCollection<Item_ResgEmpleado> Items { get; set; }
        public Command LoadItemsCommand_ResgEmpleado { get; set; }
        private ItemsViewModel_ResgEmpleado viewModel_ResgEmpleado;

		public List<string> lAreas { get; set; }

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

        private async Task CargarAreas()
        {
			lAreas = new List<String>();
			lAreas.Add("");
			DataTable? dt = await Funciones.LlenarAreaH();
			foreach (DataRow r in dt.Rows)
			{
				lAreas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
			}
			cbArea.ItemsSource = lAreas;

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
            //string Parametros = $"{Item_.clave}";
            //HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/GET", Parametros, "wsp_CerrarResguardo").Result;
            //using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
            //{
            //    if (response.StatusCode == HttpStatusCode.NotFound) return;
            //    string resp = reader.ReadToEnd();
            //    if (resp == "[]") return;
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        sResponce = r[1].ToString();
            //    }
            //}

			HttpResponseMessage response;
			try
			{
				string json = $"{{\"codigo\":\"{Item_.clave}\"}}";
				response = await SesionApi.PutAsync($"api/operacion/CerrarResguardo?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}", Funciones.jsonContent(json));
				if (!response.IsSuccessStatusCode)
				{
					string jsonResponse = await response.Content.ReadAsStringAsync();
					DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
					if (dt.Rows.Count > 0)
					{
						if (dt.Rows[0]["Code"].ToString() == "200 OK")
						{
							sResponce = dt.Rows[0][1].ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
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