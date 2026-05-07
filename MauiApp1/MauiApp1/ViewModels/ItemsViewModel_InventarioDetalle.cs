using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_InventarioDetalle : BaseViewModel_InventarioDetalle
{
    public ObservableCollection<Item_InventarioDetalle> Items { get; set; }
    public ICommand LoadItemsCommand_inventariodetalle { get; set; }
    public ICommand AgregarCommand { get; set; }

    public string Parametros = ",,";
    public bool inventariado = false;

    public ItemsViewModel_InventarioDetalle()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_InventarioDetalle>();
        LoadItemsCommand_inventariodetalle = new Command(async () => await ExecuteLoadItemsCommand_Cargar(""));
    }

    public ItemsViewModel_InventarioDetalle(string Parametros)
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_InventarioDetalle>();
        LoadItemsCommand_inventariodetalle = new Command(async () => await ExecuteLoadItemsCommand_Cargar(Parametros));
        AgregarCommand = new Command<Item_InventarioDetalle>(async (item) => await AgregarItem(item));
    }

    public async Task AgregarItem(Item_InventarioDetalle item)
    {
        Items.Add(item);
        inventariado = true;
    }

    private async Task ExecuteLoadItemsCommand_Cargar(string Parametros)
    {
        try
        {
            Items.Clear();
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
                        Items.Add(new Item_InventarioDetalle
                        {
                            id = int.Parse(r[0].ToString()),
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