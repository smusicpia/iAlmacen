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
    public partial class frmResguardosEmpleadosCerrados : ContentPage
    {
        private int cnivel_limite = 1;
        public ObservableCollection<Item_ResgEmpleado> Items { get; set; }
        public Command LoadItemsCommand_ResgEmpleado { get; set; }
        private ItemsViewModel_ResgEmpleado viewModel_ResgEmpleado;

        //private ObservableCollection<clsHerramientaEmpleados> RegEmpleados = new ObservableCollection<clsHerramientaEmpleados>();
        //private clsEmpleadoHerramienta EmpleadoAplicar;

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_ResgEmpleado.LoadItemsCommand_resgempleado.Execute(null);
        }

        public frmResguardosEmpleadosCerrados()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");

            Items = new ObservableCollection<Item_ResgEmpleado>();
            LoadItemsCommand_ResgEmpleado = new Command(async () => await cargar());
            BindingContext = viewModel_ResgEmpleado = new ItemsViewModel_ResgEmpleado();
        }

        private async void btnAplicar_Clicked(Object sender, EventArgs e)
        {
            Item_ResgEmpleado Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ResgEmpleado;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "Desea aplicar al empleado seleccionado ¿Desea Continuar?", "Si", "No");
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

            //TODO UIAlertView With Dismissed and AlertViewStyle
            //string result = await alertDialogService.ShowDialogConfirmationAsync(Titulo, "Ingrese la clave de Autorizacion, para guardar la plantilla.", null, "OK", true, true);
            string result = await DisplayPromptAsync(Titulo, "Ingrese la clave de Autorizacion, para guardar la plantilla.", "OK", "Cancelar", "Clave de Autorizacion", -1, Keyboard.Password);
            if (!string.IsNullOrEmpty(result))
            {
                Verificar_autorizacion(result, Item_.clave);
            }
        }

        private async void btnAbrir_Clicked(Object sender, EventArgs e)
        {
            Item_ResgEmpleado Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_ResgEmpleado;
            if (Item_ == null)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "¿Desea RE-ABRIR al empleado seleccionado?", "Si", "No");
            if (answer == false)
            { return; }

            string sResponce = "";
            string Parametros = "aplicado=0,cerrado=0";
            string Condicion = $"codigo_autorizado='{Item_.clave}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "detalle_salidas_resguardo_herramientas", Condicion, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
            viewModel_ResgEmpleado.LoadItemsCommand_resgempleado.Execute(null);
        }

        private async void Verificar_autorizacion(string Clave, string EmpleadoAplicar_clave)
        {
            string clave_aut_ = Clave;

            if (clave_aut_.Trim() == "")
                return;
            double cnivel_autorizacion_ = 0;
            string cautorizador_ = "";

            string Parametros = $"{clave_aut_}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]")
                {
                    await DisplayAlertAsync("Advertencia", "Clave Ingresada Incorrecta", "OK");
                    return;
                }

                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    cnivel_autorizacion_ = double.Parse(dt.Rows[0][1].ToString());
                    cautorizador_ = dt.Rows[0][0].ToString(); ;
                }
            }

            if (cnivel_autorizacion_ < cnivel_limite)
            {
                await DisplayAlertAsync("Advertencia", "Nivel de Autorizacion Insuficiente", "OK");
                return;
            }

            Parametros = $"{EmpleadoAplicar_clave}";
            response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_AplicarInventarioH");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                if (dt.Rows[0][1].ToString() == "200 OK")
                {
                    await DisplayAlertAsync("Informacion", "Inventario aplicado correctamente", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}