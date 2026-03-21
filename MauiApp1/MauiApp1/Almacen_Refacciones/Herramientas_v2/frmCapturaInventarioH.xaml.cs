using iAlmacen.Clases;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmCapturaInventarioH : ContentPage
    {
        private ObservableCollection<clsSeccion> RegSecciones = new ObservableCollection<clsSeccion>();
        private ObservableCollection<clsEstanteria> RegEstanterias = new ObservableCollection<clsEstanteria>();
        private ObservableCollection<clsInventarioDetalle> RegInventariosDetalle = new ObservableCollection<clsInventarioDetalle>();
        private ObservableCollection<CatalogoArticuloNumeroSeries> GenNumerosSeries = new ObservableCollection<CatalogoArticuloNumeroSeries>();
        private List<string> listaSeries = new List<string>();
        private string SeccionEncontrado = "";
        private int PasilloEncontrado = 0;
        private string EstanteriaEncontrado = "";
        private int NivelEncontrado = 0;
        private int TarimaEncontrado = 0;
        private int CajaEncontrado = 0;
        private bool capturando = false;
        private bool inventariado = false;
        private bool consulta = false;

        public frmCapturaInventarioH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            LeerArticuloEnInventario();
        }

        public frmCapturaInventarioH(bool Capturado)
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            consulta = Capturado;
            inventariado = Capturado;
            LeerArticuloEnInventario(true);
            this.Title = "Consulta Inventario de Herramienta: " + Global.FolioInventario;
        }

        private void CargarSeccion()
        {
            if (SeccionEncontrado == "")
            {
                RegSecciones = Funciones.LlenarSecciones();
            }
            else
            {
                RegSecciones = Funciones.LlenarSecciones("M", SeccionEncontrado);
            }
            cbSeccion.ItemsSource = RegSecciones;

            if (SeccionEncontrado != "")
            {
                cbSeccion.SelectedIndex = 0;
            }
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
            RegEstanterias.Clear();
            if (!iSeccion.Estanterias)
            {
            }
            else
            {
                RegEstanterias = Funciones.LlenarEstanterias("M", iSeccion.Clave, cbPasillo.SelectedItem.ToString());
                cbEstanteria.ItemsSource = RegEstanterias;
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

        private void CargarNieveles()
        {
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

        private void BuscarCapturas()
        {
            string Parametros = string.Empty;
            if (consulta)
                Parametros = $"0,{Global.HerramientaEnInventario.CodigoActual}";
            else
                Parametros = $"{Global.HerramientaEnInventario.id},null";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_DetalleInventarioAlmacenxArticulo");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp != "[]")
                {
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        RegInventariosDetalle.Add(new clsInventarioDetalle
                        {
                            ID = int.Parse(r[0].ToString()),
                            folioInventario = r[1].ToString(),
                            idReferencia = r[2].ToString(),
                            Seccion = r[3].ToString(),
                            DescripcionSeccion = r[18].ToString(),
                            Pasillo = int.Parse(r[4].ToString()),
                            Estanteria = r[5].ToString(),
                            DescripcionEstanteria = r[19].ToString(),
                            Nivel = int.Parse(r[6].ToString()),
                            Tarima = int.Parse(r[7].ToString()),
                            Caja = int.Parse(r[8].ToString()),
                            Familia = r[9].ToString(),
                            Linea = r[10].ToString(),
                            Grupo = r[11].ToString(),
                            CodigoArticulo = r[12].ToString(),
                            Existencia = double.Parse(r[13].ToString()),
                            UnidadControl = r[14].ToString(),
                            Sucursal = r[15].ToString(),
                            nserie = r[16].ToString(),
                            Clasificacion = r[17].ToString()
                        });
                        inventariado = true;
                    }
                }
            }
            ItemsListView.ItemsSource = RegInventariosDetalle;
        }

        private void BuscarNumeroSerie()
        {
            listaSeries.Clear();
            cbSeries.ItemsSource = null;
            List<string> lst = new List<string>();
            lst = Funciones.LlenarSeries(Global.HerramientaEnInventario.CodigoActual);
            foreach (string item in lst)
            {
                bool existe = false;
                if (RegInventariosDetalle.Count > 0)
                {
                    foreach (clsInventarioDetalle tmp in RegInventariosDetalle)
                    {
                        if (item == tmp.nserie) existe = true;
                    }
                }
                if (!existe)
                    listaSeries.Add(item);
            }

            cbSeries.ItemsSource = listaSeries;
        }

        private void LeerArticuloEnInventario(bool Articulo = false)
        {
            lblFamilia.Text = Global.HerramientaEnInventario.desc_familia;
            lblLinea.Text = Global.HerramientaEnInventario.desc_linea;
            lblGrupo.Text = Global.HerramientaEnInventario.desc_grupo;
            lblCodigo.Text = Global.HerramientaEnInventario.CodigoActual;
            lblDescripcion.Text = Global.HerramientaEnInventario.Descripcion;
            lblMedida.Text = Global.HerramientaEnInventario.DescMedida;
            lblMarca.Text = Global.HerramientaEnInventario.DescMarca;
            lblParte.Text = Global.HerramientaEnInventario.DescParte;
            CargarSeccion();
            BuscarCapturas();
            BuscarNumeroSerie();
            if (Global.HerramientaEnInventario.ExisUbi == 1)
            {
                if (Global.HerramientaEnInventario.Seccion != "")
                {
                    cbSeccion.SelectedIndex = int.Parse(Global.HerramientaEnInventario.Seccion) - 1;
                    cbPasillo.SelectedIndex = int.Parse(Global.HerramientaEnInventario.Pasillo) - 1;

                    int valor = 0;
                    foreach (clsEstanteria item in cbEstanteria.ItemsSource)
                    {
                        if (item.Clave == Global.HerramientaEnInventario.Estanteria)
                        {
                            cbEstanteria.SelectedIndex = valor;
                            break;
                        }
                        valor = valor + 1;
                    }
                    cbNivel.SelectedIndex = int.Parse(Global.HerramientaEnInventario.Nivel) - 1;
                    if (cbTarima.IsVisible)
                        cbTarima.SelectedIndex = int.Parse(Global.HerramientaEnInventario.Tarima) - 1;
                    if (cbCaja.IsVisible)
                        cbCaja.SelectedIndex = int.Parse(Global.HerramientaEnInventario.Contenedor) - 1;
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
                cbSeries.IsEnabled = !inventariado;
                txtCantidad.IsEnabled = !inventariado;
                cbClasificacion.IsEnabled = !inventariado;
                btnAgregar.IsEnabled = !inventariado;
                btnGuardar.IsEnabled = !inventariado;
                //fvalidacion.IsEnabled = !inventariado;
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
            if (capturando) return;
            capturando = true;
            cbNivel.Items.Clear();
            cbTarima.Items.Clear();
            cbCaja.Items.Clear();
            CargarNieveles();
            CargarTarimas();
            CargarCajas();
            capturando = false;
        }

        private void cbSeries_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (cbClasificacion.SelectedItem != null)
            {
                if (cbSeries.SelectedItem == null && cbClasificacion.SelectedItem.ToString() == "USADO")
                {
                    txtCantidad.IsEnabled = true;
                    txtCantidad.Text = "1";
                    return;
                }
                txtCantidad.IsEnabled = false;
                txtCantidad.Text = "1";
                return;
            }
            if (cbSeries.SelectedItem != null)
            {
                txtCantidad.IsEnabled = false;
                txtCantidad.Text = "1";
                return;
            }
            else
            {
                txtCantidad.IsEnabled = true;
                txtCantidad.Text = "1";
                return;
            }
        }

        private void cbClasificacion_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //TODO: Se Desactiva validacion para Generar Series solo para Herramientas Usadas.
            //if (cbClasificacion.SelectedItem.ToString() == "NUEVO")
            //{
            //    txtCantidad.IsEnabled = false;
            //    txtCantidad.Text = "1";
            //}
            //else
            //{
            if (cbSeries.SelectedItem == null || string.IsNullOrEmpty(cbSeries.SelectedItem.ToString()))
            {
                txtCantidad.IsEnabled = true;
                txtCantidad.Text = "1";
                return;
            }
            //txtCantidad.IsEnabled = false;
            //txtCantidad.Text = "1";
            //}
        }

        private void btnAgregar_Clicked(Object sender, EventArgs e)
        {
            clsSeccion iSeccion;
            clsEstanteria iEstanteria;
            DataTable NumerosSerieGenerados = new DataTable();

            string sSeccion = "";
            string sDescSeccion = "";
            int sPasillo = 0;
            string sEstanteria = "";
            string sDescEstanteria = "";
            string sNivel = "";
            string sClasificacion = "";

            try
            {
                iSeccion = (clsSeccion)cbSeccion.SelectedItem;
                sDescSeccion = iSeccion.Descripcion;
                sSeccion = int.Parse(iSeccion.Clave).ToString("D2");
            }
            catch (Exception)
            {
                sSeccion = "";
            }

            try
            {
                iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
                sDescEstanteria = iEstanteria.Descripcion;
                sPasillo = iEstanteria.Pasillo;
                sEstanteria = int.Parse(iEstanteria.Clave).ToString("D2");
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
            string sSerie = "";

            try
            {
                iTarima = int.Parse(cbTarima.SelectedItem.ToString());
            }
            catch (Exception)
            {
                iTarima = 0;
            }

            try
            {
                iCaja = int.Parse(cbCaja.SelectedItem.ToString());
            }
            catch (Exception)
            {
                iCaja = 0;
            }

            try
            {
                dCantidad = double.Parse(txtCantidad.Text);
            }
            catch (Exception)
            {
                dCantidad = 0;
            }

            if (string.IsNullOrEmpty(sSeccion) || sPasillo == 0 || string.IsNullOrEmpty(sEstanteria))
            {
                DisplayAlertAsync("Informacion", "La ubicacion debe se llenado correctamente (Seccion, Pasillo, Estanteria)", "OK");
                return;
            }

            if (dCantidad == 0)
            {
                DisplayAlertAsync("Informacion", "La cantidad no puede ser 0 (cero)", "OK");
                return;
            }

            try
            {
                sSerie = cbSeries.SelectedItem.ToString();
                listaSeries.Remove(sSerie);
            }
            catch (Exception)
            {
                sSerie = "";
            }

            try
            {
                sClasificacion = cbClasificacion.SelectedItem.ToString();
            }
            catch
            {
                sClasificacion = "";
            }

            if (sClasificacion == "")
            {
                DisplayAlertAsync("Informacion", "Debe seleccionar la Clasificacion del Articulo a ingresar", "OK");
                return;
            }

            // Si no tiene una serie asignada y es herramienta Usada, se generan nuevas series para el articulo
            // para no tener que generar una entrada
            if (string.IsNullOrEmpty(sSerie))
            {
                //TODO: Se Desactiva validacion para Generar Series solo para Herramientas Usadas.
                //if (sClasificacion == "NUEVO")
                //{
                //    DisplayAlert("Informacion", "Debe seleccionar una Serie para la Herramienta a registrar", "OK");
                //    return;
                //}

                // Se Generan las nuevas Series para el Articulo
                GenNumerosSeries.Add(new CatalogoArticuloNumeroSeries
                {
                    Codigo_Articulo = Global.HerramientaEnInventario.CodigoActual,
                    Cantidad = int.Parse(dCantidad.ToString()),
                    Status_Herramienta = "AA",
                    Aplica = true,
                    Psucursal = "M",
                    Seccion = sSeccion,
                    Pasillo = sPasillo,
                    Estanteria = sEstanteria,
                    Nivel = int.Parse(sNivel),
                    Tarima = iTarima,
                    Contenedor = iCaja,
                    FolioInventario = Global.FolioInventario
                });

                NumerosSerieGenerados = ConfigAPI.PostAPI_GenerarNumerosSeries("api/InventarioAlmacenH", "GenerarNumerosSerie", GenNumerosSeries);
                foreach (DataRow r in NumerosSerieGenerados.Rows)
                {
                    bool existe = false;
                    foreach (clsInventarioDetalle tmp in RegInventariosDetalle)
                    {
                        if (r[1].ToString() == tmp.nserie)
                        {
                            existe = true;
                        }
                    }

                    if (!existe)
                    {
                        RegInventariosDetalle.Add(new clsInventarioDetalle
                        {
                            folioInventario = r[11].ToString(),
                            idReferencia = Global.HerramientaEnInventario.id.ToString(),
                            CodigoArticulo = r[0].ToString(),
                            Sucursal = r[4].ToString(),
                            Seccion = r[5].ToString(),
                            Pasillo = int.Parse(r[6].ToString()),
                            Estanteria = r[7].ToString(),
                            Nivel = int.Parse(r[8].ToString()),
                            Familia = Global.HerramientaEnInventario.ClaveFamilia,
                            Linea = Global.HerramientaEnInventario.ClaveLinea,
                            Grupo = Global.HerramientaEnInventario.ClaveGrupo,
                            Tarima = int.Parse(r[9].ToString()),
                            Caja = int.Parse(r[10].ToString()),
                            UnidadControl = Global.HerramientaEnInventario.UnidadControl,
                            Existencia = 1,
                            DescFamilia = Global.HerramientaEnInventario.desc_familia,
                            nserie = r[1].ToString(),
                            Clasificacion = sClasificacion,
                            Usuario = Global.clave_usuario
                        });
                    }
                }
                GenNumerosSeries.Clear();
                NavigationPage.SetHasBackButton(this, false);
            }
            else
            {
                foreach (clsInventarioDetalle tmp in RegInventariosDetalle)
                {
                    if (sSerie == tmp.nserie)
                    {
                        DisplayAlertAsync("Informacion", "Serie ya esta capturada", "OK");
                        cbSeries.SelectedIndex = 0;
                        return;
                    }
                }

                RegInventariosDetalle.Add(new clsInventarioDetalle
                {
                    folioInventario = Global.FolioInventario,
                    idReferencia = Global.HerramientaEnInventario.id.ToString(),
                    CodigoArticulo = Global.HerramientaEnInventario.CodigoActual,
                    Sucursal = "M",
                    Seccion = sSeccion,
                    DescripcionSeccion = sDescSeccion,
                    Pasillo = sPasillo,
                    Estanteria = sEstanteria,
                    DescripcionEstanteria = sDescEstanteria,
                    Nivel = int.Parse(sNivel),
                    Familia = Global.HerramientaEnInventario.ClaveFamilia,
                    Linea = Global.HerramientaEnInventario.ClaveLinea,
                    Grupo = Global.HerramientaEnInventario.ClaveGrupo,
                    Tarima = iTarima,
                    Caja = iCaja,
                    UnidadControl = Global.HerramientaEnInventario.UnidadControl,
                    Existencia = double.Parse(txtCantidad.Text),
                    DescFamilia = Global.HerramientaEnInventario.desc_familia,
                    nserie = sSerie,
                    Clasificacion = sClasificacion,
                    Usuario = Global.clave_usuario
                });
            }

            BuscarNumeroSerie();
            txtCantidad.Text = "1";
            cbSeries.SelectedIndex = 0;
            cbClasificacion.SelectedIndex = 0;

            ItemsListView.ItemsSource = RegInventariosDetalle;
        }

        private async void btnEliminarLista_Clicked(Object sender, EventArgs e)
        {
            if (inventariado)
            {
                await DisplayAlertAsync("Alerta", "Ya no es posible Modificar el Inventario", "OK");
                return;
            }
            cbSeries.ItemsSource = null;
            clsInventarioDetalle Item_;
            Item_ = (sender as MenuItem).BindingContext as clsInventarioDetalle;
            listaSeries.Add(Item_.nserie);
            listaSeries.Sort();
            cbSeries.ItemsSource = listaSeries;
            RegInventariosDetalle.Remove(Item_);
        }

        private async void btnGuardar_Clicked(Object sender, EventArgs e)
        {
            if (inventariado)
            {
                await DisplayAlertAsync("Alerta", "Ya no es posible Modificar el Inventario", "OK");
                return;
            }
            if (RegInventariosDetalle.Count == 0)
            {
                await DisplayAlertAsync("Error", "No es posible guardar, no se ha capturado ningun inventario.", "OK");
                return;
            }

            bool resp_Ok = false;
            var answer = await DisplayAlertAsync("Informaciòn", "Se guardará el inventario capturado y ya no podrá ser Modificado. ¿Desea Continuar?", "Si", "No");
            if (answer == false)
            { return; }

            DataTable dtResponse = ConfigAPI.PostAPI_GuardarInventario("api/InventarioAlmacenH", "GuardarInventario", RegInventariosDetalle);
            foreach (DataRow r in dtResponse.Rows)
            {
                if (r[1].ToString().Trim() == "200 OK")
                    resp_Ok = true;
            }

            if (resp_Ok)
            {
                await DisplayAlertAsync("Informacion", "Inventario capturado correctamente", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}