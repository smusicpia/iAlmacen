using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.Services;
using iAlmacen.ViewModels;
using iAlmacen.Views;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

using ZXing.Net.Maui;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class Orden_Recoleccion : ContentPage
{
	public ObservableCollection<Item_ArticuloSalida> Items { get; set; }
	public Command LoadItemsCommand_Articulo { get; set; }
	private ItemsViewModel_ArticuloSalida viewModel_ArticuloSalida;

	//public Command LoadItemsCommand_Orden { get; set; }
	public ObservableCollection<Item_ArticuloSalida> ArticulosEliminados { get; set; }
	public List<string> lAreas { get; set; }
	public List<string> lCentroCostoN1 { get; set; }
	public List<string> lCentroCostoN2 { get; set; }
	public List<string> lCentroCostoN3 { get; set; }
	public List<string> lCentroCostoN4 { get; set; }
	private bool capturando = true;
	public static Boolean bUbicacionCapturada = false;
	public static string ExistenciaUbicacion = "0";
	private Item_ArticuloSalida item;

	private bool _isNavigated = false;

	//private int nUbicaciones = 0;
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

	public Orden_Recoleccion()
	{
		InitializeComponent();
		NavigationPage.SetBackButtonTitle(this, "ATRAS");
		this.Title = $"Orden de Compra ({Global.folio_orden_}) / Recoleccion ({Global.cidsql_})";

		Global.Items_recoleccion_ = new ObservableCollection<Item_Virtual_Recoleccion>();
		//Global.regArticulosSalida = new ObservableCollection<Item_ArticuloSalida>();
		ArticulosEliminados = new ObservableCollection<Item_ArticuloSalida>();

		item = new Item_ArticuloSalida();
		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_Articulo = new Command(async () => await cargar());
		string Parametros = $"{Global.cidsql_},N";
		BindingContext = viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(Global.cidsql_, "N");

		if (Global.cfiltro_ == "R")
		{
			btnGenerarListaRecoleccion.Text = "Actualizar";
		}
		buscar_datos();
	}

	protected override void OnAppearing()
	{
		//ItemsListView.BeginRefresh();
		base.OnAppearing();
		if (_isNavigated) return;

		_isNavigated = true;

		if (Global.validar == true)
		{
			Global.validar = false;
			//ItemsListView.ItemsSource = Global.regArticulosSalida;
		}
		else
		{
			Global.validar = false;
			//ItemsListView.ItemsSource = Global.regArticulosSalida;
		}
		//ItemsListView.EndRefresh();
		viewModel_ArticuloSalida.LoadItemsCommand_ArticuloSalida.Execute(null);
	}

	private async Task cargar()
	{
		var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.Camera>();
		}
		else
		{
			await DisplayAlertAsync("Error", "Los Permisos de la camara fueron denegados", "OK");
		}
	}

	private async Task LlenarArea()
	{
		lAreas = new List<String>();
		lAreas.Add("");
		DataTable? dt = await Funciones.LlenarArea();
		foreach (DataRow r in dt.Rows)
		{
			lAreas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbArea.ItemsSource = lAreas;
	}

	private async Task LlenarCentroCostoN1()
	{
		lCentroCostoN1 = new List<String>();
		lCentroCostoN1.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN1();

		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN1.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel1.ItemsSource = lCentroCostoN1;
	}

	private async Task LlenarCentroCostoN2()
	{
		lCentroCostoN2 = new List<String>();
		lCentroCostoN2.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN2();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN2.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel2.ItemsSource = lCentroCostoN2;
	}

	private async Task LlenarCentroCostoN3()
	{
		lCentroCostoN3 = new List<String>();
		lCentroCostoN3.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN3();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN3.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel3.ItemsSource = lCentroCostoN3;
	}

	private async Task LlenarCentroCostoN4()
	{
		lCentroCostoN4 = new List<String>();
		lCentroCostoN4.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN4();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN4.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel4.ItemsSource = lCentroCostoN4;
	}

	private async void buscar_datos()
	{
		capturando = false;
		await LlenarArea();
		await LlenarCentroCostoN1();
		await LlenarCentroCostoN2();
		await LlenarCentroCostoN3();
		await LlenarCentroCostoN4();

		foreach (Item_ArticuloSalida item in viewModel_ArticuloSalida.Items)
		{
			cbArea.SelectedItem = item.area.ToString().Trim() + " - " + item.desc_area.ToString().Trim();
			cbNivel1.SelectedItem = item.ccnivel1.ToString().Trim() + " - " + item.desc_nivel1.ToString().Trim();
			cbNivel2.SelectedItem = item.ccnivel2.ToString().Trim() + " - " + item.desc_nivel2.ToString().Trim();
			if (!string.IsNullOrEmpty(item.ccnivel3))
			{
				cbNivel3.SelectedItem = item.ccnivel3.ToString().Trim() + " - " + item.desc_nivel3.ToString().Trim();
			}
			if (!string.IsNullOrEmpty(item.ccnivel4))
			{
				cbNivel4.SelectedItem = item.ccnivel4.ToString().Trim() + " - " + item.desc_nivel4.ToString().Trim();
			}

			switch (item.sucursal.ToString().Trim())
			{
				case "M":
					cbSucursal.SelectedItem = "MÉRIDA";
					Global.strSucursal = "M";
					break;

				case "T":
					cbSucursal.SelectedItem = "TEBEC";
					Global.strSucursal = "T";
					break;
			}

		}

		//HttpResponseMessage response;
		//try
		//{
		//	response = await SesionApi.GetAsync($"api/Operacion/ordenRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&Estatus=N&FolioPedido={Global.cidsql_}");
		//	if (response.IsSuccessStatusCode)
		//	{
		//		string jsonResponse = await response.Content.ReadAsStringAsync();
		//		DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
		//		Global.strSucursal = dt.Rows[0][0].ToString().Trim();
		//		Global.strArea = dt.Rows[0][1].ToString().Trim();
		//		Global.strCCnivel1 = dt.Rows[0][3].ToString().Trim();
		//		Global.strCCnivel2 = dt.Rows[0][5].ToString().Trim();
		//		if (string.IsNullOrEmpty(dt.Rows[0][7].ToString()))
		//			Global.strCCnivel3 = string.Empty;
		//		else
		//			Global.strCCnivel3 = dt.Rows[0][7].ToString().Trim();

		//		if (string.IsNullOrEmpty(dt.Rows[0][9].ToString()))
		//			Global.strCCnivel4 = string.Empty;
		//		else
		//			Global.strCCnivel4 = dt.Rows[0][9].ToString().Trim();

		//		LlenarArea();
		//		LlenarCentroCostoN1();
		//		LlenarCentroCostoN2();
		//		LlenarCentroCostoN3();
		//		LlenarCentroCostoN4();
		//		foreach (DataRow r in dt.Rows)
		//		{
		//			cbArea.SelectedItem = r[1].ToString().Trim() + " - " + r[2].ToString().Trim();
		//			cbNivel1.SelectedItem = r[3].ToString().Trim() + " - " + r[4].ToString().Trim();
		//			cbNivel2.SelectedItem = r[5].ToString().Trim() + " - " + r[6].ToString().Trim();
		//			if (!string.IsNullOrEmpty(r[7].ToString().Trim()))
		//			{
		//				cbNivel3.SelectedItem = r[7].ToString().Trim() + " - " + r[8].ToString().Trim();
		//			}
		//			if (!string.IsNullOrEmpty(r[9].ToString().Trim()))
		//			{
		//				cbNivel4.SelectedItem = r[9].ToString().Trim() + " - " + r[10].ToString().Trim();
		//			}

		//			switch (r[0].ToString().Trim())
		//			{
		//				case "M":
		//					cbSucursal.SelectedItem = "MÉRIDA";
		//					Global.strSucursal = "M";
		//					break;

		//				case "T":
		//					cbSucursal.SelectedItem = "TEBEC";
		//					Global.strSucursal = "T";
		//					break;
		//			}
		//		}
		//	}
		//}
		//catch (Exception ex)
		//{
		//	Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		//}

		capturando = true;
	}

	private void LimpiarCampos()
	{
		cbArea.SelectedIndex = -1;
		cbNivel1.SelectedIndex = -1;
		cbNivel2.SelectedIndex = -1;
		cbNivel3.SelectedIndex = -1;
		cbNivel4.SelectedIndex = -1;

		Global.Editando = false;
		ExistenciaUbicacion = "0";
		bUbicacionCapturada = false;
		//Global.regArticulosSalida.Clear();
	}

	private void cbSucursal_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			cbArea.SelectedIndex = -1;
			cbNivel1.Items.Clear();
			cbNivel2.Items.Clear();
			try
			{
				Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
			}
			catch (Exception ex)
			{
				Global.strArea = "";
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				cbNivel1.Items.Clear();
				cbNivel1.Items.Clear();
				cbNivel2.Items.Clear();
				return;
			}

			Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
			LlenarArea();
		}
	}

	private void cbArea_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			string[] stmp;
			cbNivel1.SelectedIndex = -1;
			cbNivel2.SelectedIndex = -1;
			cbNivel3.SelectedIndex = -1;
			cbNivel4.SelectedIndex = -1;

			try
			{
				stmp = cbArea.SelectedItem.ToString().Split('-');
				Global.strArea = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strArea = "";
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel1.SelectedIndex = -1;
				cbNivel2.SelectedIndex = -1;
				cbNivel3.SelectedIndex = -1;
				cbNivel4.SelectedIndex = -1;
				return;
			}
			LlenarCentroCostoN1();
		}
	}

	private void cbNivel1_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			string[] stmp;
			cbNivel2.SelectedIndex = -1;

			try
			{
				stmp = cbNivel1.SelectedItem.ToString().Split('-');
				Global.strCCnivel1 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";

				cbNivel2.SelectedIndex = -1;
				cbNivel3.SelectedIndex = -1;
				cbNivel4.SelectedIndex = -1;
				return;
			}
			LlenarCentroCostoN2();
		}
	}

	private void cbNivel2_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			string[] stmp;
			cbNivel3.SelectedIndex = -1;

			try
			{
				stmp = cbNivel2.SelectedItem.ToString().Split('-');
				Global.strCCnivel2 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel3.SelectedIndex = -1;
				cbNivel4.SelectedIndex = -1;
				return;
			}
			LlenarCentroCostoN3();
		}
	}

	private void cbNivel3_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			string[] stmp;
			cbNivel4.SelectedIndex = -1;

			try
			{
				stmp = cbNivel3.SelectedItem.ToString().Split('-');
				Global.strCCnivel3 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel4.SelectedIndex = -1;
				return;
			}
			LlenarCentroCostoN4();
		}
	}

	private async void imEliminar_Clicked(Object sender, EventArgs e)
	{
		var answer = await DisplayAlertAsync("Información", "¿Desea ELIMINAR el articulo?", "Si", "No");
		if (answer == false) return;

		Item_ArticuloSalida Item_;
		Item_ = (sender as MenuItem).BindingContext as Item_ArticuloSalida;

		ArticulosEliminados.Add(Item_);
		viewModel_ArticuloSalida.Items.Remove(Item_);
		//Global.regArticulosSalida.Remove(Item_);
	}

	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
				item = e.CurrentSelection.FirstOrDefault() as Item_ArticuloSalida;
				//item = e.CurrentSelection.FirstOrDefault() as clsArticuloSalida;
				if (item == null || item.noubicaciones == 1)
				{
					await DisplayAlertAsync("Informacion", "El articulo ya cuenta con ubicacion seleccionada", "OK");
					return;
				}
				if (item == null || item.noubicaciones == 0)
				{
					await DisplayAlertAsync("Informacion", "El articulo no cuenta con Ubicacion Asignada", "OK");
					return;
				}
				var barcodePage = new BarcodePage();
				barcodePage.ParentPageName = "Lectura_Codigo";
				barcodePage.TextResultGenerated += OnTextResultGenerated;
				await Navigation.PushAsync(barcodePage);
			}
			else
			{
				await DisplayAlertAsync("Opcion no Soportada", "No se puede acceder a la cámara. Por favor, otorgue el permiso para usar esta función.", "OK");
				return;
			}
		}
		else
		{
			await DisplayAlertAsync("Permiso Denegado", "No se puede acceder a la cámara. Por favor, otorgue el permiso para usar esta función.", "OK");
			return;
		}
	}

	private async Task UpdateUbicacion(string[] Ubicacion)
	{
		if (Ubicacion != null && Ubicacion.Count() == 5)
		{
			//viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(item.codigo_articulo, Ubicacion);

			string Codigo = item.codigo_articulo;
			//string[] Ubicacion;
			//Ubicacion = result.ToString().Split('U');

			//// Aquí puedes agregar la lógica para manejar la ubicación obtenida del código QR
			//DisplayAlertAsync("Ubicación Escaneada", $"Sección: {Ubicacion[0]}, Pasillo: {Ubicacion[1]}, Estantería: {Ubicacion[2]}, Nivel: {Ubicacion[3]}, Tarima: {Ubicacion[4]}, Contenedor: {Ubicacion[5]}", "OK");

			//string Parametros = "Sucursal,Seccion, (select (tmp.Clave %2B ' - ' %2B tmp.Descripcion) from CatalogoSecciones as tmp where tmp.Clave=CatalogoArticuloUbicacion.Seccion)descseccion, " +
			//"Pasillo,Estanteria, (select top(1) (tmp.Clave %2B ' - ' %2B tmp.Descripcion) from CatalogoEstanterias as tmp where tmp.Clave=CatalogoArticuloUbicacion.Estanteria)descestanteria, " +
			//"Nivel,Tarima,Contenedor,CodigoArticulo,Existencia,UnidadControl";
			//string Condicion = $"CodigoArticulo='{Codigo}' and Estanteria='{Ubicacion[1]}' and Nivel='{Ubicacion[2]}' and Tarima='{Ubicacion[3]}' and Contenedor='{Ubicacion[4]}'";
			//HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", Parametros, "wsp_execute_qwerty", "CatalogoArticuloUbicacion", Condicion, "SELECT").Result;
			//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
			//{
			//    if (response.StatusCode == HttpStatusCode.NotFound) return;
			//    string resp = reader.ReadToEnd();
			//    if (resp == "[]") return;
			//    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
			//    foreach (DataRow r in dt.Rows)
			//    {
			//        switch (item.noubicaciones)
			//        {
			//            case 0:
			//                break;

			//            case 1:
			//                break;

			//            default:
			//                item.Seccion = r[1].ToString().Trim();
			//                item.desc_seccion = r[2].ToString().Trim();
			//                item.Pasillo = double.Parse(r[3].ToString().Trim());
			//                item.Estanteria = r[4].ToString().Trim();
			//                item.desc_estanteria = r[5].ToString().Trim();
			//                item.Nivel = double.Parse(r[6].ToString().Trim());
			//                item.Tarima = double.Parse(r[7].ToString().Trim());
			//                item.Contenedor = double.Parse(r[8].ToString().Trim());
			//                ExistenciaUbicacion = r[10].ToString().Trim();
			//                bUbicacionCapturada = true;
			//                break;
			//        }
			//    }
			//}

			HttpResponseMessage response;
			try
			{
				response = await SesionApi.GetAsync($"api/Operacion/datosUbicacion?tProyecto={ConfigAPI.TipoProyecto}&articulo={Codigo}&estanteria={Ubicacion[1]}&nivel={Ubicacion[2]}&tarima={Ubicacion[3]}&contenedor={Ubicacion[4]}");
				if (response.IsSuccessStatusCode)
				{
					string jsonResponse = await response.Content.ReadAsStringAsync();
					DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
					int i = 1;
					foreach (DataRow r in dt.Rows)
					{
						switch (item.noubicaciones)
						{
							case 0:
								break;

							case 1:
								break;

							default:
								item.Seccion = r[1].ToString().Trim();
								item.desc_seccion = r[2].ToString().Trim();
								item.Pasillo = int.Parse(r[3].ToString().Trim());
								item.Estanteria = r[4].ToString().Trim();
								item.desc_estanteria = r[5].ToString().Trim();
								item.Nivel = int.Parse(r[6].ToString().Trim());
								item.Tarima = int.Parse(r[7].ToString().Trim());
								item.Contenedor = int.Parse(r[8].ToString().Trim());
								ExistenciaUbicacion = r[10].ToString().Trim();
								bUbicacionCapturada = true;
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
			}

			if (double.Parse(ExistenciaUbicacion) < 1)
			{
				await DisplayAlertAsync("Informacion", "El articulo no cuenta con existencia en la ubicacion seleccionada", "OK");
			}
		}
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

			////string Codigo = item.codigo_articulo;
			//Ubicacion = result.ToString().Split('U');
			await UpdateUbicacion(result.ToString().Split('U'));

			//// Aquí puedes agregar la lógica para manejar la ubicación obtenida del código QR
			//await DisplayAlertAsync("Ubicación Escaneada", $"Sección: {Ubicacion[0]}, Pasillo: {Ubicacion[1]}, Estantería: {Ubicacion[1]}, Nivel: {Ubicacion[2]}, Tarima: {Ubicacion[3]}, Contenedor: {Ubicacion[4]}", "OK");
		});
	}

	private async void btnGuardarListaRecoleccion_Clicked(Object sender, EventArgs e)
	{
		if (Global.strArea == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "")  // || (cbNivel3.Items.Count > 1 && Global.strCCnivel3 == "") || (cbNivel4.Items.Count > 1 && Global.strCCnivel4 == "")
		{
			await DisplayAlertAsync("Informacion", "Informacion de centros de costo incompleta", "OK");
			return;
		}

		if (viewModel_ArticuloSalida.Items.Count == 0)
		{
			await DisplayAlertAsync("Informacion", "No hay articulos agregados", "OK");
			return;
		}

		bool ubicacionesCompletas = true;
		foreach (Item_ArticuloSalida item in viewModel_ArticuloSalida.Items)
		{
			if (item.noubicaciones > 1 && (string.IsNullOrEmpty(item.Seccion) || string.IsNullOrEmpty(item.Estanteria))) //|| item.Pasillo == 0 || item.Nivel == 0
			{
				ubicacionesCompletas = false;
				break;
			}
		}

		if (!ubicacionesCompletas)
		{
			await DisplayAlertAsync("Informacion", "Hay articulos sin ubicacion", "OK");
			return;
		}

		var answer = await DisplayAlertAsync("Informaciòn", "¿Desea guardar las ubicaciones de los Articulos?", "Si", "No");
		if (answer == false)
		{
			return;
		}

		string FechaFormateada;
		string HoraFormateada;
		try
		{
			FechaFormateada = DateTime.Now.Day.ToString().PadLeft(2, '0') + '/' +
							  DateTime.Now.Month.ToString().PadLeft(2, '0') + '/' +
							  DateTime.Now.Year.ToString();
			HoraFormateada = DateTime.Now.ToString("HH:mm:ss");
		}
		catch (Exception ex)
		{
			FechaFormateada = DateTime.Now.ToShortDateString();
			HoraFormateada = DateTime.Now.ToShortTimeString();
		}

		//// Validar si se entregaron de forma completa
		//// Orden de Recoleccion si es completo es SL de lo contrario es SI
		//string status = "SI";
		//string Parametros = "count(FolioOrdenRecoleccion)";
		//string Condicion = $"FolioOrdenRecoleccion = '{Global.cidsql_}' and FolioOrdenCompra = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and (Seccion IS NULL or Pasillo IS NULL or Estanteria IS NULL)";
		//HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", Parametros, "wsp_execute_qwerty", "Detalle_OrdenRecoleccion", Condicion, "SELECT").Result;
		//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//{
		//    if (response.StatusCode == HttpStatusCode.NotFound) return;
		//    string resp = reader.ReadToEnd();
		//    if (resp == "[]") return;
		//    DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
		//    foreach (DataRow r in dt.Rows)
		//    {
		//        if (int.Parse(r[0].ToString().Trim()) == 0)
		//        {
		//            status = "SL";
		//        }
		//    }
		//}

		//// Se actualiza el Estatus de la Orden de Recoleccion si es completo es SL de lo contrario es SI
		//string sResponce = "";
		//Parametros = $"StatusPedido = '{status}'";
		//Condicion = $"id = {Global.cidsql_} and FolioOrden = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and FolioCotizacion = '{Global.folio_cotizacion_}'";
		//response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", Parametros, "ws_fn_EjecutarQuerySQL", "OrdenRecoleccion", Condicion, "UPDATE").Result;
		//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//{
		//    if (response.StatusCode == HttpStatusCode.OK)
		//    {
		//        sResponce = "OK";
		//    }
		//}

		//// Se actualiza la ubicacion de los articulos cargados
		//foreach (Item_ArticuloSalida item in Items)
		//{
		//    string strSET = $"Seccion = '{item.Seccion}', Pasillo= '{item.Pasillo}', Estanteria='{item.Estanteria}', Nivel='{item.Nivel}', Tarima='{item.Tarima}', Contenedor='{item.Contenedor}'";
		//    string strWHERE = $"cve_articulo = '{item.codigo_articulo}' and consecutivo_mov = {item.consecutivo} and FolioOrdenRecoleccion = {Global.cidsql_} and FolioOrdenCompra = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and FolioCotizacion = '{Global.folio_cotizacion_}'";
		//    response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", strSET, "ws_fn_EjecutarQuerySQL", "Detalle_OrdenRecoleccion", strWHERE, "UPDATE").Result;
		//    using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//    {
		//        if (response.StatusCode == HttpStatusCode.OK)
		//        {
		//            sResponce = "OK";
		//        }
		//    }
		//}

		//// Se agrega registro, para el Historial de Seguimiento
		//string selectText = "top 1 Num_Articulos_Requisicion, cantidad_articulos, codigo_proveedor ";
		//string fromText = "Requisiciones_En_Cotizacion ";
		//string whereText = $"folio_requisicion = '{Global.folio_requisicion_}' and folio_cotizacion = '{Global.folio_cotizacion_}' and folio_orden_compra = '{Global.folio_orden_}' ORDER By id desc";
		//DataTable Articulos;
		//response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", selectText, "wsp_execute_qwerty", fromText, whereText, "SELECT").Result;
		//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//{
		//    if (response.StatusCode == HttpStatusCode.NotFound) return;
		//    string resp = reader.ReadToEnd();
		//    if (resp == "[]") return;
		//    Articulos = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
		//}
		//foreach (DataRow r in Articulos.Rows)
		//{
		//    string strValues = $"'{Global.folio_requisicion_}','SL','{r[0].ToString().Trim()}','{Global.folio_cotizacion_}','SL','{r[1].ToString().Trim()}','0000','{Global.folio_orden_}','SL','{DateTime.Now.ToShortDateString()}'," +
		//                       $"'{DateTime.Now.ToString("HH:mm:ss")}','{Global.nombre_usuario}','false','{Global.cidsql_}', 'SL',NULL,NULL,NULL";
		//    string strCampos = $"folio_requisicion,status_requisicion,Num_Articulos_Requisicion,folio_cotizacion,status_cotizacion,cantidad_articulos,codigo_proveedor,folio_orden_compra,status_orden_compra,Fecha_Movto,Hora_Movto,Usuario,control_bascula,folio_OrdenRecoleccion,status_OrdenRecoleccion,folio_SalidaDocumento,TipoDocumento,status_SalidaDocumento";
		//    response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", strValues, "ws_fn_EjecutarQuerySQL", "Requisiciones_En_Cotizacion", Condicion, "INSERT INTO", strCampos).Result;
		//    using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//    {
		//        if (response.StatusCode == HttpStatusCode.OK)
		//        {
		//            sResponce = "OK";
		//        }
		//    }
		//}

		////Eliminar los Articulos quitados de la Orden x Recoleccion
		//if (ArticulosEliminados.Count > 0)
		//{
		//    foreach (Item_ArticuloSalida item in ArticulosEliminados)
		//    {
		//        Parametros = "nada";
		//        Condicion = $"cve_articulo='{item.codigo_articulo}' and FolioOrdenRecoleccion='{item.id}'";
		//        response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", Parametros, "ws_fn_EjecutarQuerySQL", "Detalle_OrdenRecoleccion", Condicion, "DELETE").Result;
		//        using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//        {
		//            if (response.StatusCode == HttpStatusCode.OK)
		//            {
		//                sResponce = "OK";
		//            }
		//        }
		//    }
		//}

		HttpResponseMessage response;
		try
		{
			response = await SesionApi.PostAsync($"api/Operacion/GuardarOrdenRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}&nombre_usuario={Global.nombre_usuario}", Funciones.jsonContent(viewModel_ArticuloSalida.Items));
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				int i = 1;
				foreach (DataRow r in dt.Rows)
				{
					Mail email = new Mail();
					if (dt.Rows[0][1].ToString() == "200 OK")
					{
						string Prov = $"{r[3].ToString().Trim()} - {r[4].ToString().Trim()}";
						email.Armar_Correos_OrdenCompra(r[2].ToString().Trim(), "", " Lista para su Recoleccion", "ORDEN RECOLECCION", Prov, Global.folio_requisicion_, Global.folio_orden_, Global.cidsql_, Global.folio_cotizacion_);
					}
				}

				if (ArticulosEliminados.Count > 0)
				{
					try
					{
						//Eliminar los Articulos quitados de la Orden x Recoleccion
						response = await SesionApi.DeleteAsync($"api/Operacion/EliminarDetalleRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}", Funciones.jsonContent(ArticulosEliminados));
						if (response.IsSuccessStatusCode)
						{
							await DisplayAlertAsync("Guardado Correcto", "Orden de Recoleccion" + Global.cidsql_, "OK");
						}
						else if(!response.IsSuccessStatusCode)
						{
							await DisplayAlertAsync("Alerta", "Los Articulos eliminados no se puedieron eliminar correctamente.", "OK");
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		//if (ArticulosEliminados.Count > 0)
		//{
		//	try
		//	{
		//		//Eliminar los Articulos quitados de la Orden x Recoleccion
		//		response = await SesionApi.DeleteAsync($"api/Operacion/EliminarDetalleRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}", APIService.jsonContent(ArticulosEliminados));
		//		if (!response.IsSuccessStatusCode)
		//		{
		//			await DisplayAlertAsync("Alerta", "Los Articulos eliminados no se puedieron eliminar correctamente.", "OK");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		//	}
		//}

		//await DisplayAlertAsync("Guardado Correcto", "Orden de Recoleccion" + Global.cidsql_, "OK");



		//selectText = "sol_general, cve_pro, proveedor ";
		//fromText = "or_compra ";
		//whereText = $"folio = '{Global.folio_orden_}' and folio_cotizacion = '{Global.folio_cotizacion_}' and cancelada = 0";
		//DataTable Correos = new DataTable();
		//response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", selectText, "wsp_execute_qwerty", fromText, whereText, "SELECT").Result;
		//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//{
		//    if (response.StatusCode == HttpStatusCode.NotFound) return;
		//    string resp = reader.ReadToEnd();
		//    if (resp == "[]") return;
		//    Correos = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
		//}

		capturando = false;
		LimpiarCampos();

		await Navigation.PopAsync();
	}
}