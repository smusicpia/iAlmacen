using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace iAlmacen.ViewModels
{
    public class ItemsViewModel_Orden : BaseViewModel_Orden
    {
        public Command LoadItemsCommand_orden { get; set; }

        public ItemsViewModel_Orden()
        {
            Title = "Lista";
            Global.Items_orden_ = new ObservableCollection<Item_orden_compra>();
            LoadItemsCommand_orden = new Command(async () => await ExecuteLoadItemsCommand_Orden());
        }

        private async Task ExecuteLoadItemsCommand_Orden()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Global.Items_orden_.Clear();

                String Filter_ = "C";
                if (Global.cfiltro_ == "R")
                {
                    Filter_ = "R";
                }

                string Parametros = $"{Global.folio_orden_},{Filter_}";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_compra");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            Item_orden_compra _item = new Item_orden_compra();
                            _item.id_orden_ = float.Parse(r[0].ToString());
                            _item.tipo_orden_ = r[1].ToString();
                            _item.folio_orden_ = r[2].ToString();
                            _item.clave_proveedor_ = r[3].ToString();
                            _item.nombre_proveedor_ = r[4].ToString();
                            _item.dirigido_ = r[5].ToString();
                            _item.moneda_ = r[8].ToString().Trim();
                            _item.subtotal_orden_ = double.Parse(r[9].ToString());
                            _item.iva_orden_ = double.Parse(r[10].ToString());
                            _item.descuento_orden_ = double.Parse(r[11].ToString());
                            _item.importe_descuento_ = double.Parse(r[12].ToString());
                            _item.importe_gastos_ = double.Parse(r[13].ToString());
                            _item.total_orden_ = double.Parse(r[14].ToString());
                            _item.tipo_cambio_ = double.Parse(r[15].ToString());
                            _item.comprador_ = r[16].ToString();
                            _item.solicitante_general_ = r[17].ToString();
                            _item.status_ = r[18].ToString();
                            _item.folio_cotizacion_ = r[19].ToString();
                            _item.surtido_ = r[20].ToString();
                            _item.psucursal_ = r[21].ToString();
                            _item.id_detalle_ = float.Parse(r[22].ToString());
                            _item.solicitante_ = r[23].ToString();
                            _item.area_ = r[24].ToString();
                            _item.nivel1_ = r[25].ToString();
                            _item.nivel2_ = r[26].ToString();
                            _item.nivel3_ = r[27].ToString();
                            _item.nivel4_ = r[28].ToString();
                            _item.cantidad_ = double.Parse(r[29].ToString());
                            _item.cantidad_original_ = double.Parse(r[29].ToString());
                            _item.codigo_articulo_ = r[30].ToString();
                            _item.concepto_ = r[31].ToString();
                            _item.medida_ = r[32].ToString();
                            _item.parte_ = r[33].ToString();
                            _item.marca_ = r[34].ToString();
                            _item.unidad_ = r[35].ToString();
                            _item.precio_item_ = double.Parse(r[36].ToString());
                            _item.precio_original_ = double.Parse(r[36].ToString());
                            _item.descuento_item_ = double.Parse(r[37].ToString());
                            _item.importe_descuento_item_ = double.Parse(r[38].ToString());
                            _item.importe_item_ = double.Parse(r[39].ToString());
                            _item.iva_item_ = r[40].ToString();
                            _item.importe_iva_item_ = double.Parse(r[41].ToString());
                            _item.consecutivo_ = r[42].ToString();
                            _item.saldo_ = double.Parse(r[43].ToString());
                            _item.saldo_original_ = double.Parse(r[43].ToString());
                            _item.codigo_familia_ = r[44].ToString();
                            _item.codigo_linea_ = r[45].ToString();
                            _item.codigo_grupo_ = r[46].ToString();
                            _item.observacion_ = r[47].ToString();

                            _item.familia_ = r[48].ToString();
                            _item.linea_ = r[49].ToString();
                            _item.grupo_ = r[50].ToString();
                            _item.herramienta_ = Boolean.Parse(r[51].ToString());
                            _item.codigo_anterior_ = r[52].ToString();
                            _item.unidad_standar_ = r[53].ToString();

                            if (r[54].ToString() == "B")
                            {
                                _item.tipo_articulo_ = false;
                            }
                            else
                            {
                                _item.tipo_articulo_ = true;
                            }

                            _item.gasto_flete_ = Boolean.Parse(r[55].ToString());

                            _item.autorizado_super_ = true;
                            _item.autorizado_admin_ = true;
                            _item.Aplica_Autorizacion_ = false;
                            _item.autorizador_ = "";
                            _item.nivel_autorizado_ = 0;

                            _item.subtotal_calculo_ = 0;
                            _item.descuento_calculo_ = 0;
                            _item.iva_calculo_ = 0;
                            _item.total_calculo_ = 0;

                            if (double.Parse(r[56].ToString()) == 0)
                            {
                                _item.Ubicaciones = false;
                            }
                            else
                            {
                                _item.Ubicaciones = true;
                            }
                            _item.uSeccion = "";
                            _item.uPasillo = "";
                            _item.uEstanteria = "";
                            _item.uNivel = "";
                            _item.uTarima = "";
                            _item.uContenedor = "";
                            _item.uCapturado = false;

                            _item.Resico = Boolean.Parse(r[57].ToString());
                            _item.ProcesoInventario = Boolean.Parse(r[58].ToString());

                            Global.Items_orden_.Add(_item);
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
}