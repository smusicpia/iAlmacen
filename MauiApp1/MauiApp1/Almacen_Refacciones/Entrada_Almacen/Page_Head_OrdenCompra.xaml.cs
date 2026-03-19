using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page_Head_OrdenCompra : ContentPage
    {
        public DateTime? SelectedDate;
        public string Format { get; set; }

        public Command LoadItemsCommand_Orden { get; set; }
        private ItemsViewModel_Orden viewModel_Orden;
        private ObservableCollection<DocumentoAlmacenDetalle> DocumentoAlmacenDetalles = new ObservableCollection<DocumentoAlmacenDetalle>();
        private ObservableCollection<DocumentoAlmacen> DocumentoAlmacen = new ObservableCollection<DocumentoAlmacen>();

        private double cimporte_ = 0.00;
        private double cdescuento_ = 0.00;
        private double cflete_ = 0.00;
        private double cgasto_ = 0.00;
        private double csubtotal_ = 0.00;
        private double civa_ = 0.00;
        private double cretencion_ = 0.00;
        private double ctotal_ = 0.00;
        private Boolean Resico = false;
        private double FactorResico = 0.00;
        private double ImporteResico = 0.00;
        private string lsucursal_ = "";

        private void reset_importes()
        {
            cimporte_ = 0.00;
            cdescuento_ = 0.00;
            cflete_ = 0.00;
            cgasto_ = 0.00;
            csubtotal_ = 0.00;
            civa_ = 0.00;
            cretencion_ = 0.00;
            ctotal_ = 0.00;
        }

        public Page_Head_OrdenCompra()
        {
            InitializeComponent();

            Global.Items_orden_ = new ObservableCollection<Item_orden_compra>();
            LoadItemsCommand_Orden = new Command(async () => await cargar());
            BindingContext = viewModel_Orden = new ItemsViewModel_Orden();

            viewModel_Orden.LoadItemsCommand_orden.Execute(null);

            if (Global.cfiltro_ == "R")
            {
                btn_continuar.Text = "Actualizar";
                sw_tipo_documento.IsEnabled = false;
                sw_tipo_orden.IsEnabled = false;
                sw_ubicacion.IsEnabled = false;
                importe_calculo.IsVisible = false;
            }

            buscar_datos();
            dt_fecha_documento.Date = DateTime.Now;
        }

        private async void buscar_datos()
        {
            if (Global.Items_orden_.Count > 0)
            {
                foreach (Item_orden_compra item_ in Global.Items_orden_)
                {
                    var cfolio = Global.folio_entrada_.ToString() as string;
                    lbl_nentrada.Text = cfolio.PadLeft(7, '0');
                    lbl_orden.Text = item_.folio_orden_;
                    lbl_clave_proveedor.Text = item_.clave_proveedor_;
                    lbl_nombre_proveedor.Text = item_.nombre_proveedor_;
                    lbl_fecha_entrada.Text = DateTime.Now.ToShortDateString();
                    lbl_tipo_orden.Text = item_.tipo_orden_;
                    lbl_observaciones.Text = item_.observacion_;
                    lbl_comprador.Text = item_.comprador_;
                    lbl_solicitante.Text = item_.solicitante_general_;
                    lbl_status_orden.Text = item_.status_;
                    btn_moneda.Text = item_.moneda_.Trim();
                    if (item_.moneda_ == "MXN")
                    { txt_tipo_cambio.IsEnabled = false; }
                    txt_tipo_cambio.Text = String.Format("{0:G}", item_.tipo_cambio_);
                    txt_porcentaje_descuento.Text = String.Format("{0:G}", item_.descuento_orden_);
                    lbl_descuento.Text = String.Format("{0:C}", item_.importe_descuento_);
                    lbl_subtotal.Text = String.Format("{0:C}", item_.subtotal_orden_);
                    lbl_iva.Text = String.Format("{0:C}", item_.iva_orden_);
                    lbl_total.Text = String.Format("{0:C}", item_.total_orden_);

                    switch (item_.psucursal_)
                    {
                        case "M":
                            lbl_sucursal.Text = "Sucursal: Mérida";
                            lsucursal_ = "M";
                            break;

                        case "T":
                            lbl_sucursal.Text = "Sucursal: Tebec";
                            lsucursal_ = "T";
                            break;
                    }
                    break;
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlertAsync("Alerta", "No se encontraron registros.", "OK");
                await Navigation.PopAsync();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Global.validar == true)
            {
                Global.validar = false;
                btn_finalizar.IsVisible = true;
                calcular_importes();
            }
            else
            {
                btn_finalizar.IsVisible = false;
                limpiar_importes();
            }

            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                if (item_.ProcesoInventario)
                {
                    btn_finalizar.IsVisible = false;
                    DisplayAlertAsync("Advertencia", "Esta orden cuenta con articulos en proceso de inventario", "OK");
                    return;
                }
            }
        }

        private void limpiar_importes()
        {
            lbl_importe_calculo.Text = "---";
            lbl_descuento_calculo.Text = "---";
            lbl_subtotal_calculo.Text = "---";
            lbl_iva_calculo.Text = "---";
            lbl_retencion_calculo.Text = "---";
            lbl_total_calculo.Text = "---";
            lbl_resico_calculo.Text = "---";
        }

        private void calcular_importes()
        {
            reset_importes();
            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                cimporte_ += item_.subtotal_calculo_;
                cdescuento_ += item_.descuento_calculo_;
                csubtotal_ += item_.subtotal_calculo_ - item_.descuento_calculo_;
                civa_ += item_.iva_calculo_;
                cretencion_ += item_.retencion_item_;
                ctotal_ += item_.total_calculo_ - item_.retencion_item_;
                if (item_.Resico)
                {
                    Resico = true;
                }
            }

            if (Resico)
            {
                FactorResico = 1.25;
                ImporteResico = csubtotal_ * (FactorResico / 100);
            }
            else
            {
                FactorResico = 0.00;
                ImporteResico = 0.00;
            }

            ctotal_ -= ImporteResico;

            lbl_importe_calculo.Text = cimporte_.ToString("C2");
            lbl_descuento_calculo.Text = cdescuento_.ToString("C2");
            lbl_subtotal_calculo.Text = csubtotal_.ToString("C2");
            lbl_iva_calculo.Text = civa_.ToString("C2");
            lbl_retencion_calculo.Text = cretencion_.ToString("C2");
            lbl_total_calculo.Text = ctotal_.ToString("C2");
            lbl_resico_calculo.Text = ImporteResico.ToString("C2");
        }

        private async Task cargar()
        { }

        private async void continuar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_documento.Text))
            {
                await DisplayAlertAsync("Advertencia", "Es necesario el folio del documento del proveedor", "OK");
                return;
            }

            if (txt_documento.Text.Trim() == "")
            {
                await DisplayAlertAsync("Advertencia", "Es necesario el folio del documento del proveedor", "OK");
                return;
            }
            if (Global.cfiltro_ == "R")
            {
                var answer = await DisplayAlertAsync("Informacion?", "Desea Actualizar los datos de la entrada seleccionada", "Si", "No");
                if (answer == false)
                    return;
                DateTime selectedDate = (DateTime)dt_fecha_documento.Date;
                string Parametros = $"{txt_documento.Text.PadLeft(10, '0')},{selectedDate.ToString("dd/MM/yyyy")},A,{Global.cidsql_},";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_cancelar_vigilancia");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        await DisplayAlertAsync("Advertencia", "Error al cancelar la recepción", "OK");
                        return;
                    }

                    string resp = reader.ReadToEnd();
                    if (resp == "[]") return;
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r[1].ToString() != "200 OK")
                        {
                            await DisplayAlertAsync("Advertencia", "Error al cancelar la recepción", "OK");
                            return;
                        }
                    }
                }
                return;
            }

            btn_finalizar.IsVisible = false;
            await Navigation.PushAsync(new Page_Detail_OrdenCompra());
        }

        private async void finalizar_Clicked(object sender, EventArgs e)
        {
            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                if (item_.autorizado_super_ == false || item_.autorizado_admin_ == false)
                {
                    await DisplayAlertAsync("Advertencia", "Algunos articulos han superado el limite permitido de cambio de precio, Autorice los cambios para continar", "OK");
                    return;
                }
            }

            foreach (Item_orden_compra Item_ in Global.Items_orden_)
            {
                if (Item_.Ubicaciones == true && Item_.uCapturado == false)
                {
                    await DisplayAlertAsync("Advertencia", "Algunos articulos no cuentan con Ubicacion Capturada", "OK");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(txt_documento.Text))
            {
                await DisplayAlertAsync("Advertencia", "Es necesario el folio del documento del proveedor", "OK");
                return;
            }

            if (txt_documento.Text.Trim() == "")
            {
                await DisplayAlertAsync("Advertencia", "Es necesario el folio del documento del proveedor", "OK");
                return;
            }
            Boolean cerror_ = false;

            string Parametros = $"{lbl_orden.Text},U";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_compra");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Global.folio_entrada_ = int.Parse(r[0].ToString());
                    await DisplayAlertAsync("Advertencia", $"El Articulo {r[0].ToString()} No cuenta con unidad estandar.", "OK");
                    cerror_ = true;
                }
            }

            double tmp_TipoCambio = 0;
            if (btn_moneda.Text.ToString().Trim() == "MXN")
            {
                tmp_TipoCambio = 0;
            }
            else
            {
                try
                {
                    tmp_TipoCambio = double.Parse(txt_tipo_cambio.Text);
                }
                catch (Exception)
                {
                    await DisplayAlertAsync("Advertencia", "Es necesario indicar el tipo de cambio", "OK");
                    return;
                }

                if (tmp_TipoCambio == 0)
                {
                    await DisplayAlertAsync("Advertencia", "Es necesario indicar el tipo de cambio", "OK");
                    return;
                }
            }

            var answer = await DisplayAlertAsync("Continuar", "Desea Generar la entrada de la orden de compra seleccionada?", "Si", "No");
            if (answer == false)
                return;

            btn_finalizar.IsEnabled = false;
            if (cerror_ == true)
                return;

            string Ctipo_documento_ = "";
            if (sw_tipo_documento.IsToggled == true)
                Ctipo_documento_ = "R";
            else
                Ctipo_documento_ = "F";

            string Ctipo_pago_ = "";
            if (sw_tipo_orden.IsToggled == true)
                Ctipo_pago_ = "B";
            else
                Ctipo_pago_ = "A";

            string Ctipo_envio = "";
            if (sw_ubicacion.IsToggled == true)
                Ctipo_envio = "I";
            else
                Ctipo_envio = "N";

            if (string.IsNullOrWhiteSpace(txt_referencia.Text))
                txt_referencia.Text = "";

            if (string.IsNullOrWhiteSpace(txt_serie.Text))
                txt_serie.Text = "";

            var cfolio = Global.folio_entrada_.ToString() as string;

            double conteo_ = 0.00;
            double factor_ = 1.00 / double.Parse(Global.Items_orden_.Count.ToString());
            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                conteo_ = conteo_ + 1;

                try
                {
                    await Task.Delay(50);
                    pb_log.Progress = factor_ * conteo_;
                }
                catch (Exception)
                { }

                if (item_.saldo_ == 0)
                    continue;

                double Tiva_ = 0;
                Boolean Ttasa_exe_ = false;

                switch (item_.iva_item_)
                {
                    case "16":
                        Tiva_ = 0.16;
                        break;

                    case "16.0000":
                        Tiva_ = 0.16;
                        break;

                    case "Exento":
                        Ttasa_exe_ = true;
                        break;
                }

                string Ttipo_item_ = "B";
                switch (item_.tipo_articulo_)
                {
                    case true:
                        Ttipo_item_ = "A";
                        break;
                }

                DocumentoAlmacenDetalles.Add(new DocumentoAlmacenDetalle
                {
                    id = decimal.Parse(item_.id_detalle_.ToString()),
                    folio_documento = txt_documento.Text.ToString().PadLeft(10, '0'),
                    serie_documento = item_.moneda_,
                    folio_entrada = cfolio.PadLeft(7, '0'),
                    fecha_documento = (DateTime)dt_fecha_documento.Date,
                    fecha_entrada = DateTime.Now,
                    codigo_proveedor = lbl_clave_proveedor.Text,
                    tipo_documento = Ctipo_documento_,
                    taza_iva = 16,
                    taza_exento = Ttasa_exe_,
                    orden_compra = item_.folio_orden_,
                    area = item_.area_,
                    centro_costo = item_.nivel1_,
                    referencia_documento = txt_referencia.Text,
                    codigo_articulo = item_.codigo_articulo_,
                    cantidad = decimal.Parse(item_.saldo_.ToString()),
                    precio = decimal.Parse(item_.precio_item_.ToString()),
                    porcentaje_descuento = decimal.Parse(item_.descuento_item_.ToString()),
                    importe_descuento = decimal.Parse(item_.descuento_calculo_.ToString()),
                    iva = decimal.Parse(item_.iva_calculo_.ToString()),
                    importe = decimal.Parse(item_.importe_item_.ToString()),
                    codigo_embalaje = item_.unidad_,
                    psucursal = lsucursal_,
                    tipo_cambio = tmp_TipoCambio,
                    Seccion = item_.uSeccion,
                    Pasillo = int.Parse(item_.uPasillo),
                    Estanteria = item_.uEstanteria,
                    Nivel = int.Parse(item_.uNivel),
                    Tarima = int.Parse(item_.uTarima),
                    Contenedor = int.Parse(item_.uContenedor)
                });
            }

            DataTable dtResponse = ConfigAPI.PostAPI_DocumentoAlmacenDetalle("api/Operacion", "wsp_set_detalle_entrada", DocumentoAlmacenDetalles);
            foreach (DataRow r in dtResponse.Rows)
            {
                if (r[1].ToString().Trim() == "200 OK")
                {
                    //if (int.Parse(r[2].ToString().Trim()) > 0)
                    //{
                    //    sFolioInventario = int.Parse(r[2].ToString().Trim());
                    //}
                }
            }

            await Task.Delay(2000);

            foreach (Item_orden_compra item_ in Global.Items_orden_)
            {
                double Tiva_ = 0;
                Boolean Ttasa_exe_ = false;

                switch (item_.iva_item_)
                {
                    case "16":
                        Tiva_ = 0.16;
                        break;

                    case "16.0000":
                        Tiva_ = 0.16;
                        break;

                    case "Exento":
                        Ttasa_exe_ = true;
                        break;
                }

                DocumentoAlmacen.Add(new Models.DocumentoAlmacen
                {
                    folio_documento = txt_documento.Text.ToString().PadLeft(10, '0'),
                    serie_documento = item_.moneda_,
                    folio_entrada = cfolio.PadLeft(7, '0'),
                    fecha_documento = (DateTime)dt_fecha_documento.Date,
                    fecha_entrada = DateTime.Now,
                    codigo_proveedor = lbl_clave_proveedor.Text,
                    tipo_documento = Ctipo_documento_,
                    taza_iva = decimal.Parse(Tiva_.ToString()),
                    iva = decimal.Parse(civa_.ToString()),
                    subtotal = decimal.Parse(csubtotal_.ToString()),
                    flete = decimal.Parse(cflete_.ToString()),
                    ret_flete = decimal.Parse(cretencion_.ToString()),
                    gastos = decimal.Parse(cgasto_.ToString()),
                    total = decimal.Parse(ctotal_.ToString()),
                    orden_compra = lbl_orden.Text,
                    tipo_cambio = decimal.Parse(tmp_TipoCambio.ToString()),
                    credito_contado = Ctipo_pago_,
                    nacional_importacion = Ctipo_envio,
                    referencia_documento = txt_referencia.Text,
                    tipo_moneda = btn_moneda.Text,
                    tipo_descuento = "%",
                    importe_descuento_general = 0,
                    usuario = Global.nombre_usuario,
                    psucursal = lsucursal_,
                    Resico = Resico,
                    FactorResico = decimal.Parse(FactorResico.ToString()),
                    ImporteResico = decimal.Parse(ImporteResico.ToString())
                });
            }

            dtResponse = ConfigAPI.PostAPI_DocumentoAlmacen("api/Operacion", "wsp_set_encabezado_entrada", DocumentoAlmacen);
            foreach (DataRow r in dtResponse.Rows)
            {
                if (r[1].ToString().Trim() == "200 OK")
                {
                    await DisplayAlertAsync("Informacion", "Entrada Generada Correctamente", "OK");
                    Global.validar = true;
                }
                else
                {
                    await DisplayAlertAsync("Informacion", "Error al generar el encabezado de la entrada", "OK");
                    btn_finalizar.IsEnabled = false;
                    return;
                }
            }

            string sResponce = "";
            Parametros = "Control_Area=1";
            string Condicion = $"codigo_articulo in ({Global.ParametrosControlArea}) and orden_compra='{lbl_orden.Text}' and folio_documento='{txt_documento.Text.ToString().PadLeft(10, '0')}' and codigo_proveedor='{lbl_clave_proveedor.Text}'";
            response = ConfigAPI.GetAPI("PATCH", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "detalle_documentos_almacen", Condicion);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                }
            }
            await Navigation.PopAsync();
        }

        private async void btnImprimirEtiquetas_Clicked(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Almacen_Refacciones.Entrada_Almacen.pageImpresionEtiquetas());
        }
    }
}