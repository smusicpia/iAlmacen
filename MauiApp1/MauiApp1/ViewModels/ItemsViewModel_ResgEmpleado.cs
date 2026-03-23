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

public class ItemsViewModel_ResgEmpleado : BaseViewModel_Herramienta
{
    public ObservableCollection<Item_ResgEmpleado> Items { get; set; }
    public ICommand LoadItemsCommand_resgempleado { get; set; }

    public string Parametros = "";

    public ItemsViewModel_ResgEmpleado()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_ResgEmpleado>();
        LoadItemsCommand_resgempleado = new Command(async () => await ExecuteLoadItemsCommand());
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
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_ResguardosEmpleados");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Items.Add(new Item_ResgEmpleado
                    {
                        area = r[0].ToString(),
                        clave = r[1].ToString(),
                        empleado = r[2].ToString(),
                        fecha = ValidarFecha(r[3].ToString()),
                        articulos = int.Parse(r[4].ToString())
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