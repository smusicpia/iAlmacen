using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
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
        //if (IsBusy)
        //    return;

        //IsBusy = true;

        try
        {
            Items.Clear();
            string Parametros = $"{Global.cfiltro_},{0}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "sp_entradas_vigilancia");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
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

                    Items.Add(_item);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}