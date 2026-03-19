using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace iAlmacen.ViewModels
{
    public class ItemsViewModel_Proterm_Fotos : BaseViewModel_Proterm
    {
        public ObservableCollection<Item_proterm> Items { get; set; }
        public Command LoadItemsCommand_Proterm { get; set; }

        public ItemsViewModel_Proterm_Fotos()
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
                string Parametros = $"0";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/ProductoTerminado", Parametros, "spget_lista_fotografias");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    if (resp == "[]") return;
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        Item_proterm _item = new Item_proterm();
                        _item.id = float.Parse(r[0].ToString());
                        _item.codigo_articulo = r[1].ToString();
                        _item.descripcion_general = r[2].ToString();
                        _item.desc_marca = r[6].ToString();
                        _item.desc_medida = r[7].ToString();
                        _item.desc_parte = r[8].ToString();
                        if (float.Parse(r[9].ToString()) > 0)
                            _item.Color_ = "Teal";
                        else
                            _item.Color_ = "White";

                        _item.texto_1 = r[1].ToString() + " - " + r[2].ToString() + " - " + r[7].ToString();
                        _item.texto_2 = "Marca: " + r[6].ToString() + "   /   " + "Parte: " + r[8].ToString();

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
}