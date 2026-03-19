using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Entrada_Almacen
{
    public partial class pageImpresionEtiquetas : ContentPage
    {
        private ObservableCollection<clsArticuloEtiqueta> EtiquetasArticulos = new ObservableCollection<clsArticuloEtiqueta>();

        public pageImpresionEtiquetas()
        {
            InitializeComponent();
            EtiquetasArticulos = new ObservableCollection<clsArticuloEtiqueta>();
        }

        protected override void OnAppearing()
        {
            ItemsListView.BeginRefresh();
            base.OnAppearing();
            CargarArticulos();
            ItemsListView.ItemsSource = EtiquetasArticulos;
            ItemsListView.EndRefresh();

            if (Global.ImpresoraEtiquetas != "")
            {
                cbImpresora.SelectedItem = Global.ImpresoraEtiquetas;
            }
        }

        private void CargarArticulos()
        {
            string Parametro = "";
            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                if (item_.cantidad_ == 0) { continue; }

                if (Parametro == "")
                {
                    Parametro = "'" + item_.codigo_articulo_.Trim() + "'";
                }
                else
                {
                    Parametro = Parametro + ",'" + item_.codigo_articulo_.Trim() + "'";
                }
            }

            string Parametros = "codigo_articulo, rtrim(descripcion_general)descripcion_general, rtrim(desc_marca)desc_marca, rtrim(desc_medida)desc_medida, rtrim(desc_parte)desc_parte, " +
            "cast((case when ImpresionEtiqueta = 0 then 0 else 1 end)as bit) 'Si', " +
            "cast(0 as bit) 'No', cast((case when ImpresionEtiqueta = 0 then 1 else 0 end)as bit) 'Nunca',CAST(0.00 AS NUMERIC(18, 2)) Cantidad,Control_Area ";
            string Condicion = $"tipo_articulo = 'B' and status_articulo = 1 and control_bascula = 0 and sin_inventario = 0 and uso_gasto = 0 " +
                "and uso_herramienta = 0 and extintor = 0 and insumo_intendencia = 0 and Combustible = 0 and uso_servicio = 0 " +
                $"and codigo_articulo in ({Parametro})";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "catalogo_articulo", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    double tmpCantidad = 0;
                    if (bool.Parse(r[5].ToString().Trim()))
                    {
                        foreach (Item_orden_compra orden in Global.Items_orden_)
                        {
                            if (orden.codigo_articulo_ == r[0].ToString().Trim())
                            {
                                tmpCantidad = orden.cantidad_;
                                break;
                            }
                        }
                    }

                    EtiquetasArticulos.Add(new clsArticuloEtiqueta
                    {
                        ID = 1,
                        codigo_articulo = r[0].ToString().Trim(),
                        descripcion_general = r[1].ToString().Trim(),
                        desc_marca = r[2].ToString().Trim(),
                        desc_medida = r[3].ToString().Trim(),
                        desc_parte = r[4].ToString().Trim(),
                        Si = bool.Parse(r[5].ToString().Trim()),
                        No = bool.Parse(r[6].ToString().Trim()),
                        Nunca = bool.Parse(r[7].ToString().Trim()),
                        Cantidad = tmpCantidad,
                        ControlArea = bool.Parse(r[9].ToString().Trim()),
                    });
                }
            }
        }

        private async void btnImprimir_Clicked(System.Object sender, System.EventArgs e)
        {
            if (cbImpresora.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Informacion", "No hay ninguna impresora seleccionada", "OK");
                return;
            }

            foreach (clsArticuloEtiqueta items in EtiquetasArticulos)
            {
                string Parametros = string.Empty;
                string Condicion = string.Empty;
                HttpWebResponse response = null;
                if (items.No)
                {
                    continue;
                }

                string sResponce = "";

                if (items.ControlArea)
                {
                    if (Global.ParametrosControlArea == "")
                    {
                        Global.ParametrosControlArea = "'" + items.codigo_articulo + "'";
                    }
                    else
                    {
                        Global.ParametrosControlArea = Global.ParametrosControlArea + ",'" + items.codigo_articulo + "'";
                    }

                    Parametros = "Control_Area=1";
                    Condicion = $"codigo_articulo='{items.codigo_articulo}'";
                    response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "catalogo_articulo", Condicion, "UPDATE");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            sResponce = "OK";
                        }
                    }
                }

                if (items.Nunca)
                {
                    Parametros = "ImpresionEtiqueta=0";
                    Condicion = $"codigo_articulo='{items.codigo_articulo}'";
                    response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "catalogo_articulo", Condicion, "UPDATE");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            sResponce = "OK";
                        }
                    }
                    continue;
                }

                string Campos = "codigo_articulo,impresora,cantidad";
                Parametros = $"'{items.codigo_articulo}','{Global.ImpresoraEtiquetas}','{items.Cantidad}'";
                Condicion = "";
                response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "impresionetiquetas", Condicion, "INSERT INTO", Campos);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        sResponce = "OK";
                    }
                }
            }
            await DisplayAlertAsync("Informacion", "Las impresiones saldran en la brevedad", "OK");
            await Navigation.PopAsync();
        }

        private void cbImpresora_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            Global.ImpresoraEtiquetas = cbImpresora.SelectedItem.ToString();
        }
    }
}