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

public class ItemsViewModel_Vigilancia : BaseViewModel_Vigilancia
{
	public ObservableCollection<Item_entrada_vigilancia> Items { get; set; }
	public ICommand LoadItemsCommand_vigilancia { get; set; }

	public ItemsViewModel_Vigilancia()
	{
		Title = "Lista";
		Items = new ObservableCollection<Item_entrada_vigilancia>();
		LoadItemsCommand_vigilancia = new Command(async () => await ExecuteLoadItemsCommand_Vigilancia());
	}

	private async Task ExecuteLoadItemsCommand_Vigilancia()
	{
		HttpResponseMessage response;
		Items.Clear();
		try
		{
			//TODO: Modificar el parametro id=0 cuando se vuelva a publicar la api
			//string Parametros = $"{Global.cfiltro_},{0}";
			response = await SesionApi.GetAsync($"api/operacion/EntradasVigilancia?tProyecto={ConfigAPI.TipoProyecto}&tipo={Global.cfiltro_}&id=1");
			//HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "sp_entradas_vigilancia").Result;
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				//Console.WriteLine("Respuesta GET recibida: " + jsonResponse);
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				int i = 1;
				foreach (DataRow r in dt.Rows)
				{
					Item_entrada_vigilancia _item = new Item_entrada_vigilancia();
					_item.id_ = float.Parse(r[9].ToString());
					_item.captura_almacen_ = r[8].ToString();
					_item.captura_ = r[7].ToString().Trim();
					_item.vigilante_ = r[6].ToString().Trim();
					_item.observacion_ = r[5].ToString();
					_item.hora_ = r[4].ToString();
					_item.fecha_ = r[3].ToString().Trim();
					_item.proveedor_ = r[2].ToString().Trim();
					_item.codigo_proovedor_ = r[1].ToString();
					_item.folio_orden_ = r[0].ToString().Trim();
					_item.index = i++;
					Items.Add(_item);
				}
			}
			else
			{
				Console.WriteLine("Error en la petición: " + response.StatusCode);
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
}