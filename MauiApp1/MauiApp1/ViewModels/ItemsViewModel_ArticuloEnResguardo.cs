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

public class ItemsViewModel_ArticuloEnResguardo : BaseViewModel_Herramienta
{
    public ObservableCollection<Item_ArticuloEnResguardo> Items { get; set; }
    public ICommand LoadItemsCommand_articuloenresguardo { get; set; }

    public string Parametros = "";

    public ItemsViewModel_ArticuloEnResguardo()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_ArticuloEnResguardo>();
        LoadItemsCommand_articuloenresguardo = new Command(async () => await ExecuteLoadItemsCommand());
    }

    private string ValidarFecha(string Valor)
    {
        string resultado = "";
        if (Valor.Length == 0)
        {
            resultado = "";
        }
        else
        {
            resultado = Valor.Substring(1, 10);
        }
        return resultado;
    }

    private async Task ExecuteLoadItemsCommand()
    {
        try
        {
            Items.Clear();
            string Parametros = $"{Global.ResgEmpleado.clave}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_DetalleResguardoEmpleado");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Items.Add(new Item_ArticuloEnResguardo
                    {
                        folio = r[0].ToString(),
                        codigo = r[1].ToString(),
                        descripcion = r[2].ToString(),
                        marca = r[3].ToString(),
                        medida = r[4].ToString(),
                        parte = r[5].ToString(),
                        unidadmedida = "PZA",
                        cantidad = 1,
                        serie = r[8].ToString(),
                        inventario = bool.Parse(r[9].ToString()),
                        id = int.Parse(r[10].ToString()),
                        fechainventario = ValidarFecha(r[11].ToString()),
                        condicion = r[12].ToString(),
                        aplicado = bool.Parse(r[13].ToString()),
                        cerrado = bool.Parse(r[14].ToString()),
                        fechaentregado = r[15].ToString().Substring(1, 10)
                    });
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