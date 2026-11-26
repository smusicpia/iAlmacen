using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;

using System.Collections.ObjectModel;
using System.Data;

namespace iAlmacen.Almacen_Refacciones.Herramientas_v2
{
    public partial class frmConsultaH : ContentPage
    {
        private string _result;
		public List<string> lFamilias { get; set; }
		public List<string> lLineas { get; set; }
		public List<string> lGrupos { get; set; }

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

			Task task = LlenarDatos();
        }

        private async Task LlenarDatos()
        {
			await CargarCategoriasFamilias();
            await CargarSecciones();
		}

		private async Task CargarCategoriasFamilias()
		{
			lFamilias = new List<String>();
			lFamilias.Add("");
			DataTable? dt = await Funciones.LlenarCategoriaFamilias(true);
			foreach (DataRow r in dt.Rows)
			{
				lFamilias.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
			}
			cbFamilia.ItemsSource = lFamilias;
		}

		private async Task CargarCategoriasLineas(string ClaveFamilia)
		{
			lLineas = new List<String>();
			lLineas.Add("");
			DataTable? dt = await Funciones.LlenarCategoriaLineas(ClaveFamilia);
			foreach (DataRow r in dt.Rows)
			{
				lLineas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
			}
			cbLinea.ItemsSource = lLineas;
		}

		private async Task CargarCategoriasGrupos(string ClaveFamilia, string ClaveLinea)
		{
			lGrupos = new List<String>();
			lGrupos.Add("");
			DataTable? dt = await Funciones.LlenarCategoriaGrupo(ClaveFamilia, ClaveLinea);
			foreach (DataRow r in dt.Rows)
			{
				lGrupos.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
			}
			cbgrupo.ItemsSource = lGrupos;
		}

		private async Task CargarSecciones()
		{
			RegSecciones = new ObservableCollection<clsSeccion>();
			DataTable? dtSecciones = await Funciones.LlenarSecciones("", "");
			foreach (DataRow r in dtSecciones.Rows)
			{
				RegSecciones.Add(new clsSeccion
				{
					ID = int.Parse(r[0].ToString().Trim()),
					Clave = r[1].ToString().Trim(),
					Descripcion = r[2].ToString().Trim(),
					Pasillos = Boolean.Parse(r[3].ToString().Trim()),
					NumeroPasillos = int.Parse(r[4].ToString().Trim()),
					Estanterias = Boolean.Parse(r[5].ToString().Trim()),
					Sucursal = r[6].ToString().Trim()
				});
			}
			cbSeccion.ItemsSource = RegSecciones;
			cbSeccion.SelectedIndex = -1;
		}

		private async Task CargarEstanterias()
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
				DataTable? dtEstanterias = await Funciones.LlenarEstanterias("M", iSeccion.Clave);
				foreach (DataRow r in dtEstanterias.Rows)
				{
					RegEstanterias.Add(new Clases.clsEstanteria
					{
						ID = int.Parse(r[0].ToString().Trim()),
						Clave = r[1].ToString().Trim(),
						Tipo = r[2].ToString().Trim(),
						Descripcion = r[3].ToString().Trim(),
						Seccion = r[4].ToString().Trim(),
						Pasillo = int.Parse(r[5].ToString().Trim()),
						NumeroNiveles = int.Parse(r[6].ToString().Trim()),
						Tarimas = Boolean.Parse(r[7].ToString().Trim()),
						NumeroTarimas = int.Parse(r[8].ToString().Trim()),
						Cajas = Boolean.Parse(r[9].ToString().Trim()),
						ReiniciarNumeracionCajas = Boolean.Parse(r[10].ToString().Trim()),
						NumeroCajas = int.Parse(r[11].ToString().Trim()),
						NumeroCajasTarima = int.Parse(r[12].ToString().Trim()),
						Sucursal = r[13].ToString().Trim(),
					});
				}
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

        private async void cbFamilia_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] sFamilia;
            sFamilia = cbFamilia.SelectedItem.ToString().Split('-');
            await CargarCategoriasLineas(sFamilia[0].ToString().Trim());
        }

        private async void cbLinea_SelectedIndexChanged(Object sender, EventArgs e)
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
            await CargarCategoriasGrupos(sFamilia[0].ToString().Trim(), sLinea[0].ToString().Trim());
        }

        private async void cbSeccion_SelectedIndexChanged(Object sender, EventArgs e)
        {
            await CargarEstanterias();
        }

        private void cbEstanteria_SelectedIndexChanged(Object sender, EventArgs e)
        {
            CargarNiveles();
            CargarTarimas();
            CargarCajas();
        }

        private async void btnBuscarFamilias_Clicked(Object sender, EventArgs e)
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

            Loading.IsVisible = true;

			viewModel_Herramientas.Parametros = Parametros;
            viewModel_Herramientas.LoadItemsCommand_herramienta.Execute(null);
            Loading.IsVisible = false;
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
        }

        //private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var item = e.CurrentSelection.FirstOrDefault() as Item_RegArticulo;
        //    if (item == null)
        //        return;
        //    //Global.HerramientaEnInventario = item;
        //    Global.ArticuloEnInventario = item;
        //    await Navigation.PushAsync(new frmCapturaInventarioH(false));
        //}
    }
}