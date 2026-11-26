using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class ItemsViewModel : BaseViewModel
{
	public ObservableCollection<Item> Items { get; set; }
	public ICommand LoadItemsCommand { get; set; }

	public ItemsViewModel()
	{
		Title = "Lista";
		Items = new ObservableCollection<Item>();
		LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
	}

	private async Task ExecuteLoadItemsCommand()
	{
		//if (IsBusy)
		//    return;
		//IsBusy = true;
		Items.Clear();
		HttpResponseMessage response;
		try
		{
			//string Parametros = $"C,'0',0,{Global.filtro_}";
			//HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/Operacion/GET", Parametros, "spget_template_reposiciones").Result;
			//using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
			//{
			//    if (response.StatusCode == HttpStatusCode.NotFound) return;
			//    string resp = reader.ReadToEnd();
			//    if (resp == "[]") return;
			//    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
			//    foreach (DataRow r in dt.Rows)
			//    {
			//        Item _item = new Item();
			//        _item.mes = float.Parse(r[0].ToString().Trim());
			//        _item.anio = float.Parse(r[1].ToString().Trim());
			//        _item.codigo_articulo = r[2].ToString().Trim();
			//        _item.codigo_anterior = r[3].ToString().Trim();
			//        _item.descripcion_general = r[4].ToString().Trim();
			//        _item.desc_marca = r[5].ToString().Trim();
			//        _item.desc_medida = r[6].ToString().Trim();
			//        _item.desc_parte = r[7].ToString().Trim();
			//        _item.cantidad = float.Parse(r[8].ToString().Trim());
			//        _item.cantidad_sistema = float.Parse(r[9].ToString().Trim());
			//        _item.unidad = r[10].ToString().Trim();
			//        _item.id = float.Parse(r[11].ToString().Trim());
			//        _item.cantidad_inventario = float.Parse(r[12].ToString().Trim());

			//        if (float.Parse(r[12].ToString().Trim()) > 0)
			//        {
			//            _item.Color_ = "Teal";
			//        }
			//        else
			//        {
			//            _item.Color_ = "White";
			//        }

			//        _item.texto_1 = _item.codigo_articulo + " - " + _item.descripcion_general + " - " + _item.desc_medida;
			//        _item.texto_2 = "Cantidad Sistema: " + _item.cantidad + "   /   " + "Cantidad Inventario: " + _item.cantidad_inventario;
			//        Items.Add(_item);
			//    }
			//}
			if (string.IsNullOrEmpty(Global.filtro_))
				response = await SesionApi.GetAsync($"api/operacion/Template_Reposiciones?tProyecto={ConfigAPI.TipoProyecto}&tipo=C");
			else
				response = await SesionApi.GetAsync($"api/operacion/Template_Reposiciones?tProyecto={ConfigAPI.TipoProyecto}&tipo=C&filtro={Global.filtro_}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow r in dt.Rows)
					{
						Item _item = new Item();
						_item.mes = float.Parse(r[0].ToString().Trim());
						_item.anio = float.Parse(r[1].ToString().Trim());
						_item.codigo_articulo = r[2].ToString().Trim();
						_item.codigo_anterior = r[3].ToString().Trim();
						_item.descripcion_general = r[4].ToString().Trim();
						_item.desc_marca = r[5].ToString().Trim();
						_item.desc_medida = r[6].ToString().Trim();
						_item.desc_parte = r[7].ToString().Trim();
						_item.cantidad = float.Parse(r[8].ToString().Trim());
						_item.cantidad_sistema = float.Parse(r[9].ToString().Trim());
						_item.unidad = r[10].ToString().Trim();
						_item.id = float.Parse(r[11].ToString().Trim());
						_item.cantidad_inventario = float.Parse(r[12].ToString().Trim());

						if (float.Parse(r[12].ToString().Trim()) > 0)
						{
							_item.Color_ = "Teal";
						}
						else
						{
							_item.Color_ = "White";
						}

						_item.texto_1 = _item.codigo_articulo + " - " + _item.descripcion_general + " - " + _item.desc_medida;
						_item.texto_2 = "Cantidad Sistema: " + _item.cantidad + "   /   " + "Cantidad Inventario: " + _item.cantidad_inventario;
						Items.Add(_item);
					}
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
}