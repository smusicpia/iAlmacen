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

public class ItemsViewModel_Inventario : BaseViewModel_Inventario
{
    public ObservableCollection<Item_RegArticulo> Items { get; set; }
    public ICommand LoadItemsCommand_inventario { get; set; }
    public bool BEncontrado { get => _bEncontrado; set => _bEncontrado = value; }
    public bool MEncontrado { get => _mEncontrado; set => _mEncontrado = value; }
    public bool Aleatorio { get => _aleatorio; set => _aleatorio = value; }
    public int AleatorioCantidad { get => _aleatorioCantidad; set => _aleatorioCantidad = value; }

    private bool _bEncontrado;
    public bool benProceso;
    private bool _mEncontrado;
    private bool _aleatorio;
    private int _aleatorioCantidad;

    public string Parametros = ",,";

    public ItemsViewModel_Inventario()
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_RegArticulo>();
        LoadItemsCommand_inventario = new Command(async () => await ExecuteLoadItemsCommand_Orden());
    }

    private async Task ExecuteLoadItemsCommand_Orden()
    {
        BEncontrado = false;
        benProceso = false;
        MEncontrado = false;
        try
        {
            Items.Clear();
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_InvCatalogo_Articulos");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    if (r[16].ToString() == "1")
                    {
                        benProceso = true;
                        continue;
                    }

                    foreach (Item_RegArticulo RegArt in Items)
                    {
                        if (RegArt.CodigoActual == r[1].ToString())
                        {
                            BEncontrado = true;
                            MEncontrado = true;
                            break;
                        }
                    }

                    if (BEncontrado)
                    { continue; }

                    if (Aleatorio)
                        if (Items.Count == AleatorioCantidad) break;

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
                        Seccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[19].ToString().Trim() : "",
                        DescSeccion = int.Parse(r[17].ToString().Trim()) == 1 ? r[20].ToString().Trim() : "",
                        Pasillo = int.Parse(r[17].ToString().Trim()) == 1 ? r[21].ToString().Trim() : "",
                        Estanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[22].ToString().Trim() : "",
                        DescEstanteria = int.Parse(r[17].ToString().Trim()) == 1 ? r[23].ToString().Trim() : "",
                        Nivel = int.Parse(r[17].ToString().Trim()) == 1 ? r[24].ToString().Trim() : "",
                        Tarima = int.Parse(r[17].ToString().Trim()) == 1 ? r[25].ToString().Trim() : "",
                        Contenedor = int.Parse(r[17].ToString().Trim()) == 1 ? r[26].ToString().Trim() : ""
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