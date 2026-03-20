using Controls.UserDialogs.Maui;
using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class Almacen_Salidas : ContentPage
{
    private Point[] points;
    private bool capturando = true;
    public List<string> lAreas { get; set; }
    public ObservableCollection<clsArticuloSalida> ArticulosEliminados { get; set; }
    public List<string> lCentroCostoN1 { get; set; }
    public List<string> lCentroCostoN2 { get; set; }
    public List<string> lCentroCostoN3 { get; set; }
    public List<string> lCentroCostoN4 { get; set; }
    public bool OrdenRecoleccion = false;
    public string Origen = string.Empty;

    public Almacen_Salidas()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "ATRAS");
        ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();
        LimpiarCampos();
        btnGenerarSalida.IsEnabled = true;
    }

    public Almacen_Salidas(bool Orden_Lista)
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "ATRAS");
        OrdenRecoleccion = Orden_Lista;

        Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
        ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();
        Global.strSucursal = "";
        Global.strArea = "";
        Global.strCCnivel1 = "";
        Global.strCCnivel2 = "";
        Global.strCCnivel3 = "";
        Global.strCCnivel4 = "";
        cbSucursal.IsEnabled = false;
        cbArea.IsEnabled = false;
        cbNivel1.IsEnabled = false;
        cbNivel2.IsEnabled = false;
        cbNivel3.IsEnabled = false;
        cbNivel4.IsEnabled = false;
        btnRefacciones.IsEnabled = false;
        btnGenerarSalida.IsEnabled = true;
        dtFechaSalida.Date = DateTime.Now;
        buscar_datos();
    }

    public Almacen_Salidas(bool Orden_Lista, Boolean historico)
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "ATRAS");
        this.Title = $"Orden de Compra ({Global.folio_orden_}) / Recoleccion ({Global.cidsql_})";
        Origen = "H";
        OrdenRecoleccion = Orden_Lista;
        Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
        Global.strSucursal = "";
        Global.strArea = "";
        Global.strCCnivel1 = "";
        Global.strCCnivel2 = "";
        Global.strCCnivel3 = "";
        Global.strCCnivel4 = "";
        cbSucursal.IsEnabled = false;
        cbArea.IsEnabled = false;
        cbNivel1.IsEnabled = false;
        cbNivel2.IsEnabled = false;
        cbNivel3.IsEnabled = false;
        cbNivel4.IsEnabled = false;
        cbResonsable.IsEnabled = false;
        cbAutorizado.IsEnabled = false;
        btnRefacciones.IsEnabled = false;
        btnGenerarSalida.IsEnabled = false;
        signatureSample.IsEnabled = false;
        dtFechaSalida.IsEnabled = false;
        buscar_datos(historico);
        BindingContext = new OrdenViewModel();
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

    protected override void OnAppearing()
    {
        //ItemsListView.BeginRefresh();
        base.OnAppearing();
        if (Global.validar == true)
        {
            Global.validar = false;
        }
        else
        {
            Global.validar = false;
            //ItemsListView.ItemsSource = Global.regArticulosSalida;
        }
        //ItemsListView.EndRefresh();
    }

    private void LlenarResponsables()
    {
        cbResonsable.ItemsSource = Funciones.LlenarResponsables();
    }

    private void LimpiarCampos()
    {
        Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
        //ItemsListView.ItemsSource = Global.regArticulosSalida;
        ArticulosEliminados.Clear();
        cbArea.SelectedIndex = 0;
        capturando = false;

        Global.strSucursal = "";
        Global.strArea = "";
        Global.strCCnivel1 = "";
        Global.strCCnivel2 = "";
        Global.strCCnivel3 = "";
        Global.strCCnivel4 = "";

        LlenarArea();
        LlenarCentroCostoN1();
        LlenarCentroCostoN2();
        LlenarCentroCostoN3();
        LlenarCentroCostoN4();
        LlenarResponsables();
        capturando = true;

        Global.Editando = false;
        Global.cidsql_ = 0;
        Global.folio_orden_ = string.Empty;
        Global.folio_requisicion_ = string.Empty;
        Global.folio_cotizacion_ = string.Empty;
        OrdenRecoleccion = false;
        cbSucursal.SelectedIndex = -1;
        cbResonsable.SelectedIndex = -1;
        cbAutorizado.SelectedIndex = -1;
        signatureSample.Clear();
    }

    private async void buscar_datos()
    {
        capturando = false;
        string Parametros = $"{Global.cidsql_},RL";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_Recoleccion");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string resp = reader.ReadToEnd();
            if (resp == "[]")
            {
                await DisplayAlertAsync("Informacion", "La Orden de recoleccion no tiene detalle, Favor de validar la Informacion con el administrador del sistema.", "OK");
                return;
            }

            DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            Global.strSucursal = dt.Rows[0][0].ToString();
            Global.strArea = dt.Rows[0][1].ToString();
            Global.strCCnivel1 = dt.Rows[0][3].ToString();
            Global.strCCnivel2 = dt.Rows[0][5].ToString();
            Global.strCCnivel3 = dt.Rows[0][7].ToString();
            Global.strCCnivel4 = dt.Rows[0][9].ToString();
            LlenarArea();
            LlenarCentroCostoN1();
            LlenarCentroCostoN2();
            LlenarCentroCostoN3();
            LlenarCentroCostoN4();
            LlenarResponsables();
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
                    Empleado = r[31].ToString()
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

    public static void SaveByte(string fileName, byte[] data)
    {
        var filePath = "local:firma.jpg";
        if (!File.Exists(filePath))
            File.Delete(filePath);
        File.WriteAllBytes(filePath, data);
    }

    private async void buscar_datos(bool Historico)
    {
        capturando = false;
        string Parametros = $"{Global.cidsql_},H";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_orden_Recoleccion");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string resp = reader.ReadToEnd();
            if (resp == "[]")
            {
                await DisplayAlertAsync("Informacion", "La Orden de recoleccion no tiene detalle, Favor de validar la Informacion con el administrador del sistema.", "OK");
                return;
            }

            DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            Global.strSucursal = dt.Rows[0][0].ToString();
            Global.strArea = dt.Rows[0][1].ToString();
            Global.strCCnivel1 = dt.Rows[0][3].ToString();
            Global.strCCnivel2 = dt.Rows[0][5].ToString();
            Global.strCCnivel3 = dt.Rows[0][7].ToString();
            Global.strCCnivel4 = dt.Rows[0][9].ToString();
            LlenarArea();
            LlenarCentroCostoN1();
            LlenarCentroCostoN2();
            LlenarCentroCostoN3();
            LlenarCentroCostoN4();
            LlenarResponsables();
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
                    Empleado = r[31].ToString().Trim()
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
            cbResonsable.SelectedItem = dt.Rows[0][32].ToString().Trim() + " - " + dt.Rows[0][33].ToString().Trim();
            cbAutorizado.SelectedItem = dt.Rows[0][34].ToString().Trim() + " - " + dt.Rows[0][35].ToString().Trim();
            if (!string.IsNullOrEmpty(dt.Rows[0][37].ToString()))
                dtFechaSalida.Date = DateTime.Parse(dt.Rows[0][37].ToString());
            spvFirma.IsVisible = false;
            imgFirma.IsVisible = true;
            srcFirma.Source = ImageSource.FromStream(() =>
            {
                var stream = new MemoryStream(Base64StringIntoImage(dt.Rows[0][36].ToString().Trim()));
                return stream;
            });
        }
        capturando = true;
    }

    private static byte[] Base64StringIntoImage(string str64)
    {
        byte[] img = Convert.FromBase64String(str64);
        return img;
    }

    private void cbSucursal_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            lAreas = new List<String>();
            lCentroCostoN1 = new List<string>();
            lCentroCostoN2 = new List<string>();
            lCentroCostoN3 = new List<string>();
            lCentroCostoN4 = new List<string>();

            cbArea.ItemsSource = lAreas;
            cbNivel1.ItemsSource = lCentroCostoN1;
            cbNivel2.ItemsSource = lCentroCostoN2;
            cbNivel3.ItemsSource = lCentroCostoN3;
            cbNivel4.ItemsSource = lCentroCostoN4;
            try
            {
                Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
            }
            catch (Exception ex)
            {
                Global.strArea = "";
                Global.strCCnivel1 = "";
                Global.strCCnivel2 = "";
                Global.strCCnivel3 = "";
                Global.strCCnivel4 = "";
                cbNivel1.ItemsSource = lCentroCostoN1;
                cbNivel2.ItemsSource = lCentroCostoN2;
                cbNivel3.ItemsSource = lCentroCostoN3;
                cbNivel4.ItemsSource = lCentroCostoN4;
                return;
            }

            Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
            LlenarArea();
            LlenarResponsables();
        }
    }

    private void cbArea_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (capturando)
        {
            string[] stmp;
            lCentroCostoN1 = new List<string>();
            lCentroCostoN2 = new List<string>();
            lCentroCostoN3 = new List<string>();
            lCentroCostoN4 = new List<string>();

            cbNivel1.ItemsSource = lCentroCostoN1;
            cbNivel2.ItemsSource = lCentroCostoN2;
            cbNivel3.ItemsSource = lCentroCostoN3;
            cbNivel4.ItemsSource = lCentroCostoN4;

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
                cbNivel1.ItemsSource = lCentroCostoN1;
                cbNivel2.ItemsSource = lCentroCostoN2;
                cbNivel3.ItemsSource = lCentroCostoN3;
                cbNivel4.ItemsSource = lCentroCostoN4;
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

    private async void imEditar_Clicked(Object sender, EventArgs e)
    {
        if (Origen == "H") return;
        Global.ArticuloSalidaEditando = new clsArticuloSalida();
        Global.ArticuloSalidaEditando = (sender as MenuItem).BindingContext as clsArticuloSalida;
        Global.Editando = true;
        await Navigation.PushAsync(new Almacen_Salidas_Articulos(Global.strCCnivel3, Global.strCCnivel4));
    }

    private void imEliminar_Clicked(Object sender, EventArgs e)
    {
        if (Origen == "H") return;
        clsArticuloSalida Item_;
        Item_ = (sender as MenuItem).BindingContext as clsArticuloSalida;
        if (OrdenRecoleccion)
            ArticulosEliminados.Add(Item_);
        Global.regArticulosSalida.Remove(Item_);
    }

    private void cbResonsable_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(cbResonsable.SelectedItem.ToString()) || cbResonsable.SelectedIndex <= 0)
        {
            return;
        }

        cbAutorizado.ItemsSource = null;
        cbAutorizado.ItemsSource = Funciones.LlenarAutorizado();
    }

    private async void btnAgregarItems_Clicked(Object sender, EventArgs e)
    {
        if (Global.strSucursal == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "" || Global.strArea == "") //|| Global.strCCnivel3 == ""
        {
            await DisplayAlertAsync("Informacion", "Llenar primero los datos de centros de costo", "OK");
            return;
        }
        string nivel3 = "", nivel4 = "";
        if (cbNivel3.Items.Count > 0 && cbNivel3.SelectedIndex >= 0)
            if (!string.IsNullOrEmpty(cbNivel3.SelectedItem.ToString())) nivel3 = cbNivel3.SelectedItem.ToString();
        if (cbNivel4.Items.Count > 0 && cbNivel4.SelectedIndex >= 0)
            if (!string.IsNullOrEmpty(cbNivel4.SelectedItem.ToString())) nivel4 = cbNivel4.SelectedItem.ToString();

        await Navigation.PushAsync(new Almacen_Salidas_Articulos(nivel3, nivel4));
    }

    private async void btnGenerarSalida_Clicked(Object sender, EventArgs e)
    {
        if (Global.strArea == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "")  //|| Global.strCCnivel3 == ""
        {
            await DisplayAlertAsync("Informacion", "Informacion de centros de costo incompleta", "OK");
            return;
        }

        if (Global.regArticulosSalida.Count == 0)
        {
            await DisplayAlertAsync("Informacion", "No hay articulos relacionados a la salida", "OK");
            return;
        }

        if (cbResonsable.SelectedIndex == -1 || cbAutorizado.SelectedIndex == -1 || String.IsNullOrEmpty(cbResonsable.SelectedItem.ToString()))
        {
            await DisplayAlertAsync("Informacion", "Responsable y autorizado invalidos", "OK");
            return;
        }

        var answer = await DisplayAlertAsync("Informaciòn", "¿Desea generar la salida?", "Si", "No");
        if (answer == false)
        {
            return;
        }

        UserDialogs.Instance.ShowLoading("Guardando");
        btnGenerarSalida.IsEnabled = false;
        string strResponsable = "";
        string strAutorizado = "";
        try
        {
            string[] stmp;
            stmp = cbResonsable.SelectedItem.ToString().Split('-');
            strResponsable = stmp[0].ToString().Trim();
        }
        catch (Exception ex)
        {
            //return;
        }

        try
        {
            string[] stmp;
            stmp = cbAutorizado.SelectedItem.ToString().Split('-');
            strAutorizado = stmp[0].ToString().Trim();
        }
        catch (Exception ex)
        {
            //return;
        }

        string FechaFormateada;
        try
        {
            FechaFormateada = dtFechaSalida.Date.Value.Day.ToString().PadLeft(2, '0') + '/' +
                dtFechaSalida.Date.Value.Month.ToString().PadLeft(2, '0') + '/' +
                dtFechaSalida.Date.Value.Year.ToString();
        }
        catch (Exception ex)
        {
            FechaFormateada = DateTime.Now.ToShortDateString();
        }

        string sFolioSalida = "";
        string Parametros = "folio_salidas_consumo";
        string Condicion = $"1=1";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "control_folios_sistema", Condicion, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            foreach (DataRow r in dt.Rows)
            {
                sFolioSalida = r[0].ToString().Trim().PadLeft(6, '0');
            }
        }

        string sResponce = "";
        Parametros = $"folio_salidas_consumo={int.Parse(sFolioSalida) + 1}";
        Condicion = "id=1";
        response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "control_folios_sistema", Condicion, "UPDATE", "folio_salidas_consumo");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                sResponce = "OK";
            }
        }

        foreach (clsArticuloSalida item in Global.regArticulosSalida)
        {
            if (OrdenRecoleccion)
            {
                // Se Actualiza el Detalle de la orden de recoleccion (Detalle_OrdenRecoleccion) con el folio de la salida
                Parametros = $"TipoDocumento = 'S', Folio_DocumentoSalida = '{sFolioSalida}'";
                Condicion = $"cve_articulo = '{item.codigo_articulo}' and consecutivo_mov = {item.consecutivo} and FolioOrdenRecoleccion = {Clases.Global.cidsql_} and FolioOrdenCompra = '{Clases.Global.folio_orden_}'";
                Condicion += $"and FolioRequisicion = '{Clases.Global.folio_requisicion_}' and FolioCotizacion = '{Clases.Global.folio_cotizacion_}'";
                response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "Detalle_OrdenRecoleccion", Condicion, "UPDATE");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        sResponce = "OK";
                    }
                }
            }
            Parametros = $"{strResponsable},{strAutorizado},{Global.strSucursal},{Global.strArea},{Global.strCCnivel1.Trim()},{Global.strCCnivel2.Trim()}," +
                         $"{item.ccnivel3.Trim()},{item.ccnivel4.Trim()},{Clases.Global.clave_usuario}," +
                         $"{item.codigo_articulo},{item.cantidad},{item.UnidadControlUbicacion},{item.Seccion},{item.Pasillo},{item.Estanteria},{item.Nivel},{item.Tarima},{item.Contenedor},{sFolioSalida}," +
                         $"{item.ControlArea},{item.Reasignado},{item.AreaAsignado},{item.CantidadAsignado},{item.ObservacionAsignado},{item.identrada},{FechaFormateada}";
            response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fnSet_InsertarSalida_v3");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sResponce = "OK";
                }
            }
        }

        if (OrdenRecoleccion)
        {
            // Se valida si fueron entregados, todos los articulos de la Orden de recoleccion, para cambiar el estatus de la Orden
            DataTable dt = new DataTable();
            Parametros = "count(FolioOrdenRecoleccion)";
            Condicion = $"FolioOrdenRecoleccion = '{Clases.Global.cidsql_}' and FolioOrdenCompra = '{Clases.Global.folio_orden_}' and FolioRequisicion = '{Clases.Global.folio_requisicion_}' and Folio_DocumentoSalida IS NULL";
            response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "Detalle_OrdenRecoleccion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            }

            // Se Actualiza el Detalle de la orden de recoleccion (Detalle_OrdenRecoleccion) con el folio de la salida
            foreach (DataRow r in dt.Rows)
            {
                Parametros = $"StatusPedido = 'SR', FechaConfirmado = '{DateTime.Now.ToShortDateString()}', HoraConfirmado = '{DateTime.Now.ToString("HH:mm:ss")}'";
                Condicion = $"id = {Clases.Global.cidsql_} and FolioOrden = '{Clases.Global.folio_orden_}' and FolioRequisicion = '{Clases.Global.folio_requisicion_}' and FolioCotizacion = '{Clases.Global.folio_cotizacion_}'";
                response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "ws_fn_EjecutarQuerySQL", "OrdenRecoleccion", Condicion, "UPDATE");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        sResponce = "OK";
                    }
                }
            }

            // Se agrega registro en Requisiciones_En_Cotizaciones, para llevar el Historial de la Requisicion
            string selectText = "top 1 Num_Articulos_Requisicion, cantidad_articulos, codigo_proveedor ";
            string whereText = $"folio_requisicion = '{Global.folio_requisicion_}' and folio_cotizacion = '{Global.folio_cotizacion_}' and folio_orden_compra = '{Global.folio_orden_}' ORDER By id desc";
            Parametros = selectText;
            Condicion = whereText;
            response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "Requisiciones_En_Cotizacion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return;
                dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            }
            foreach (DataRow r in dt.Rows)
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
        }

        //TODO : Guardar la firma del responsable de la salida
        Stream stream = await signatureSample.GetImageStream(200, 200);
        Parametros = $"{sFolioSalida}";
        response = ConfigAPI.PostAPI_Firma("api/Firma", Parametros, "ws_fnSet_FirmaSalida", stream);
        if (response.StatusCode == HttpStatusCode.NotFound) return;

        UserDialogs.Instance.HideHud();
        await DisplayAlertAsync("Guardado Correcto", "Salida " + sFolioSalida, "OK");
        Clases.Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
        Global.folio_requisicion_ = string.Empty;
        Global.folio_cotizacion_ = string.Empty;
        Global.folio_orden_ = string.Empty;
        Global.cidsql_ = 0;
        sFolioSalida = "";

        //ItemsListView.BeginRefresh();
        //ItemsListView.ItemsSource = Global.regArticulosSalida;
        //ItemsListView.EndRefresh();

        OrdenRecoleccion = false;
        btnGenerarSalida.IsEnabled = true;
        await Navigation.PopAsync();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Origen == "H")
            if (e.CurrentSelection.FirstOrDefault() != null) miCollectionView.SelectedItem = null;
    }
}