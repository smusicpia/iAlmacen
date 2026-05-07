using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using System.Collections.ObjectModel;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmConsultaH : ContentPage
    {
        private string _result;

        public ObservableCollection<Item_RegArticulo> Items { get; set; }
        public Command LoadItemsCommand_Herramienta { get; set; }
        private ItemsViewModel_Herramienta viewModel_Herramientas;

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        private ObservableCollection<clsSeccion> RegSecciones = new ObservableCollection<clsSeccion>();
        private ObservableCollection<clsEstanteria> RegEstanterias = new ObservableCollection<clsEstanteria>();
        private ObservableCollection<RegArticulo> RegArticulos = new ObservableCollection<RegArticulo>();

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_Herramientas.LoadItemsCommand_herramienta.Execute(null);
        }

        public frmConsultaH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");

            Items = new ObservableCollection<Item_RegArticulo>();
            LoadItemsCommand_Herramienta = new Command(async () => await cargar());
            BindingContext = viewModel_Herramientas = new ItemsViewModel_Herramienta();

            CargarCategoriasFamilias();
            CargarSecciones();
        }

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

        private void rbUbicacion_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbUbicacion.IsChecked)
            {
                PanelUbicacion.IsVisible = true;
            }
            else
            {
                PanelUbicacion.IsVisible = false;
            }
        }

        private void rbConcepto_CheckedChanged(Object sender, CheckedChangedEventArgs e)
        {
            if (rbConcepto.IsChecked)
            {
                PanelDescripcion.IsVisible = true;
            }
            else
            {
                PanelDescripcion.IsVisible = false;
            }
        }

        private void CargarCategoriasFamilias()
        {
            cbFamilia.ItemsSource = Funciones.LlenarCategoriaFamilias(true);
        }

        private void CargarSecciones()
        {
            RegSecciones = Funciones.LlenarSecciones("", "", true);
            cbSeccion.ItemsSource = RegSecciones;
            cbSeccion.SelectedIndex = 0;
        }

        private void CargarEstanterias()
        {
            clsSeccion iSeccion;
            iSeccion = (clsSeccion)cbSeccion.SelectedItem;
            RegEstanterias.Clear();
            if (iSeccion == null) return;
            if (!iSeccion.Estanterias)
            {
                //cbEstanteria.IsEnabled = false;
            }
            else
            {
                RegEstanterias = Funciones.LlenarEstanterias("M", iSeccion.Clave);
                cbEstanteria.ItemsSource = RegEstanterias;
                cbEstanteria.SelectedIndex = -1;
            }
        }

        private void CargarNiveles()
        {
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbNivel.Items.Clear();
            if (iEstanteria == null)
            {
                return;
            }

            for (int i = 0; i < iEstanteria.NumeroNiveles; i++)
            {
                cbNivel.Items.Add((i + 1).ToString());
            }

            cbNivel.SelectedIndex = -1;
        }

        private void CargarTarimas()
        {
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbTarima.Items.Clear();

            for (int i = 0; i < iEstanteria.NumeroTarimas; i++)
            {
                cbTarima.Items.Add((i + 1).ToString());
            }

            cbTarima.SelectedIndex = -1;
        }

        private void CargarCajas()
        {
            clsEstanteria iEstanteria;
            iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
            cbContenedor.Items.Clear();

            int Cajas = 0;

            if (iEstanteria.Cajas)
            {
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
                    cbContenedor.Items.Add((i + 1).ToString());
                }

                cbContenedor.SelectedIndex = -1;
            }
            else
            {
                cbContenedor.SelectedIndex = -1;
            }
        }

        private void cbFamilia_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] sFamilia;
            sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
            cbLinea.ItemsSource = Funciones.LlenarCategoriaLineas(sFamilia[0].ToString().Trim());
        }

        private void cbLinea_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] sFamilia;
            string[] sLinea;
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

        private void cbSeccion_SelectedIndexChanged(Object sender, EventArgs e)
        {
            CargarEstanterias();
        }

        private void cbEstanteria_SelectedIndexChanged(Object sender, EventArgs e)
        {
            CargarNiveles();
            CargarTarimas();
            CargarCajas();
        }

        private async void btnAgregarCategoria_Clicked(Object sender, EventArgs e)
        {
            if (cbFamilia.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Alerta", "Es necesario ingresar la Familia, la Linea y el Grupo, para generar la busqueda.", "OK");
                return;
            }
            ;
            if (cbLinea.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Alerta", "Es necesario ingresar la Linea y el Grupo, para generar la busqueda.", "OK");
                return;
            }
            ;
            if (cbgrupo.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Alerta", "Es necesario ingresar el Grupo, para generar la busqueda.", "OK");
                return;
            }
            ;

            //RegArticulos.Clear();
            string[] sFamilia;
            sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
            string[] sLinea;
            sLinea = cbLinea.SelectedItem.ToString().Split('-');
            string[] sGrupo;
            sGrupo = cbgrupo.SelectedItem.ToString().Split('-');

            string Parametros = $"{sFamilia[0].ToString().Trim()},{sLinea[0].ToString().Trim()},{sGrupo[0].ToString().Trim()}";
            //HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_datos_ArticulosH");
            // Cambiar al controlador InventarioAlmacenH

            viewModel_Herramientas.Parametros = Parametros;
            viewModel_Herramientas.LoadItemsCommand_herramienta.Execute(null);

            //HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_datos_ArticulosH");
            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            //    if (response.StatusCode == HttpStatusCode.NotFound) return;
            //    string resp = reader.ReadToEnd();
            //    if (resp == "[]")
            //    {
            //        await DisplayAlertAsync("Alerta", "No se encontraron registros", "OK");
            //        return;
            //    }
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        Item_orden_compra _item = new Item_orden_compra();
            //        _item.id_orden_ = float.Parse(r[0].ToString());
            //        _item.tipo_orden_ = r[1].ToString();
            //        RegArticulos.Add(new RegArticulo
            //        {
            //            ID = int.Parse(r[0].ToString()),
            //            CodigoActual = r[1].ToString(),
            //            CodigoAnterior = r[2].ToString(),
            //            Descripcion = r[3].ToString(),
            //            ClaveFamilia = r[4].ToString(),
            //            ClaveLinea = r[5].ToString(),
            //            ClaveGrupo = r[6].ToString(),
            //            desc_familia = r[7].ToString(),
            //            desc_linea = r[8].ToString(),
            //            desc_grupo = r[9].ToString(),
            //            DescMarca = r[10].ToString(),
            //            DescMedida = r[11].ToString(),
            //            DescParte = r[12].ToString(),
            //            existencia = double.Parse(r[13].ToString()),
            //            Fisico = 0,
            //            Inventario = "0",
            //            Aplicado = "0",
            //            Fecha_ = DateTime.Now.ToShortDateString().ToString(),
            //            UnidadControl = r[14].ToString(),
            //            Costo = double.Parse(r[15].ToString()),
            //            Seccion = r[17].ToString(),
            //            Pasillo = r[18].ToString(),
            //            Estanteria = r[19].ToString(),
            //            Nivel = r[20].ToString(),
            //            Tarima = r[21].ToString(),
            //            Contenedor = r[22].ToString(),
            //            ExisUbi = double.Parse(r[23].ToString())
            //        });
            //    }
            //}

            //ItemsListView.ItemsSource = RegArticulos;
        }

        private void btnCancelar_Clicked(Object sender, EventArgs e)
        {
            RegArticulos.Clear();
        }

        private async void btnBuscarUbicacion_Clicked(Object sender, EventArgs e)
        {
            if (cbSeccion.SelectedIndex == -1)
            { return; }
            ;
            if (cbEstanteria.SelectedIndex == -1)
            { return; }
            ;

            RegArticulos.Clear();

            clsSeccion iSeccion;
            clsEstanteria iEstanteria;

            string sSeccion = "";
            string sEstanteria = "";

            try
            {
                iSeccion = (clsSeccion)cbSeccion.SelectedItem;
                sSeccion = iSeccion.Clave;
            }
            catch (Exception)
            {
                sSeccion = "0";
            }

            try
            {
                iEstanteria = (clsEstanteria)cbEstanteria.SelectedItem;
                //sPasillo = iEstanteria.Pasillo;
                sEstanteria = iEstanteria.Clave;
            }
            catch (Exception)
            {
                sEstanteria = "0";
            }

            string sNivel;
            try
            {
                sNivel = cbNivel.SelectedItem.ToString();
            }
            catch (Exception)
            {
                sNivel = "0";
            }

            string sTarima;
            try
            {
                sTarima = cbTarima.SelectedItem.ToString();
            }
            catch (Exception)
            {
                sTarima = "0";
            }

            string sContenedor;
            try
            {
                sContenedor = cbContenedor.SelectedItem.ToString();
            }
            catch (Exception)
            {
                sContenedor = "0";
            }

            string Parametros = $",,,,{sSeccion},{sEstanteria},{sNivel},{sTarima},{sContenedor}";
            viewModel_Herramientas.Parametros = Parametros;
            viewModel_Herramientas.LoadItemsCommand_herramienta.Execute(null);

            //HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_datos_ArticulosH");
            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            //    if (response.StatusCode == HttpStatusCode.NotFound) return;
            //    string resp = reader.ReadToEnd();
            //    if (resp == "[]")
            //    {
            //        await DisplayAlertAsync("Alerta", "No se encontraron registros", "OK");
            //        return;
            //    }
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        Item_orden_compra _item = new Item_orden_compra();
            //        _item.id_orden_ = float.Parse(r[0].ToString());
            //        _item.tipo_orden_ = r[1].ToString();
            //        RegArticulos.Add(new RegArticulo
            //        {
            //            ID = int.Parse(r[0].ToString()),
            //            CodigoActual = r[1].ToString(),
            //            CodigoAnterior = r[2].ToString(),
            //            Descripcion = r[3].ToString(),
            //            ClaveFamilia = r[4].ToString(),
            //            ClaveLinea = r[5].ToString(),
            //            ClaveGrupo = r[6].ToString(),
            //            desc_familia = r[7].ToString(),
            //            desc_linea = r[8].ToString(),
            //            desc_grupo = r[9].ToString(),
            //            DescMarca = r[10].ToString(),
            //            DescMedida = r[11].ToString(),
            //            DescParte = r[12].ToString(),
            //            existencia = double.Parse(r[13].ToString()),
            //            Fisico = 0,
            //            Inventario = "0",
            //            Aplicado = "0",
            //            Fecha_ = DateTime.Now.ToShortDateString().ToString(),
            //            UnidadControl = r[14].ToString(),
            //            Costo = double.Parse(r[15].ToString()),
            //            Seccion = r[17].ToString(),
            //            Pasillo = r[18].ToString(),
            //            Estanteria = r[19].ToString(),
            //            Nivel = r[20].ToString(),
            //            Tarima = r[21].ToString(),
            //            Contenedor = r[22].ToString(),
            //            ExisUbi = double.Parse(r[23].ToString())
            //        });
            //    }
            //}
            //ItemsListView.ItemsSource = RegArticulos;
        }

        private async void btnBuscarFiltro_Clicked(Object sender, EventArgs e)
        {
            if (txtFiltro.Text == "")
            { return; }
            ;

            //RegArticulos.Clear();
            string Parametros = $",,,{txtFiltro.Text.Trim()}";
            viewModel_Herramientas.Parametros = Parametros;
            viewModel_Herramientas.LoadItemsCommand_herramienta.Execute(null);

            ////HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_datos_ArticulosH");
            //// Cambiar al controlador InventarioAlmacenH
            //HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_datos_ArticulosH");
            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            //    if (response.StatusCode == HttpStatusCode.NotFound) return;
            //    string resp = reader.ReadToEnd();
            //    if (resp == "[]")
            //    {
            //        await DisplayAlertAsync("Alerta", "No se encontraron registros", "OK");
            //        return;
            //    }
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        Item_orden_compra _item = new Item_orden_compra();
            //        _item.id_orden_ = float.Parse(r[0].ToString());
            //        _item.tipo_orden_ = r[1].ToString();
            //        RegArticulos.Add(new RegArticulo
            //        {
            //            ID = int.Parse(r[0].ToString()),
            //            CodigoActual = r[1].ToString(),
            //            CodigoAnterior = r[2].ToString(),
            //            Descripcion = r[3].ToString(),
            //            ClaveFamilia = r[4].ToString(),
            //            ClaveLinea = r[5].ToString(),
            //            ClaveGrupo = r[6].ToString(),
            //            desc_familia = r[7].ToString(),
            //            desc_linea = r[8].ToString(),
            //            desc_grupo = r[9].ToString(),
            //            DescMarca = r[10].ToString(),
            //            DescMedida = r[11].ToString(),
            //            DescParte = r[12].ToString(),
            //            existencia = double.Parse(r[13].ToString()),
            //            Fisico = 0,
            //            Inventario = "0",
            //            Aplicado = "0",
            //            Fecha_ = DateTime.Now.ToShortDateString().ToString(),
            //            UnidadControl = r[14].ToString(),
            //            Costo = double.Parse(r[15].ToString()),
            //            Seccion = r[17].ToString(),
            //            Pasillo = r[18].ToString(),
            //            Estanteria = r[19].ToString(),
            //            Nivel = r[20].ToString(),
            //            Tarima = r[21].ToString(),
            //            Contenedor = r[22].ToString(),
            //            ExisUbi = double.Parse(r[23].ToString())
            //        });
            //    }
            //}
            ////ItemsListView.ItemsSource = RegArticulos;
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_RegArticulo;
            if (item == null)
                return;
            Global.HerramientaEnInventario = item;
            await Navigation.PushAsync(new frmCapturaInventarioH(true));
        }
    }
}