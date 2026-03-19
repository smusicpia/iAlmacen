using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_Proterm : BaseViewModel_Proterm
{
    public ObservableCollection<Item_proterm> Items { get; set; }
    public Command LoadItemsCommand_Proterm { get; set; }

    public ItemsViewModel_Proterm()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_proterm>();
        LoadItemsCommand_Proterm = new Command(async () => await ExecuteLoadItemsCommand_proterm());
    }

    private async Task ExecuteLoadItemsCommand_proterm()
    {
        //if (IsBusy)
        //    return;

        //IsBusy = true;

        try
        {
            Items.Clear();

            string Parametros = $"L,0,0";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/ProductoTerminado", Parametros, "spget_inventario");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Item_proterm _item = new Item_proterm();
                    _item.id = float.Parse(r[0].ToString().Trim());
                    _item.codigo_articulo = r[1].ToString().Trim();
                    _item.descripcion_general = r[2].ToString().Trim();
                    _item.desc_marca = r[3].ToString().Trim();
                    _item.desc_medida = r[4].ToString().Trim();
                    _item.desc_parte = r[5].ToString().Trim();

                    Clases.Global.fecha_inventario = r[6].ToString().Trim();
                    Clases.Global.hora_inventario = r[7].ToString().Trim();

                    _item.existencia = float.Parse(r[8].ToString().Trim());
                    _item.unidad_existencia = r[9].ToString().Trim();
                    _item.inventario_cantidad = float.Parse(r[11].ToString().Trim());

                    _item.ifecha = r[10].ToString().Trim();
                    _item.ihora = r[12].ToString().Trim();

                    if (float.Parse(r[11].ToString().Trim()) > 0)
                        _item.Color_ = "Teal";
                    else
                        _item.Color_ = "White";

                    _item.texto_1 = r[1].ToString().Trim() + " - " + r[2].ToString().Trim() + " - " + r[4].ToString().Trim();
                    _item.texto_2 = "Existencia Sistema: " + r[8].ToString().Trim() + "   /   " + "Cantidad Inventario: " + r[11].ToString().Trim();

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