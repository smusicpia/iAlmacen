using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_Recoleccion : BaseViewModel_Recoleccion
{
    public ObservableCollection<Item_Virtual_Recoleccion> Items { get; set; }
    public Command LoadItemsCommand_recoleccion { get; set; }
    public String Estatus { get; set; }
    public string FolioPedido { get; set; }
    public Boolean Historial { get; set; }

    public ItemsViewModel_Recoleccion(string pedido, string status, bool historial = false)
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_Virtual_Recoleccion>();
        if (!historial)
            LoadItemsCommand_recoleccion = new Command(async () => await ExecuteLoadItemsCommand_recoleccion());
        else
            LoadItemsCommand_recoleccion = new Command(async () => await ExecuteLoadItemsCommand_recoleccionH());

        Estatus = status;
        FolioPedido = pedido;
        Historial = historial;
    }

    private async Task ExecuteLoadItemsCommand_recoleccion()
    {
        //if (IsBusy)
        //    return;

        //IsBusy = true;
        try
        {
            Items.Clear();
            string Parametros = "id, FolioOrden, FolioPedido, Convert(varchar(30), FechaPedido, 103) AS FechaPedido, Convert(varchar(30), HoraPedido, 108) AS HoraPedido, UsuarioPedido, StatusPedido, FolioRequisicion, FolioCotizacion, FechaConfirmado, HoraConfirmado";
            string Condicion = $"ISNULL(FolioPedido, '') = '{FolioPedido}' and StatusPedido IN ({Estatus})";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "OrdenRecoleccion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Item_Virtual_Recoleccion _item = new Item_Virtual_Recoleccion();
                    _item.id_ = float.Parse(r[0].ToString().Trim());
                    _item.folio_orden_ = r[1].ToString().Trim();
                    _item.folio_pedido_ = r[2].ToString().Trim();
                    _item.fecha_pedido_ = r[3].ToString().Trim();
                    _item.hora_pedido_ = r[4].ToString().Trim();
                    _item.usuario_pedido_ = r[5].ToString().Trim();
                    _item.status_pedido_ = r[6].ToString().Trim();
                    _item.folio_requisicion_ = r[7].ToString().Trim();
                    _item.folio_cotizacion_ = r[8].ToString().Trim();
                    _item.fecha_confirmacion_ = r[9].ToString().Trim();
                    _item.hora_confirmacion_ = r[10].ToString().Trim();
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

    private async Task ExecuteLoadItemsCommand_recoleccionH()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            Items.Clear();
            string Parametros = "id, FolioOrden, FolioPedido, Convert(varchar(30), FechaPedido, 103) AS FechaPedido, Convert(varchar(30), HoraPedido, 108) AS HoraPedido, UsuarioPedido, StatusPedido, FolioRequisicion, FolioCotizacion, FechaConfirmado, HoraConfirmado";
            string Condicion = $"StatusPedido Not In ('SP', 'SX', 'SI') and cast(FechaPedido as date) >= DATEADD(day, -30, GETDATE())";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "OrdenRecoleccion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Item_Virtual_Recoleccion _item = new Item_Virtual_Recoleccion();
                    _item.id_ = float.Parse(r[0].ToString().Trim());
                    _item.folio_orden_ = r[1].ToString().Trim();
                    _item.folio_pedido_ = r[2].ToString().Trim();
                    _item.fecha_pedido_ = r[3].ToString().Trim();
                    _item.hora_pedido_ = r[4].ToString().Trim();
                    _item.usuario_pedido_ = r[5].ToString().Trim();
                    _item.status_pedido_ = r[6].ToString().Trim();
                    _item.folio_requisicion_ = r[7].ToString().Trim();
                    _item.folio_cotizacion_ = r[8].ToString().Trim();
                    _item.fecha_confirmacion_ = r[9].ToString().Trim();
                    _item.hora_confirmacion_ = r[10].ToString().Trim();
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