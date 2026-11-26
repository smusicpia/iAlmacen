using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.Views;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_ArticuloSalida : BaseViewModel_RegArticulo
{
	public ObservableCollection<Item_ArticuloSalida> Items { get; set; }
	public ICommand LoadItemsCommand_ArticuloSalida { get; set; }

	public bool BEncontrado { get => _bEncontrado; set => _bEncontrado = value; }

	private bool _bEncontrado;
	public bool benProceso;

	public ItemsViewModel_ArticuloSalida(Item_ArticuloSalida item)
	{
		Title = "Lista";
		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_ArticuloSalida = new Command(async () => await ExecuteLoadItemsCommand_Cargar(item));
	}

	public void AddItem(Item_ArticuloSalida item)
	{
		if (item != null)
		{
			Items.Add(item);
			item_ArticuloSalida = new Item_ArticuloSalida();
			//DataStore.AddItemAsync(item);
		}
	}

	public ItemsViewModel_ArticuloSalida(int folio, string tipo)
	{
		Title = "Lista";
		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_ArticuloSalida = new Command(async () => await ExecuteLoadItemsCommand_Cargar(folio, tipo));
	}

	public ItemsViewModel_ArticuloSalida(string Articulo, string[] Ubicacion)
	{
		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_ArticuloSalida = new Command(async () => await ExecuteLoadItemsCommand_Cargar(Articulo, Ubicacion));
	}

	public ItemsViewModel_ArticuloSalida(int folio, string tipo, bool Historico)
	{
		Title = "Lista";
		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_ArticuloSalida = new Command(async () => await ExecuteLoadItemsCommand_Cargar(folio, tipo, Historico));
	}

	private async Task ExecuteLoadItemsCommand_Cargar(Item_ArticuloSalida item)
	{
		try
		{
			if (item.codigo_articulo != null)
			{

			}

		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			App.Current.MainPage = new LoginView();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ExecuteLoadItemsCommand_Cargar(int folio, string tipo = "")
	{
		BEncontrado = false;
		benProceso = false;
		Items.Clear();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/Operacion/datosOrdenRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&folio_={folio}&tipo_={tipo}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				int i = 1;
				foreach (DataRow r in dt.Rows)
				{
					Global.strSucursal = dt.Rows[0][0].ToString().Trim();
					Global.strArea = dt.Rows[0][1].ToString().Trim();
					Global.strCCnivel1 = dt.Rows[0][3].ToString().Trim();
					Global.strCCnivel2 = dt.Rows[0][5].ToString().Trim();
					if (string.IsNullOrEmpty(dt.Rows[0][7].ToString()))
						Global.strCCnivel3 = string.Empty;
					else
						Global.strCCnivel3 = dt.Rows[0][7].ToString().Trim();

					if (string.IsNullOrEmpty(dt.Rows[0][9].ToString()))
						Global.strCCnivel4 = string.Empty;
					else
						Global.strCCnivel4 = dt.Rows[0][9].ToString().Trim();

					Items.Add(new Item_ArticuloSalida
					{
						index = i++,
						codigo_articulo = r[11].ToString().Trim(),
						descripcion_general = r[12].ToString().Trim(),
						desc_familia = r[13].ToString().Trim(),
						desc_linea = r[14].ToString().Trim(),
						desc_grupo = r[15].ToString().Trim(),
						desc_medida = (r[16].ToString().Trim()),
						desc_marca = r[17].ToString().Trim(),
						desc_parte = r[18].ToString().Trim(),
						consecutivo = int.Parse(r[19].ToString().Trim()),
						cantidad = double.Parse(r[20].ToString().Trim()),
						noubicaciones = double.Parse(r[21].ToString().Trim()),
						Seccion = r[23].ToString().Trim(),
						desc_seccion = r[24].ToString().Trim(),
						Pasillo = !string.IsNullOrEmpty(r[25].ToString()) ? int.Parse(r[25].ToString().Trim()) : 0,
						Estanteria = r[26].ToString().Trim(),
						desc_estanteria = r[27].ToString().Trim(),
						Nivel = !string.IsNullOrEmpty(r[28].ToString()) ? int.Parse(r[28].ToString().Trim()) : 0,
						Tarima = !string.IsNullOrEmpty(r[29].ToString()) ? int.Parse(r[29].ToString().Trim()) : 0,
						Contenedor = !string.IsNullOrEmpty(r[30].ToString()) ? int.Parse(r[30].ToString().Trim()) : 0,
						ExistenciaUbicacion = double.Parse(r[22].ToString().Trim()),
						sucursal = Global.strSucursal.ToString(),
						area = Global.strArea.ToString(),
						desc_area = r[2].ToString().Trim(),
						ccnivel1 = Global.strCCnivel1.ToString(),
						desc_nivel1 = r[4].ToString().Trim(),
						ccnivel2 = Global.strCCnivel2.ToString(),
						desc_nivel2 = r[6].ToString().Trim(),
						ccnivel3 = Global.strCCnivel3.ToString(),
						desc_nivel3 = r[8].ToString().Trim(),
						ccnivel4 = Global.strCCnivel4.ToString(),
						desc_nivel4 = r[10].ToString().Trim(),
						Responsable = r[32].ToString().Trim(),
						nom_responsable = r[33].ToString().Trim(),
						Autorizador = r[34].ToString().Trim(),
						nom_autorizador = r[35].ToString().Trim(),
						FechaConfirmado = r[36].ToString().Trim(),
						FolioOrdenRecoleccion = Global.cidsql_,
						FolioOrdenCompra = Global.folio_orden_,
						FolioRequisicion = Global.folio_requisicion_,
						FolioCotizacion = Global.folio_cotizacion_,
						unidad = r[37].ToString().Trim()
						//AreaAsignado = false
					});
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ExecuteLoadItemsCommand_Cargar(string Articulo, string[] Ubicacion)
	{
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/operacion/datosUbicacion?tProyecto={ConfigAPI.TipoProyecto}&articulo={Articulo}&estanteria={Ubicacion[1]}&nivel={Ubicacion[2]}&tarima={Ubicacion[3]}&contenedor={Ubicacion[4]}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				foreach (DataRow r in dt.Rows)
				{
					switch (Ubicacion.Length)
					{
						case 0:
							break;

						case 1:
							break;

						default:
							foreach (Item_ArticuloSalida item in Items)
							{
								if (item.codigo_articulo != Articulo) continue;
								item.Seccion = r[2].ToString().Trim();
								item.Pasillo = int.Parse(r[3].ToString().Trim());
								item.Estanteria = r[5].ToString().Trim();
								item.Nivel = int.Parse(r[6].ToString().Trim());
								item.Tarima = int.Parse(r[7].ToString().Trim());
								item.Contenedor = int.Parse(r[8].ToString().Trim());
								//ExistenciaUbicacion = r[10].ToString().Trim();
								//bUbicacionCapturada = true;
							}
							break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		//      try
		//      {
		//	string Parametros = $"Sucursal,Seccion, (select (tmp.Clave %2B ' - ' %2B tmp.Descripcion) from CatalogoSecciones as tmp where tmp.Clave=CatalogoArticuloUbicacion.Seccion)descseccion, " +
		//	"Pasillo,Estanteria, (select top(1) (tmp.Clave %2B ' - ' %2B tmp.Descripcion) from CatalogoEstanterias as tmp where tmp.Clave=CatalogoArticuloUbicacion.Estanteria)descestanteria, " +
		//	"Nivel,Tarima,Contenedor,CodigoArticulo,Existencia,UnidadControl";
		//	string Condicion = $"CodigoArticulo='{Articulo}' and Estanteria='{Ubicacion[1]}' and Nivel='{Ubicacion[2]}' and Tarima='{Ubicacion[3]}' and Contenedor='{Ubicacion[4]}'";
		//	HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/SQL", Parametros, "wsp_execute_qwerty", "CatalogoArticuloUbicacion", Condicion, "SELECT").Result;
		//	using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
		//	{
		//		if (response.StatusCode == HttpStatusCode.NotFound) return;
		//		string resp = reader.ReadToEnd();
		//		if (resp == "[]") return;
		//		DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
		//		foreach (DataRow r in dt.Rows)
		//		{
		//			switch (Ubicacion.Length)
		//			{
		//				case 0:
		//					break;

		//				case 1:
		//					break;

		//				default:
		//					foreach (Item_ArticuloSalida item in Items)
		//					{
		//						if (item.codigo_articulo != Articulo) continue;
		//						item.Seccion = r[2].ToString().Trim();
		//						item.Pasillo = double.Parse(r[3].ToString().Trim());
		//						item.Estanteria = r[5].ToString().Trim();
		//						item.Nivel = double.Parse(r[6].ToString().Trim());
		//						item.Tarima = double.Parse(r[7].ToString().Trim());
		//						item.Contenedor = double.Parse(r[8].ToString().Trim());
		//						//ExistenciaUbicacion = r[10].ToString().Trim();
		//						//bUbicacionCapturada = true;
		//					}
		//					break;
		//			}
		//		}
		//	}
		//}
		//      catch (Exception ex)
		//      {
		//	Debug.WriteLine(ex);
		//	App.Current.MainPage = new LoginView();
		//}
	}

	private async Task ExecuteLoadItemsCommand_Cargar(int folio, string tipo, bool Historico)
	{
		BEncontrado = false;
		benProceso = false;
		try
		{
			Items.Clear();

			HttpResponseMessage response;
			try
			{
				response = await SesionApi.GetAsync($"api/Operacion/datosOrdenRecoleccion?tProyecto={ConfigAPI.TipoProyecto}&folio_={folio}&tipo_={tipo}");
				if (response.IsSuccessStatusCode)
				{
					string jsonResponse = await response.Content.ReadAsStringAsync();
					DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
					int i = 1;
					foreach (DataRow r in dt.Rows)
					{
						Items.Add(new Item_ArticuloSalida
						{
							index = i++,
							codigo_articulo = r[11].ToString().Trim(),
							descripcion_general = r[12].ToString().Trim(),
							desc_familia = r[13].ToString().Trim(),
							desc_linea = r[14].ToString().Trim(),
							desc_grupo = r[15].ToString().Trim(),
							desc_medida = (r[16].ToString().Trim()),
							desc_marca = r[17].ToString().Trim(),
							desc_parte = r[18].ToString().Trim(),
							consecutivo = int.Parse(r[19].ToString().Trim()),
							cantidad = double.Parse(r[20].ToString().Trim()),
							noubicaciones = double.Parse(r[21].ToString().Trim()),
							Seccion = r[23].ToString().Trim(),
							desc_seccion = r[24].ToString().Trim(),
							Pasillo = !string.IsNullOrEmpty(r[25].ToString()) ? int.Parse(r[25].ToString().Trim()) : 0,
							Estanteria = r[26].ToString().Trim(),
							desc_estanteria = r[27].ToString().Trim(),
							Nivel = !string.IsNullOrEmpty(r[28].ToString()) ? int.Parse(r[28].ToString().Trim()) : 0,
							Tarima = !string.IsNullOrEmpty(r[29].ToString()) ? int.Parse(r[29].ToString().Trim()) : 0,
							Contenedor = !string.IsNullOrEmpty(r[30].ToString()) ? int.Parse(r[30].ToString().Trim()) : 0,
							ExistenciaUbicacion = double.Parse(r[22].ToString().Trim()),
							sucursal = Global.strSucursal.ToString(),
							area = Global.strArea.ToString(),
							ccnivel1 = Global.strCCnivel1.ToString(),
							ccnivel2 = Global.strCCnivel2.ToString(),
							ccnivel3 = Global.strCCnivel3.ToString(),
							ccnivel4 = Global.strCCnivel4.ToString(),
						});
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
			}

			//HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/GET", Parametros, "wsp_orden_Recoleccion").Result;
			//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
			//{
			//    if (response.StatusCode == HttpStatusCode.NotFound) return;
			//    string resp = reader.ReadToEnd();
			//    if (resp == "[]") return;
			//    DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
			//    int i = 1;
			//    foreach (DataRow r in dt.Rows)
			//    {
			//        Items.Add(new Item_ArticuloSalida
			//        {
			//            index = i++,
			//            codigo_articulo = r[11].ToString().Trim(),
			//            descripcion_general = r[12].ToString().Trim(),
			//            desc_familia = r[13].ToString().Trim(),
			//            desc_linea = r[14].ToString().Trim(),
			//            desc_grupo = r[15].ToString().Trim(),
			//            desc_medida = (r[16].ToString().Trim()),
			//            desc_marca = r[17].ToString().Trim(),
			//            desc_parte = r[18].ToString().Trim(),
			//            consecutivo = int.Parse(r[19].ToString().Trim()),
			//            cantidad = double.Parse(r[20].ToString().Trim()),
			//            noubicaciones = double.Parse(r[21].ToString().Trim()),
			//            Seccion = r[23].ToString().Trim(),
			//            desc_seccion = r[24].ToString().Trim(),
			//            Pasillo = !string.IsNullOrEmpty(r[25].ToString()) ? double.Parse(r[25].ToString().Trim()) : 0,
			//            Estanteria = r[26].ToString().Trim(),
			//            desc_estanteria = r[27].ToString().Trim(),
			//            Nivel = !string.IsNullOrEmpty(r[28].ToString()) ? double.Parse(r[28].ToString().Trim()) : 0,
			//            Tarima = !string.IsNullOrEmpty(r[29].ToString()) ? double.Parse(r[29].ToString().Trim()) : 0,
			//            Contenedor = !string.IsNullOrEmpty(r[30].ToString()) ? double.Parse(r[30].ToString().Trim()) : 0,
			//            ExistenciaUbicacion = double.Parse(r[22].ToString().Trim()),
			//            ccsucursal = Global.strSucursal.ToString(),
			//            ccarea = Global.strArea.ToString(),
			//            ccnivel1 = Global.strCCnivel1.ToString(),
			//            ccnivel2 = Global.strCCnivel2.ToString(),
			//            ccnivel3 = Global.strCCnivel3.ToString(),
			//            ccnivel4 = Global.strCCnivel4.ToString(),
			//        });
			//    }
			//}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
			App.Current.MainPage = new LoginView();
		}
		finally
		{
			IsBusy = false;
		}
	}
}