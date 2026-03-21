using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_Herramienta : BaseViewModel_Herramienta
{
    public ObservableCollection<Item_RegArticulo> Items { get; set; }
    public ICommand LoadItemsCommand_herramienta { get; set; }

    public string Parametros = ",,";

    public ItemsViewModel_Herramienta()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_RegArticulo>();
        LoadItemsCommand_herramienta = new Command(async () => await ExecuteLoadItemsCommand_Orden());
    }

    private async Task ExecuteLoadItemsCommand_Orden()
    {
        //if (IsBusy) return;
        //IsBusy = true;

        try
        {
            Items.Clear();
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_datos_ArticulosH");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]")
                {
                    return;
                }
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        Item_orden_compra _item = new Item_orden_compra();
                        _item.id_orden_ = float.Parse(r[0].ToString());
                        _item.tipo_orden_ = r[1].ToString();
                        Items.Add(new Item_RegArticulo
                        {
                            id = int.Parse(r[0].ToString()),
                            CodigoActual = r[1].ToString(),
                            CodigoAnterior = r[2].ToString(),
                            Descripcion = r[3].ToString(),
                            ClaveFamilia = r[4].ToString(),
                            ClaveLinea = r[5].ToString(),
                            ClaveGrupo = r[6].ToString(),
                            desc_familia = r[7].ToString(),
                            desc_linea = r[8].ToString(),
                            desc_grupo = r[9].ToString(),
                            DescMarca = r[10].ToString(),
                            DescMedida = r[11].ToString(),
                            DescParte = r[12].ToString(),
                            existencia = double.Parse(r[13].ToString()),
                            Fisico = 0,
                            Inventario = "0",
                            Aplicado = "0",
                            Fecha_ = DateTime.Now.ToShortDateString().ToString(),
                            UnidadControl = r[14].ToString(),
                            Costo = double.Parse(r[15].ToString()),
                            Seccion = r[17].ToString(),
                            Pasillo = r[18].ToString(),
                            Estanteria = r[19].ToString(),
                            Nivel = r[20].ToString(),
                            Tarima = r[21].ToString(),
                            Contenedor = r[22].ToString(),
                            ExisUbi = double.Parse(r[23].ToString())
                        });
                    }
                }
                else
                {
                    return;
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