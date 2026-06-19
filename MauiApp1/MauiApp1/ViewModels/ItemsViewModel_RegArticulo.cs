using iAlmacen.Models;
using iAlmacen.Views;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class ItemsViewModel_RegArticulo : BaseViewModel_RegArticulo
{
    public ObservableCollection<Item_RegArticulo> Items { get; set; }
    public ICommand LoadItemsCommand_regArticulo { get; set; }
    public bool BEncontrado { get => _bEncontrado; set => _bEncontrado = value; }

    private bool _bEncontrado;
    public bool benProceso;

    public ItemsViewModel_RegArticulo()
    {   Title = "Lista";
        Items = new ObservableCollection<Item_RegArticulo>();
        LoadItemsCommand_regArticulo = new Command(async () => await ExecuteLoadItemsCommand_Cargar());
    }

    public ItemsViewModel_RegArticulo(string Parametros)
    {
        Title = "Lista";
        Items = new ObservableCollection<Item_RegArticulo>();
        LoadItemsCommand_regArticulo = new Command(async () => await ExecuteLoadItemsCommand_Cargar(Parametros));
    }

    private async Task ExecuteLoadItemsCommand_Cargar()
    {
    }

    private async Task ExecuteLoadItemsCommand_Cargar(string Parametros)
    {
        BEncontrado = false;
        benProceso = false;
        try
        {
            Items.Clear();
            HttpResponseMessage response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacen", Parametros, "wsp_DetalleInventarioAlmacen").Result;
            using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
                int i = 1;
				int contarUbicacion = 0;
				foreach (DataRow r in dt.Rows)
                {
					if (Items.Count > 0)
                    {
                        foreach (Item_RegArticulo RegArt in Items)
                        {
                            if (RegArt.id == int.Parse(r[0].ToString()))
                            {
                                BEncontrado = true;
                                contarUbicacion++;
                                RegArt.ExisUbi = contarUbicacion;
                                RegArt.Seccion = "";
                                RegArt.DescSeccion = "";
                                RegArt.Pasillo = "";
                                RegArt.Estanteria = "";
                                RegArt.DescEstanteria = "";
                                RegArt.Nivel = "";
                                RegArt.Tarima = "";
                                RegArt.Contenedor = "";
                                break;
                            }
                        }
                    }

                    if (!BEncontrado)
                    {
                        contarUbicacion = 1;
						Items.Add(new Item_RegArticulo
                        {
                            index = i,
                            id = int.Parse(r[0].ToString()),
                            CodigoActual = r[1].ToString(),
                            CodigoAnterior = r[2].ToString(),
                            Descripcion = r[3].ToString(),
                            ClaveFamilia = r[15].ToString(),
                            ClaveLinea = r[16].ToString(),
                            ClaveGrupo = r[17].ToString(),
                            desc_familia = r[4].ToString(),
                            desc_linea = r[5].ToString(),
                            desc_grupo = r[6].ToString(),
                            DescMarca = r[7].ToString(),
                            DescMedida = r[8].ToString(),
                            DescParte = r[9].ToString(),
                            existencia = double.Parse(r[10].ToString()),
                            Fisico = Boolean.Parse(r[27].ToString()) == true ? double.Parse(r[10].ToString()) : 0,
                            Inventario = "0",
                            Aplicado = "0",
                            Fecha_ = DateTime.Now.ToShortDateString().ToString(),
                            UnidadControl = r[11].ToString(),
                            Costo = double.Parse(r[12].ToString()),
							Seccion = int.Parse(r[18].ToString().Trim()) == 1 ? r[19].ToString().Trim() : "",
							DescSeccion = int.Parse(r[18].ToString().Trim()) == 1 ? r[20].ToString().Trim() : "",
							Pasillo = int.Parse(r[18].ToString().Trim()) == 1 ? r[21].ToString().Trim() : "",
							Estanteria = int.Parse(r[18].ToString().Trim()) == 1 ? r[22].ToString().Trim() : "",
							DescEstanteria = int.Parse(r[18].ToString().Trim()) == 1 ? r[23].ToString().Trim() : "",
							Nivel = int.Parse(r[18].ToString().Trim()) == 1 ? r[24].ToString().Trim() : "",
							Tarima = int.Parse(r[18].ToString().Trim()) == 1 ? r[25].ToString().Trim() : "",
							Contenedor = int.Parse(r[18].ToString().Trim()) == 1 ? r[26].ToString().Trim() : "",
							ExisUbi = int.Parse(r[18].ToString().Trim()) == 1 ? double.Parse(r[18].ToString().Trim()) : 0,
                            Capturado = Boolean.Parse(r[27].ToString()),
						});
                        i++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
			Debug.WriteLine(ex);
			App.Current.MainPage = new LoginView();
		}
        finally
        {
            IsBusy = false;
        }
    }
}