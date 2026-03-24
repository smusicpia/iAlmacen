using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.Services;
using iAlmacen.Views;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using ZXing.Net.Maui;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class Orden_Recoleccion : ContentPage
{
    public Command LoadItemsCommand_Orden { get; set; }
    public ObservableCollection<clsArticuloSalida> ArticulosEliminados { get; set; }
    public List<string> lCentroCostoN1 { get; set; }
    public List<string> lCentroCostoN2 { get; set; }
    public List<string> lCentroCostoN3 { get; set; }
    public List<string> lCentroCostoN4 { get; set; }
    private bool capturando = true;
    public static Boolean bUbicacionCapturada = false;
    public static string ExistenciaUbicacion = "0";

    private clsArticuloSalida item = new clsArticuloSalida();

    private int nUbicaciones = 0;
    private string _result;

    public string Result
    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged(nameof(Result));
        }
    }

    public Orden_Recoleccion()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "ATRAS");
        this.Title = $"Orden de Compra ({Global.folio_orden_}) / Recoleccion ({Global.cidsql_})";

        Global.Items_recoleccion_ = new ObservableCollection<Item_Virtual_Recoleccion>();
        Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
        ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();
        LoadItemsCommand_Orden = new Command(async () => await cargar());

        if (Global.cfiltro_ == "R")
        {
            btnGenerarListaRecoleccion.Text = "Actualizar";
        }
        buscar_datos();
    }

    protected override void OnAppearing()
    {
        //ItemsListView.BeginRefresh();
        base.OnAppearing();
        if (Global.validar == true)
        {
            Global.validar = false;
            //ItemsListView.ItemsSource = Global.regArticulosSalida;
        }
        else
        {
            Global.validar = false;
            //ItemsListView.ItemsSource = Global.regArticulosSalida;
        }
        //ItemsListView.EndRefresh();
    }

    private async Task cargar()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        else
        {
            await DisplayAlertAsync("Error", "Los Permisos de la camara fueron denegados", "OK");
        }
    }

    private void LlenarArea()
    {
        cbArea.ItemsSource = Funciones.LlenarArea();
    }

    private void LlenarCentroCostoN1()
    {
        cbNivel1.ItemsSource = Funciones.LlenarCentroCostoN1();
    }

    private void LlenarCentroCostoN2()
    {
        cbNivel2.ItemsSource = Funciones.LlenarCentroCostoN2();
    }

    private void LlenarCentroCostoN3()
    {
        cbNivel3.ItemsSource = Funciones.LlenarCentroCostoN3();
    }

    private void LlenarCentroCostoN4()
    {
        cbNivel4.ItemsSource = Funciones.LlenarCentroCostoN4();
    }

    private void buscar_datos()
    {
        capturando = false;
        string Parametros = $"{Global.cidsql_},N";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_Recoleccion");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string resp = reader.ReadToEnd();
            if (resp == "[]")
            {
                DisplayAlertAsync("Informacion", "La Orden de recoleccion no tiene detalle, Favor de validar la Informacion con el administrador del sistema.", "OK");
                btnGenerarListaRecoleccion.IsEnabled = false;
                return;
            }

            DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            Global.strSucursal = dt.Rows[0][0].ToString().Trim();
            Global.strArea = dt.Rows[0][1].ToString().Trim();
            Global.strCCnivel1 = dt.Rows[0][3].ToString().Trim();
            Global.strCCnivel2 = dt.Rows[0][5].ToString().Trim();
            Global.strCCnivel3 = !string.IsNullOrEmpty(dt.Rows[0][7].ToString().Trim()) ? dt.Rows[0][7].ToString().Trim() : string.Empty;
            Global.strCCnivel4 = !string.IsNullOrEmpty(dt.Rows[0][9].ToString().Trim()) ? dt.Rows[0][9].ToString().Trim() : string.Empty;
            LlenarArea();
            LlenarCentroCostoN1();
            LlenarCentroCostoN2();
            LlenarCentroCostoN3();
            LlenarCentroCostoN4();
            foreach (DataRow r in dt.Rows)
            {
                cbArea.SelectedItem = r[1].ToString().Trim() + " - " + r[2].ToString().Trim();
                cbNivel1.SelectedItem = r[3].ToString().Trim() + " - " + r[4].ToString().Trim();
                cbNivel2.SelectedItem = r[5].ToString().Trim() + " - " + r[6].ToString().Trim();
                if (!string.IsNullOrEmpty(r[7].ToString().Trim()))
                {
                    cbNivel3.SelectedItem = r[7].ToString().Trim() + " - " + r[8].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(r[9].ToString().Trim()))
                {
                    cbNivel4.SelectedItem = r[9].ToString().Trim() + " - " + r[10].ToString().Trim();
                }
                Global.regArticulosSalida.Add(new clsArticuloSalida
                {
                    codigo_articulo = r[11].ToString().Trim(),
                    descripcion_general = r[12].ToString().Trim(),
                    desc_familia = r[13].ToString().Trim(),
                    desc_linea = r[14].ToString().Trim(),
                    desc_grupo = r[15].ToString().Trim(),
                    desc_medida = (r[16].ToString().Trim()),
                    desc_marca = r[17].ToString().Trim(),
                    desc_parte = r[18].ToString().Trim(),
                    consecutivo = int.Parse(r[19].ToString().Trim()),
                    cantidad = double.Parse(r[20].ToString().Trim()),
                    noubicaciones = double.Parse(r[21].ToString().Trim()),
                    Seccion = r[23].ToString().Trim(),
                    desc_seccion = r[24].ToString().Trim(),
                    Pasillo = !string.IsNullOrEmpty(r[25].ToString()) ? double.Parse(r[25].ToString().Trim()) : 0,
                    Estanteria = r[26].ToString().Trim(),
                    desc_estanteria = r[27].ToString().Trim(),
                    Nivel = !string.IsNullOrEmpty(r[28].ToString()) ? double.Parse(r[28].ToString().Trim()) : 0,
                    Tarima = !string.IsNullOrEmpty(r[29].ToString()) ? double.Parse(r[29].ToString().Trim()) : 0,
                    Contenedor = !string.IsNullOrEmpty(r[30].ToString()) ? double.Parse(r[30].ToString().Trim()) : 0,
                    ExistenciaUbicacion = double.Parse(r[22].ToString().Trim()),
                    ccsucursal = Global.strSucursal.ToString(),
                    ccarea = Global.strArea.ToString(),
                    ccnivel1 = Global.strCCnivel1.ToString(),
                    ccnivel2 = Global.strCCnivel2.ToString(),
                    ccnivel3 = Global.strCCnivel3.ToString(),
                    ccnivel4 = Global.strCCnivel4.ToString(),
                });

                switch (r[0].ToString().Trim())
                {
                    case "M":
                        cbSucursal.SelectedItem = "MÉRIDA";
                        Global.strSucursal = "M";
                        break;

                    case "T":
                        cbSucursal.SelectedItem = "TEBEC";
                        Global.strSucursal = "T";
                        break;
                }
            }
        }
        capturando = true;
    }

    private void LimpiarCampos()
    {
        cbArea.SelectedIndex = -1;
        cbNivel1.SelectedIndex = -1;
        cbNivel2.SelectedIndex = -1;
        cbNivel3.SelectedIndex = -1;
        cbNivel4.SelectedIndex = -1;

        Global.Editando = false;
        ExistenciaUbicacion = "0";
        bUbicacionCapturada = false;
        Global.regArticulosSalida.Clear();
    }

    private void cbSucursal_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            cbArea.SelectedIndex = -1;
            cbNivel1.Items.Clear();
            cbNivel2.Items.Clear();
            try
            {
                Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
            }
            catch (Exception ex)
            {
                Global.strArea = "";
                Global.strCCnivel1 = "";
                Global.strCCnivel2 = "";
                cbNivel1.Items.Clear();
                cbNivel1.Items.Clear();
                cbNivel2.Items.Clear();
                return;
            }

            Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
            LlenarArea();
        }
    }

    private void cbArea_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            string[] stmp;
            cbNivel1.SelectedIndex = -1;
            cbNivel2.SelectedIndex = -1;
            cbNivel3.SelectedIndex = -1;
            cbNivel4.SelectedIndex = -1;

            try
            {
                stmp = cbArea.SelectedItem.ToString().Split('-');
                Global.strArea = stmp[0].ToString().Trim();
            }
            catch (Exception ex)
            {
                Global.strArea = "";
                Global.strCCnivel1 = "";
                Global.strCCnivel2 = "";
                Global.strCCnivel3 = "";
                Global.strCCnivel4 = "";
                cbNivel1.SelectedIndex = -1;
                cbNivel2.SelectedIndex = -1;
                cbNivel3.SelectedIndex = -1;
                cbNivel4.SelectedIndex = -1;
                return;
            }
            LlenarCentroCostoN1();
        }
    }

    private void cbNivel1_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            string[] stmp;
            cbNivel2.SelectedIndex = -1;

            try
            {
                stmp = cbNivel1.SelectedItem.ToString().Split('-');
                Global.strCCnivel1 = stmp[0].ToString().Trim();
            }
            catch (Exception ex)
            {
                Global.strCCnivel1 = "";
                Global.strCCnivel2 = "";
                Global.strCCnivel3 = "";
                Global.strCCnivel4 = "";

                cbNivel2.SelectedIndex = -1;
                cbNivel3.SelectedIndex = -1;
                cbNivel4.SelectedIndex = -1;
                return;
            }
            LlenarCentroCostoN2();
        }
    }

    private void cbNivel2_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            string[] stmp;
            cbNivel3.SelectedIndex = -1;

            try
            {
                stmp = cbNivel2.SelectedItem.ToString().Split('-');
                Global.strCCnivel2 = stmp[0].ToString().Trim();
            }
            catch (Exception ex)
            {
                Global.strCCnivel2 = "";
                Global.strCCnivel3 = "";
                Global.strCCnivel4 = "";
                cbNivel3.SelectedIndex = -1;
                cbNivel4.SelectedIndex = -1;
                return;
            }
            LlenarCentroCostoN3();
        }
    }

    private void cbNivel3_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            string[] stmp;
            cbNivel4.SelectedIndex = -1;

            try
            {
                stmp = cbNivel3.SelectedItem.ToString().Split('-');
                Global.strCCnivel3 = stmp[0].ToString().Trim();
            }
            catch (Exception ex)
            {
                Global.strCCnivel3 = "";
                Global.strCCnivel4 = "";
                cbNivel4.SelectedIndex = -1;
                return;
            }
            LlenarCentroCostoN4();
        }
    }

    private void imEliminar_Clicked(Object sender, EventArgs e)
    {
        clsArticuloSalida Item_;
        Item_ = (sender as MenuItem).BindingContext as clsArticuloSalida;
        ArticulosEliminados.Add(Item_);
        Global.regArticulosSalida.Remove(Item_);
    }

    private async void btnGuardarListaRecoleccion_Clicked(Object sender, EventArgs e)
    {
        if (Global.strArea == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "")  // || (cbNivel3.Items.Count > 1 && Global.strCCnivel3 == "") || (cbNivel4.Items.Count > 1 && Global.strCCnivel4 == "")
        {
            await DisplayAlertAsync("Informacion", "Informacion de centros de costo incompleta", "OK");
            return;
        }

        if (Global.regArticulosSalida.Count == 0)
        {
            await DisplayAlertAsync("Informacion", "No hay articulos agregados", "OK");
            return;
        }

        bool ubicacionesCompletas = true;
        foreach (clsArticuloSalida item in Global.regArticulosSalida)
        {
            if (item.noubicaciones > 1 && (string.IsNullOrEmpty(item.Seccion) || string.IsNullOrEmpty(item.Estanteria))) //|| item.Pasillo == 0 || item.Nivel == 0
            {
                ubicacionesCompletas = false;
                break;
            }
        }

        if (!ubicacionesCompletas)
        {
            await DisplayAlertAsync("Informacion", "Hay articulos sin ubicacion", "OK");
            return;
        }

        var answer = await DisplayAlertAsync("Informaciòn", "¿Desea guardar las ubicaciones de los Articulos?", "Si", "No");
        if (answer == false)
        {
            return;
        }

        string FechaFormateada;
        string HoraFormateada;
        try
        {
            FechaFormateada = DateTime.Now.Day.ToString().PadLeft(2, '0') + '/' +
                DateTime.Now.Month.ToString().PadLeft(2, '0') + '/' +
                DateTime.Now.Year.ToString();
            HoraFormateada = DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception ex)
        {
            FechaFormateada = DateTime.Now.ToShortDateString();
            HoraFormateada = DateTime.Now.ToShortTimeString();
        }

        // Validar si se entregaron de forma completo
        // Orden de Recoleccion si es completo es SL de lo contrario es SI
        string status = "SI";
        string Parametros = "count(FolioOrdenRecoleccion)";
        string Condicion = $"FolioOrdenRecoleccion = '{Global.cidsql_}' and FolioOrdenCompra = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and (Seccion IS NULL or Pasillo IS NULL or Estanteria IS NULL)";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "Detalle_OrdenRecoleccion", Condicion, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            DataTable? dt = JsonConvert.DeserializeObject<DataTable>(resp);
            foreach (DataRow r in dt.Rows)
            {
                if (int.Parse(r[0].ToString().Trim()) == 0)
                {
                    status = "SL";
                }
            }
        }

        // Se actualiza el Estatus de la Orden de Recoleccion si es completo es SL de lo contrario es SI
        string sResponce = "";
        Parametros = $"StatusPedido = '{status}'";
        Condicion = $"id = {Global.cidsql_} and FolioOrden = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and FolioCotizacion = '{Global.folio_cotizacion_}'";
        response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "OrdenRecoleccion", Condicion, "UPDATE");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                sResponce = "OK";
            }
        }

        // Se actualiza la ubicacion de los articulos cargados
        foreach (clsArticuloSalida item in Global.regArticulosSalida)
        {
            string strSET = $"Seccion = '{item.Seccion}', Pasillo= '{item.Pasillo}', Estanteria='{item.Estanteria}', Nivel='{item.Nivel}', Tarima='{item.Tarima}', Contenedor='{item.Contenedor}'";
            string strWHERE = $"cve_articulo = '{item.codigo_articulo}' and consecutivo_mov = {item.consecutivo} and FolioOrdenRecoleccion = {Global.cidsql_} and FolioOrdenCompra = '{Global.folio_orden_}' and FolioRequisicion = '{Global.folio_requisicion_}' and FolioCotizacion = '{Global.folio_cotizacion_}'";
            response = ConfigAPI.GetAPI("GET", "api/Operacion", strSET, "ws_fn_EjecutarQuerySQL", "Detalle_OrdenRecoleccion", strWHERE, "UPDATE");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
        }

        // Se agrega registro, para el Historial de Seguimiento
        string selectText = "top 1 Num_Articulos_Requisicion, cantidad_articulos, codigo_proveedor ";
        string fromText = "Requisiciones_En_Cotizacion ";
        string whereText = $"folio_requisicion = '{Global.folio_requisicion_}' and folio_cotizacion = '{Global.folio_cotizacion_}' and folio_orden_compra = '{Global.folio_orden_}' ORDER By id desc";
        DataTable Articulos;
        response = ConfigAPI.GetAPI("GET", "api/Operacion", selectText, "wsp_execute_qwerty", fromText, whereText, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            Articulos = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
        }
        foreach (DataRow r in Articulos.Rows)
        {
            string strValues = $"'{Global.folio_requisicion_}','SL','{r[0].ToString().Trim()}','{Global.folio_cotizacion_}','SL','{r[1].ToString().Trim()}','0000','{Global.folio_orden_}','SL','{DateTime.Now.ToShortDateString()}'," +
                               $"'{DateTime.Now.ToString("HH:mm:ss")}','{Global.nombre_usuario}','false','{Global.cidsql_}', 'SL',NULL,NULL,NULL";
            string strCampos = $"folio_requisicion,status_requisicion,Num_Articulos_Requisicion,folio_cotizacion,status_cotizacion,cantidad_articulos,codigo_proveedor,folio_orden_compra,status_orden_compra,Fecha_Movto,Hora_Movto,Usuario,control_bascula,folio_OrdenRecoleccion,status_OrdenRecoleccion,folio_SalidaDocumento,TipoDocumento,status_SalidaDocumento";
            response = ConfigAPI.GetAPI("GET", "api/Operacion", strValues, "ws_fn_EjecutarQuerySQL", "Requisiciones_En_Cotizacion", Condicion, "INSERT INTO", strCampos);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
        }

        //Eliminar los Articulos quitados de la Orden xe Recoleccion
        if (ArticulosEliminados.Count > 0)
        {
            foreach (clsArticuloSalida item in ArticulosEliminados)
            {
                Parametros = "nada";
                Condicion = $"cve_articulo='{item.codigo_articulo}' and FolioOrdenRecoleccion='{item.ID}'";
                response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "Detalle_OrdenRecoleccion", Condicion, "DELETE");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        sResponce = "OK";
                    }
                }
            }
        }

        await DisplayAlertAsync("Guardado Correcto", "Orden de Recoleccion" + Global.cidsql_, "OK");

        selectText = "sol_general, cve_pro, proveedor ";
        fromText = "or_compra ";
        whereText = $"folio = '{Global.folio_orden_}' and folio_cotizacion = '{Global.folio_cotizacion_}' and cancelada = 0";
        DataTable Correos = new DataTable();
        response = ConfigAPI.GetAPI("GET", "api/Operacion", selectText, "wsp_execute_qwerty", fromText, whereText, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            Correos = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
        }

        foreach (DataRow r in Correos.Rows)
        {
            Mail email = new Mail();
            string Prov = $"{r[1].ToString().Trim()} - {r[2].ToString().Trim()}";

            email.Armar_Correos_OrdenCompra(r[0].ToString().Trim(), "", " Lista para su Recoleccion", "ORDEN RECOLECCION", Prov, Global.folio_requisicion_, Global.folio_orden_, Global.cidsql_, Global.folio_cotizacion_);
        }

        LimpiarCampos();

        await Navigation.PopAsync();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (status == PermissionStatus.Granted)
        {
            if (BarcodeScanning.IsSupported)
            {
                item = e.CurrentSelection.FirstOrDefault() as clsArticuloSalida;
                if (item == null || item.noubicaciones == 1)
                {
                    await DisplayAlertAsync("Informacion", "El articulo ya cuenta con ubicacion seleccionada", "OK");
                    return;
                }
                if (item == null || item.noubicaciones == 0)
                {
                    await DisplayAlertAsync("Informacion", "El articulo no cuenta con Ubicacion Asignada", "OK");
                    return;
                }
                var barcodePage = new BarcodePage();
                barcodePage.ParentPageName = "Lectura_Codigo";
                barcodePage.TextResultGenerated += OnTextResultGenerated;
                await Navigation.PushAsync(barcodePage);
            }
            else
            {
                await DisplayAlertAsync("Permiso Denegado", "No se puede acceder a la cámara. Por favor, otorgue el permiso para usar esta función.", "OK");
                return;
            }
        }
    }

    private async void OnTextResultGenerated(string result)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (string.IsNullOrEmpty(result))
            {
                await DisplayAlertAsync("Informacion", "No se ha escaneado un código válido", "OK");
                return;
            }
            string Codigo = item.codigo_articulo;
            string[] Ubicacion;
            Ubicacion = result.ToString().Split('U');

            //// Aquí puedes agregar la lógica para manejar la ubicación obtenida del código QR
            //DisplayAlertAsync("Ubicación Escaneada", $"Sección: {Ubicacion[0]}, Pasillo: {Ubicacion[1]}, Estantería: {Ubicacion[2]}, Nivel: {Ubicacion[3]}, Tarima: {Ubicacion[4]}, Contenedor: {Ubicacion[5]}", "OK");

            string Parametros = "Sucursal,Seccion, (select (tmp.Clave + ' - ' + tmp.Descripcion) from CatalogoSecciones as tmp where tmp.Clave=CatalogoArticuloUbicacion.Seccion)descseccion, " +
            "Pasillo,Estanteria, (select top(1) (tmp.Clave + ' - ' + tmp.Descripcion) from CatalogoEstanterias as tmp where tmp.Clave=CatalogoArticuloUbicacion.Estanteria)descestanteria, " +
            "Nivel,Tarima,Contenedor,CodigoArticulo,Existencia,UnidadControl";
            string Condicion = $"CodigoArticulo='{Codigo}' and Estanteria='{Ubicacion[1]}' and Nivel='{Ubicacion[2]}' and Tarima='{Ubicacion[3]}' and Contenedor='{Ubicacion[4]}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "CatalogoArticuloUbicacion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    switch (nUbicaciones)
                    {
                        case 0:
                            break;

                        case 1:
                            break;

                        default:
                            item.Seccion = r[2].ToString().Trim();
                            item.Pasillo = double.Parse(r[3].ToString().Trim());
                            item.Estanteria = r[5].ToString().Trim();
                            item.Nivel = double.Parse(r[6].ToString().Trim());
                            item.Tarima = double.Parse(r[7].ToString().Trim());
                            item.Contenedor = double.Parse(r[8].ToString().Trim());
                            ExistenciaUbicacion = r[10].ToString().Trim();
                            bUbicacionCapturada = true;
                            break;
                    }
                }
            }

            if (double.Parse(ExistenciaUbicacion) < 1)
            {
                await DisplayAlertAsync("Informacion", "El articulo no cuenta con existencia en la ubicacion seleccionada", "OK");
            }
        });
    }
}