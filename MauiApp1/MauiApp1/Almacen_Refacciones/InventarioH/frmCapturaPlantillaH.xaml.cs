using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.Views;

using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using ZXing;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace iAlmacen.Almacen_Refacciones.InventarioH
{
    public partial class frmCapturaPlantillaH : ContentPage
    {
        private int cnivel_limite = 1;

        public ObservableCollection<Item_RegArticulo> Items { get; set; }
        public Command LoadItemsCommand_Inventario { get; set; }
        private ItemsViewModel_Inventario viewModel_Inventario;

        private string _result;

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        //private ObservableCollection<RegArticulo> RegArticulos = new ObservableCollection<RegArticulo>();
        private ObservableCollection<InventarioAlmacen> InventarioAlmacens = new ObservableCollection<InventarioAlmacen>();

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_Inventario.LoadItemsCommand_inventario.Execute(null);
        }

        public frmCapturaPlantillaH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");

            Items = new ObservableCollection<Item_RegArticulo>();
            LoadItemsCommand_Inventario = new Command(async () => await cargar());
            BindingContext = viewModel_Inventario = new ItemsViewModel_Inventario();

            CargarCategoriasFamilias();
            CargarAlmacenistas();
        }

        #region "Funciones y Metodos"

        private void CargarAlmacenistas()
        {
            cbAlmacenista.ItemsSource = Funciones.LlenarAlmacenistas();
        }

        private void CargarCategoriasFamilias()
        {
            cbFamilia.ItemsSource = Funciones.LlenarCategoriaFamilias(true);
        }

        private async void OnTextResultGenerated(string result)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (string.IsNullOrEmpty(result))
                {
                    await DisplayAlertAsync("Informacion", "No se ha escaneado un código válido", "OK");
                    return;
                }
                else
                {
                    Result = $"{result}";
                    //Result = "0U01U1U0U1";
                    //BuscarUbicacion();
                    string Codigo = Result;
                    //string[] Valores;
                    Codigo = Codigo.Substring(1, 5);
                    foreach (Item_RegArticulo RegArt in Items)
                    {
                        if (RegArt.CodigoActual == Codigo)
                        {
                            await DisplayAlertAsync("Informacion", "Articulo ya se encuentra agregado", "OK");
                            return;
                        }
                    }

                    string Parametros = $"2,{Codigo},,,,false,0";
                    HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_InvCatalogo_Articulos");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound) return;
                        string resp = reader.ReadToEnd();
                        if (resp == "[]") return;
                        DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r[16].ToString() == "1")
                            {
                                await DisplayAlertAsync("Informacion", "Articulo ya se encuentran en proceso de inventario", "OK");
                                break;
                            }

                            Items.Add(new Item_RegArticulo
                            {
                                id = int.Parse(r[0].ToString()),
                                CodigoActual = r[1].ToString(),
                                CodigoAnterior = r[2].ToString(),
                                Descripcion = r[3].ToString(),
                                ClaveFamilia = r[4].ToString(),
                                ClaveLinea = r[5].ToString(),
                                ClaveGrupo = r[6].ToString(),
                                desc_familia = r[7].ToString(),
                                desc_linea = r[8].ToString(),
                                desc_grupo = r[9].ToString(),
                                DescMarca = r[10].ToString(),
                                DescMedida = r[11].ToString(),
                                DescParte = r[12].ToString(),
                                existencia = double.Parse(r[13].ToString()),
                                Fisico = 0,
                                Inventario = "0",
                                Aplicado = "0",
                                Fecha_ = DateTime.Now.ToShortDateString().ToString(),
                                UnidadControl = r[14].ToString(),
                                Costo = double.Parse(r[15].ToString()),
                                Seccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[19].ToString().Trim() : "",
                                DescSeccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[20].ToString().Trim() : "",
                                Pasillo = int.Parse(r[17].ToString().Trim()) == 1 ? r[21].ToString().Trim() : "",
                                Estanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[22].ToString().Trim() : "",
                                DescEstanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[23].ToString().Trim() : "",
                                Nivel = int.Parse(r[17].ToString().Trim()) == 1 ? r[24].ToString().Trim() : "",
                                Tarima = int.Parse(r[17].ToString().Trim()) == 1 ? r[25].ToString().Trim() : "",
                                Contenedor = int.Parse(r[17].ToString().Trim()) == 1 ? r[26].ToString().Trim() : ""
                            });
                        }
                    }
                }
            });
        }

        #endregion "Funciones y Metodos"

        private void rbFamilias_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbFamilias.IsChecked)
            {
                PanelFamilias.IsVisible = true;
            }
            else
            {
                PanelFamilias.IsVisible = false;
            }
        }

        private void rbClave_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbClave.IsChecked)
            {
                PanelClave.IsVisible = true;
            }
            else
            {
                PanelClave.IsVisible = false;
            }
        }

        private void rbCodigoBarras_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbCodigoBarras.IsChecked)
            {
                PanelCodigoBarras.IsVisible = true;
            }
            else
            {
                PanelCodigoBarras.IsVisible = false;
            }
        }

        private void rbAleatorio_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbAleatorio.IsChecked)
            {
                PanelAleatorio.IsVisible = true;
            }
            else
            {
                PanelAleatorio.IsVisible = false;
            }
        }

        private void cbFamilia_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] sFamilia;
            sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
            cbLinea.ItemsSource = null;
            cbLinea.ItemsSource = Funciones.LlenarCategoriaLineas(sFamilia[0].ToString().Trim());
        }

        private void cbLinea_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] sFamilia;
            string[] sLinea;
            cbgrupo.ItemsSource = null;

            try
            {
                sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
                sLinea = cbLinea.SelectedItem.ToString().Split('-');
            }
            catch (Exception ex)
            {
                cbgrupo.Items.Clear();
                return;
            }
            cbgrupo.ItemsSource = Funciones.LlenarCategoriaGrupo(sFamilia[0].ToString().Trim(), sLinea[0].ToString().Trim());
        }

        private async void btnAgregarCategoria_Clicked(Object sender, EventArgs e)
        {
            if (cbFamilia.SelectedIndex == -1)
            { return; }
            ;
            if (cbLinea.SelectedIndex == -1)
            { return; }
            ;
            if (cbgrupo.SelectedIndex == -1)
            { return; }
            ;

            string[] sFamilia;
            sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
            string[] sLinea;
            sLinea = cbLinea.SelectedItem.ToString().Split('-');
            string[] sGrupo;
            sGrupo = cbgrupo.SelectedItem.ToString().Split('-');
            Boolean bEncontrado = false;
            Boolean benProceso = false;
            Boolean mEncontrado = false;

            string Parametros = $"2,,{sFamilia[0].ToString().Trim()},{sLinea[0].ToString().Trim()},{sGrupo[0].ToString().Trim()},false,0";

            viewModel_Inventario.Parametros = Parametros;
            viewModel_Inventario.LoadItemsCommand_inventario.Execute(null);

            if (viewModel_Inventario.MEncontrado)
            {
                await DisplayAlertAsync("Informacion", "Se encontraron articulos ya agregados en la lista", "OK");
            }

            if (viewModel_Inventario.benProceso)
            {
                await DisplayAlertAsync("Informacion", "Se omitieron los articulos que ya se encuentran en proceso de inventario", "OK");
            }
        }

        private async void btnAgregarClaveArticulo_Clicked(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtClaveArticulo.Text))
            {
                await DisplayAlertAsync("Informacion", "Favor de capturar una clave correcta", "OK");
                return;
            }

            foreach (Item_RegArticulo RegArt in Items)
            {
                if (RegArt.CodigoActual == txtClaveArticulo.Text.ToString().Trim())
                {
                    await DisplayAlertAsync("Informacion", "Articulo ya se encuentra agregado", "OK");
                    return;
                }
            }

            string Parametros = $"2,{txtClaveArticulo.Text.ToString().Trim()},,,,false,0";
            viewModel_Inventario.Parametros = Parametros;
            viewModel_Inventario.LoadItemsCommand_inventario.Execute(null);

            if (!viewModel_Inventario.Items.Any())
            {
                await DisplayAlertAsync("Error", "La Clave ingresada no existe o no pertenece a la categoria de Herramienta.", "OK");
            }
        }

        private async void btnCodigoBarras_Clicked(Object sender, EventArgs e)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status == PermissionStatus.Granted)
            {
                if (BarcodeScanning.IsSupported)
                {
                    var barcodePage = new BarcodePage();
                    barcodePage.ParentPageName = "Lectura_Codigo";
                    barcodePage.TextResultGenerated += OnTextResultGenerated;
                }
                else
                {
                    await DisplayAlertAsync("Error", "La funcionalidad de escaneo no es compatible con este dispositivo.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlertAsync("Informacion", $"El permiso fue denegado.", "OK");
            }
        }

        private async void btnAgregarAleatorio_Clicked(Object sender, EventArgs e)
        {
            if (cbAleatorio.SelectedIndex == -1)
            { return; }
            ;

            string Parametros = $"2,,,,,true,{cbAleatorio.SelectedItem.ToString().Trim()}";
            viewModel_Inventario.Parametros = Parametros;
            viewModel_Inventario.Aleatorio = true;
            viewModel_Inventario.AleatorioCantidad = int.Parse(cbAleatorio.SelectedItem.ToString().Trim());
            viewModel_Inventario.LoadItemsCommand_inventario.Execute(null);

            if (viewModel_Inventario.MEncontrado)
            {
                await DisplayAlertAsync("Informacion", "Se encontraron articulos ya agregados en la lista", "OK");
            }

            if (viewModel_Inventario.benProceso)
            {
                await DisplayAlertAsync("Informacion", "Se omitieron los articulos que ya se encuentran en proceso de inventario", "OK");
            }
        }

        private void btnEliminarLista_Clicked(Object sender, EventArgs e)
        {
            Item_RegArticulo Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_RegArticulo;
            Items.Remove(Item_);
        }

        private async void btnCancelar_Clicked(Object sender, EventArgs e)
        {
            if (viewModel_Inventario.Items.Any())
            {
                var answer = await DisplayAlertAsync("Información", "Se eliminaran los Articulos capturados. ¿Desea Continuar?", "Si", "No");
                if (answer == false)
                { return; }

                viewModel_Inventario.Items.Clear();
                cbAlmacenista.SelectedIndex = -1;
            }
            else
            {
                await DisplayAlertAsync("Información", "No existe ningun articulo para eliminar", "OK");
                return;
            }
        }

        private async void btnGrabar_Clicked(Object sender, EventArgs e)
        {
            if (cbAlmacenista.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Informacion", "Debe seleccionar al almacenista responsable", "OK");
                return;
            }

            // ############ VALIDAR AUTORIZACION
            //bAutorizado = false;
            if (cnivel_limite == 0)
                return;

            var answer = await DisplayAlertAsync("Informaciòn", "Se guardara la plantilla de inventario. ¿Desea Continuar?", "Si", "No");
            if (answer == false)
            { return; }

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

            //string result = await alertDialogService.ShowDialogConfirmationAsync(Titulo, "Ingrese la clave de Autorizacion, para guardar la plantilla.", "Cancel", "OK", true, true);
            string result = await DisplayPromptAsync(Titulo, "Ingrese la clave de Autorizacion, para guardar la plantilla.", "OK", "Cancel", "Clave de Autorizacion", -1, Keyboard.Password);
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
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
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

            int sFolioInventario = 0;
            string[] sAlmacenista;
            sAlmacenista = cbAlmacenista.SelectedItem.ToString().Split('-');
            foreach (Item_RegArticulo Art_ in Items)
            {
                InventarioAlmacens.Add(new InventarioAlmacen
                {
                    id = Art_.id,
                    FolioInventario = "0",
                    CodigoArticulo = Art_.CodigoActual,
                    FechaInventario = DateTime.Now.ToShortDateString(),
                    HoraInventario = DateTime.Now.ToShortTimeString(),
                    UnidadControl = Art_.UnidadControl,
                    ExistenciaSistema = Art_.existencia,
                    InventarioAlmacen_ = 0.00,
                    InventarioContabilidad = 0.00,
                    EntradasContabilidad = 0.00,
                    SalidasContabilidad = 0.00,
                    Aplicado = false,
                    FechaAplicacion = "",
                    HoraAplicacion = "",
                    Cancelado = false,
                    Capturado = false,
                    Costo = Art_.Costo,
                    CostoCapturado = 0.00,
                    Importe = Art_.Costo * Art_.existencia,
                    UsuarioResponsable = sAlmacenista[1].ToString().Trim(),
                    ClaveResponsable = sAlmacenista[0].ToString().Trim(),
                    Cerrado = false,
                    Duplicado = false,
                    Muestreo = false,
                    uso_herramienta = true
                });
            }

            DataTable dtResponse = ConfigAPI.PostAPI_NvaPlantillaH("api/InventarioAlmacenH", "CrearPlantilla", InventarioAlmacens);
            foreach (DataRow r in dtResponse.Rows)
            {
                if (r[1].ToString().Trim() == "200 OK")
                {
                    if (int.Parse(r[2].ToString().Trim()) > 0)
                    {
                        sFolioInventario = int.Parse(r[2].ToString().Trim());
                    }
                }
            }

            await DisplayAlertAsync("Informacion", "Plantilla Generada Correctamente con Folio: " + sFolioInventario.ToString(), "OK");
            Items.Clear();
        }
    }
}