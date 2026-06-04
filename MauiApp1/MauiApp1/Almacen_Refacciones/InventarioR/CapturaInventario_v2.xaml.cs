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
    public partial class CapturaInventario_v2 : ContentPage
    {
        //private ObservableCollection<clsInventarioDetalle> RegInventariosDetalle = new ObservableCollection<clsInventarioDetalle>();
        private ObservableCollection<string> SeccionesEncontrados = new ObservableCollection<string>();
        private string SeccionEncontrado = "";
        private int PasilloEncontrado = 0;
        private string EstanteriaEncontrado = "";
        private int NivelEncontrado = 0;
        private int TarimaEncontrado = 0;
        private int CajaEncontrado = 0;
        private bool capturando = false;
        private bool consulta = false;
        private bool inventariado = false;
        private string _tInventario = "R";

        public ObservableCollection<Item_InventarioDetalle> Items { get; set; }
        public Command LoadItemsCommand_InventarioDetalle { get; set; }
        private ItemsViewModel_InventarioDetalle viewModel_InventarioDetalle;

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_InventarioDetalle.LoadItemsCommand_inventariodetalle.Execute($"{Global.FolioInventario}");

            switch (_tInventario)
            {
                case "H":
                    LeerArticuloEnInventario();
                    this.Title = "Consulta Inventario de Herramienta: " + Global.FolioInventario;
                    break;
                case "R":
                    LeerArticuloEnInventario();
                    this.Title = "Consulta Inventario de Refacciones: " + Global.FolioInventario;
                    break;
                default:
                    break;
            }
        }

        public CapturaInventario_v2()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            Items = new ObservableCollection<Item_InventarioDetalle>();

            LoadItemsCommand_InventarioDetalle = new Command(async () => await cargar());
            string Parametros = string.Empty;
            if (consulta)
                Parametros = $"0,{Global.ArticuloEnInventario.CodigoActual}";
            else
                Parametros = $"{Global.ArticuloEnInventario.id},null";
            BindingContext = viewModel_InventarioDetalle = new ItemsViewModel_InventarioDetalle(Parametros);
            LeerArticuloEnInventario();
        }

        public CapturaInventario_v2(bool Capturado, string tInventario = "R")
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            consulta = Capturado;
            inventariado = Capturado;

            Items = new ObservableCollection<Item_InventarioDetalle>();
            LoadItemsCommand_InventarioDetalle = new Command(async () => await cargar());
            string Parametros = string.Empty;
            if (consulta)
                Parametros = $"0,{Global.ArticuloEnInventario.CodigoActual}";
            else
                Parametros = $"{Global.ArticuloEnInventario.id},null";
            BindingContext = viewModel_InventarioDetalle = new ItemsViewModel_InventarioDetalle(Parametros);
            _tInventario = tInventario;
        }

        private void CargarSeccion()
        {
            foreach (var item in SeccionesEncontrados)
            {
                if (SeccionEncontrado == "")
                    SeccionEncontrado = "'" + item + "'";
                else
                    SeccionEncontrado += ",'" + item + "'";
            }

            cbSeccion.ItemsSource = Funciones.LlenarSecciones("M", SeccionEncontrado);

            if (SeccionesEncontrados.Count == 1)
                cbSeccion.SelectedIndex = 0;
            else if (SeccionesEncontrados.Count > 1)
                cbSeccion.SelectedIndex = -1;
        }

        private void CargarPasillos()
        {
            clsSeccion iSeccion;
            iSeccion = (clsSeccion)cbSeccion.SelectedItem;

            if (!iSeccion.Pasillos)
            {
                cbPasillo.IsEnabled = false;
            }
            else
            {
                for (int i = 1; i <= iSeccion.NumeroPasillos; i++)
                {
                    cbPasillo.Items.Add(i.ToString());
                }

                if (PasilloEncontrado != 0)
                {
                    cbPasillo.SelectedIndex = PasilloEncontrado + 1;
                    cbPasillo.IsEnabled = false;
                }
            }
        }

        private void CargarEstanterias()
        {
            clsSeccion iSeccion;
            iSeccion = (clsSeccion)cbSeccion.SelectedItem;

            if (!iSeccion.Estanterias)
            {
                //cbEstanteria.IsEnabled = false;
            }
            else
            {
                cbEstanteria.ItemsSource = Funciones.LlenarEstanterias("M", iSeccion.Clave);

                if (EstanteriaEncontrado != "")
                {
                    int valor = 0;
                    foreach (clsEstanteria item in cbEstanteria.ItemsSource)
                    {
                        if (item.Clave == EstanteriaEncontrado)
                        {
                            cbEstanteria.SelectedIndex = valor;
                            break;
                        }
                        valor = valor + 1;
                    }
                }
            }
        }

        private void SeleccionarEstanteria(string Estanteria)
        {
            int valor = 0;
            foreach (clsEstanteria item in cbEstanteria.ItemsSource)
            {
                if (item.Clave == Estanteria)
                {
                    cbEstanteria.SelectedIndex = valor;
                    break;
                }
                valor = valor + 1;
            }
        }

        private void CargarNieveles()
        {
            if (cbEstanteria.SelectedIndex < 0) return;
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbNivel.Items.Clear();

            for (int i = 0; i < iEstanteria.NumeroNiveles; i++)
            {
                cbNivel.Items.Add((i + 1).ToString());
            }

            if (NivelEncontrado != 0)
            {
                cbNivel.SelectedIndex = NivelEncontrado - 1;
            }
        }

        private void CargarTarimas()
        {
            if (cbEstanteria.SelectedIndex < 0) return;
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbTarima.Items.Clear();

            if (iEstanteria.Tarimas)
            {
                cbTarima.IsVisible = true;
                lblTarima.IsVisible = true;
                for (int i = 0; i < iEstanteria.NumeroTarimas; i++)
                {
                    cbTarima.Items.Add((i + 1).ToString());
                }

                if (TarimaEncontrado != 0)
                {
                    cbTarima.SelectedIndex = TarimaEncontrado - 1;
                }
            }
            else
            {
                cbTarima.IsVisible = false;
                lblTarima.IsVisible = false;
            }
        }

        private void CargarCajas()
        {
            if (cbEstanteria.SelectedIndex < 0) return;
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbCaja.Items.Clear();

            int Cajas = 0;

            if (iEstanteria.Cajas)
            {
                cbCaja.IsVisible = true;
                lblCaja.IsVisible = true;
                if (iEstanteria.ReiniciarNumeracionCajas)
                {
                    Cajas = iEstanteria.NumeroCajasTarima;
                }
                else
                {
                    Cajas = iEstanteria.NumeroCajas;
                }

                for (int i = 0; i < Cajas; i++)
                {
                    cbCaja.Items.Add((i + 1).ToString());
                }

                if (CajaEncontrado != 0)
                {
                    cbCaja.SelectedIndex = CajaEncontrado - 1;
                }
            }
            else
            {
                cbCaja.IsVisible = false;
                lblCaja.IsVisible = false;
            }
        }

        private void LeerArticuloEnInventario()
        {
            lblFamilia.Text = Global.ArticuloEnInventario.desc_familia;
            lblLinea.Text = Global.ArticuloEnInventario.desc_linea;
            lblGrupo.Text = Global.ArticuloEnInventario.desc_grupo;
            lblCodigo.Text = Global.ArticuloEnInventario.CodigoActual;
            lblDescripcion.Text = Global.ArticuloEnInventario.Descripcion;
            lblMedida.Text = Global.ArticuloEnInventario.DescMedida;
            lblMarca.Text = Global.ArticuloEnInventario.DescMarca;
            lblParte.Text = Global.ArticuloEnInventario.DescParte;
            lblUnidadControl.Text = Global.ArticuloEnInventario.UnidadControl;

            CargarSeccion();
            //BuscarCapturas();
            if (Global.ArticuloEnInventario.ExisUbi == 1)
            {
                if (Global.ArticuloEnInventario.Seccion != "")
                {
                    cbSeccion.SelectedIndex = int.Parse(Global.ArticuloEnInventario.Seccion) - 1;
                    cbPasillo.SelectedIndex = int.Parse(Global.ArticuloEnInventario.Pasillo) - 1;

                    int valor = 0;
                    foreach (clsEstanteria item in cbEstanteria.ItemsSource)
                    {
                        if (item.Clave == Global.ArticuloEnInventario.Estanteria)
                        {
                            cbEstanteria.SelectedIndex = valor;
                            break;
                        }
                        valor = valor + 1;
                    }
                    cbNivel.SelectedIndex = int.Parse(Global.ArticuloEnInventario.Nivel) - 1;
                    if (cbTarima.IsVisible)
                        cbTarima.SelectedIndex = int.Parse(Global.ArticuloEnInventario.Tarima) - 1;
                    if (cbCaja.IsVisible)
                        cbCaja.SelectedIndex = int.Parse(Global.ArticuloEnInventario.Contenedor) - 1;
                }
            }
            if (inventariado)
            {
                cbSeccion.IsEnabled = !inventariado;
                cbPasillo.IsEnabled = !inventariado;
                cbEstanteria.IsEnabled = !inventariado;
                cbNivel.IsEnabled = !inventariado;
                cbTarima.IsEnabled = !inventariado;
                cbCaja.IsEnabled = !inventariado;
                txtCantidad.IsEnabled = !inventariado;
                btnAgregar.IsEnabled = !inventariado;
                btnGuardar.IsEnabled = !inventariado;
                if (consulta)
                    btnGuardar.IsEnabled = !inventariado;
            }
        }
        private void cbSeccion_SelectedIndexChanged(Object sender, EventArgs e)
        {
            capturando = true;
            cbEstanteria.SelectedIndex = -1;
            cbNivel.Items.Clear();
            cbTarima.Items.Clear();
            cbCaja.Items.Clear();
            CargarPasillos();
            capturando = false;
        }

        private void cbPasillo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (capturando) return;
            capturando = true;
            cbNivel.Items.Clear();
            cbTarima.Items.Clear();
            cbCaja.Items.Clear();
            CargarEstanterias();
            capturando = false;
        }

        private void cbEstanteria_SelectedIndexChanged(Object sender, EventArgs e)
        {
            CargarNieveles();
            CargarTarimas();
            CargarCajas();
        }

        private void btnAgregar_Clicked(Object sender, EventArgs e)
        {
            clsSeccion iSeccion;
            clsEstanteria iEstanteria;

            string sSeccion = "";
            string sDescSeccion = "";
            int sPasillo = 0;
            string sEstanteria = "";
            string sDescEstanteria = "";
            string sNivel = "";

            try
            {
                iSeccion = (clsSeccion)cbSeccion.SelectedItem;
                sSeccion = iSeccion.Clave;
                sDescSeccion = iSeccion.Descripcion;
            }
            catch (Exception)
            {
                sSeccion = "";
            }

            try
            {
                iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
                sPasillo = iEstanteria.Pasillo;
                sEstanteria = iEstanteria.Clave;
                sDescEstanteria = iEstanteria.Descripcion;
            }
            catch (Exception)
            {
                sPasillo = 0;
                sEstanteria = "";
            }

            try
            {
                sNivel = cbNivel.SelectedItem.ToString();
            }
            catch (Exception)
            {
                sNivel = "0";
            }

            double dCantidad = 0;
            int iTarima = 0;
            int iCaja = 0;

            if (cbTarima.IsVisible)
            {
                try
                {
                    iTarima = int.Parse(cbTarima.SelectedItem.ToString());
                }
                catch (Exception)
                {
                    iTarima = 0;
                }
            }
            else { iTarima = 0; }

            if (cbCaja.IsVisible)
            {
                try
                {
                    iCaja = int.Parse(cbCaja.SelectedItem.ToString());
                }
                catch (Exception)
                {
                    iCaja = 0;
                }
            }
            else { iCaja = 0; }
            

            try
            {
                dCantidad = double.Parse(txtCantidad.Text);
            }
            catch (Exception)
            {
                dCantidad = 0;
            }

            if (dCantidad == 0)
            {
                DisplayAlertAsync("Informacion", "La cantidad no puede ser 0 (cero)", "OK");
                return;
            }

            //RegInventariosDetalle.Add(new clsInventarioDetalle
            //{
            //    folioInventario = Global.FolioInventario,
            //    idReferencia = Global.ArticuloEnInventario.id.ToString(),
            //    CodigoArticulo = Global.ArticuloEnInventario.CodigoActual,
            //    Sucursal = "M",
            //    Seccion = sSeccion,
            //    DescripcionSeccion = sDescSeccion,
            //    Pasillo = sPasillo,
            //    Estanteria = sEstanteria,
            //    DescripcionEstanteria = sDescEstanteria,
            //    Nivel = int.Parse(sNivel),
            //    Familia = Global.ArticuloEnInventario.ClaveFamilia,
            //    Linea = Global.ArticuloEnInventario.ClaveLinea,
            //    Grupo = Global.ArticuloEnInventario.ClaveGrupo,
            //    Tarima = iTarima,
            //    Caja = iCaja,
            //    UnidadControl = Global.ArticuloEnInventario.UnidadControl,
            //    Existencia = double.Parse(txtCantidad.Text),
            //    DescFamilia = Global.ArticuloEnInventario.desc_familia
            //});

            viewModel_InventarioDetalle.AgregarCommand.Execute(new Item_InventarioDetalle
            {
                folioInventario = Global.FolioInventario,
                idReferencia = Global.ArticuloEnInventario.id.ToString(),
                CodigoArticulo = Global.ArticuloEnInventario.CodigoActual,
                Sucursal = "M",
                Seccion = sSeccion,
                DescripcionSeccion = sDescSeccion,
                Pasillo = sPasillo,
                Estanteria = sEstanteria,
                DescripcionEstanteria = sDescEstanteria,
                Nivel = int.Parse(sNivel),
                Familia = Global.ArticuloEnInventario.ClaveFamilia,
                Linea = Global.ArticuloEnInventario.ClaveLinea,
                Grupo = Global.ArticuloEnInventario.ClaveGrupo,
                Tarima = iTarima,
                Caja = iCaja,
                UnidadControl = Global.ArticuloEnInventario.UnidadControl,
                Existencia = double.Parse(txtCantidad.Text),
                DescFamilia = Global.ArticuloEnInventario.desc_familia,
                Usuario = Global.clave_usuario,
                EsActivo = false // Nuevo campo para indicar si el artículo es Refaccion (false) o Herramienta (true)
            });

            txtCantidad.Text = "0";
            //ItemsListView.ItemsSource = RegInventariosDetalle;
        }

        private async void btnEliminarLista_Clicked(Object sender, EventArgs e)
        {
            Item_InventarioDetalle Item_;
            Item_ = (sender as MenuItem).BindingContext as Item_InventarioDetalle;
            if (Item_.ID! > 0)
            {
                await DisplayAlertAsync("Alerta", "Ya no es posible Modificar el Inventario", "OK");
                return;
            }

            viewModel_InventarioDetalle.Items.Remove(Item_);
        }

        private async void btnGuardar_Clicked(Object sender, EventArgs e)
        {
            if (inventariado)
            {
                await DisplayAlertAsync("Alerta", "Ya no es posible Modificar el Inventario", "OK");
                return;
            }
            if (viewModel_InventarioDetalle.Items.Count == 0)
            {
                await DisplayAlertAsync("Error", "No es posible guardar, no se ha capturado ningun inventario.", "OK");
                return;
            }

            bool resp_Ok = false;
            var answer = await DisplayAlertAsync("Informaciòn", "Se guardara el inventario capturado ¿Desea Continuar?", "Si", "No");
            if (answer == false)
            { return; }

            foreach (Item_InventarioDetalle item in viewModel_InventarioDetalle.Items)
            {
                item.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                item.Hora = DateTime.Now.ToString("HH:mm:ss");
                item.Usuario = Global.clave_usuario;
            }

            HttpResponseMessage Response = APIService.PostAPI_GuardarInventario("api/InventarioAlmacen", viewModel_InventarioDetalle.Items).Result;
            if (Response.IsSuccessStatusCode)
            {
                resp_Ok = true;
            }

            if (resp_Ok)
            {
                await DisplayAlertAsync("Informacion", "Inventario capturado correctamente", "OK");
                await Navigation.PopAsync();
            }

            await DisplayAlertAsync("Informacion", "Inventario capturado correctamente", "OK");
            await Navigation.PopAsync();
        }
    }
}