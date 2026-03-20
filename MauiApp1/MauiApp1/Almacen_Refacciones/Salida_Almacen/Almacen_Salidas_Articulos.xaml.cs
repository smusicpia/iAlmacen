using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.Views;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using ZXing;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using static SQLite.SQLite3;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen
{
    public partial class Almacen_Salidas_Articulos : ContentPage
    {
        public List<string> lCentroCostoN3 { get; set; }
        public List<string> lCentroCostoN4 { get; set; }
        public static Boolean bUbicacionCapturada = false;
        public static Boolean bControlArea = false;
        public static Boolean bControlAreaValido = false;
        public static string sAreaAsignado = "";
        public static double dCantidadAsignado = 0;
        public static int identrada = 0;

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

        private string _result2;

        public string Result2
        {
            get => _result2;
            set
            {
                _result2 = value;
                OnPropertyChanged(nameof(Result2));
            }
        }

        public Almacen_Salidas_Articulos(string Nivel3, string Nivel4)
        {
            InitializeComponent();
            if (Global.Editando == true)
            {
                btnScanArticulo.IsEnabled = false;
                btnScanUbicacion.IsEnabled = false;
                btnAgregar.Text = "Actualizar";
                LlenarDatos();
            }
            else
            {
                LlenarNivel3();
                LlenarNivel4();
                cbNivel3.SelectedItem = Nivel3;
                cbNivel4.SelectedItem = Nivel4;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void LlenarDatos()
        {
            lblClave.Text = Global.ArticuloSalidaEditando.codigo_articulo;
            lblDescripcion.Text = Global.ArticuloSalidaEditando.descripcion_general;
            lblFamilia.Text = Global.ArticuloSalidaEditando.desc_familia;
            lblLinea.Text = Global.ArticuloSalidaEditando.desc_linea;
            lblGrupo.Text = Global.ArticuloSalidaEditando.desc_grupo;
            lblMedida.Text = Global.ArticuloSalidaEditando.desc_medida;
            lblMarca.Text = Global.ArticuloSalidaEditando.desc_marca;
            lblParte.Text = Global.ArticuloSalidaEditando.desc_parte;
            nUbicaciones = int.Parse(Global.ArticuloSalidaEditando.noubicaciones.ToString());
            lblSeccion.Text = Global.ArticuloSalidaEditando.Seccion + " - " + Global.ArticuloSalidaEditando.desc_seccion;
            lblPasillo.Text = Global.ArticuloSalidaEditando.Pasillo.ToString();
            lblEstanteria.Text = Global.ArticuloSalidaEditando.Estanteria + " - " + Global.ArticuloSalidaEditando.desc_estanteria;
            lblNivel.Text = Global.ArticuloSalidaEditando.Nivel.ToString();
            lblTarima.Text = Global.ArticuloSalidaEditando.Tarima.ToString();
            lblContenedor.Text = Global.ArticuloSalidaEditando.Contenedor.ToString();
            lblExistenciaUbicacion.Text = Global.ArticuloSalidaEditando.ExistenciaUbicacion.ToString();
            lblUnidad.Text = Global.ArticuloSalidaEditando.UnidadControlUbicacion;
            lblExistenciaglobal.Text = Global.ArticuloSalidaEditando.ExistenciaKardex.ToString();
            txtCantidad.Text = Global.ArticuloSalidaEditando.cantidad.ToString();
        }

        private void LimpiarCampos()
        {
            lblClave.Text = "";
            lblDescripcion.Text = "";
            lblMedida.Text = "";
            lblMarca.Text = "";
            lblParte.Text = "";
            lblFamilia.Text = "";
            lblLinea.Text = "";
            lblGrupo.Text = "";
            nUbicaciones = 0;
            lblSeccion.Text = "";
            lblPasillo.Text = "";
            lblEstanteria.Text = "";
            lblNivel.Text = "";
            lblTarima.Text = "";
            lblContenedor.Text = "";
            lblExistenciaUbicacion.Text = "";
            lblUnidad.Text = "";
            lblExistenciaglobal.Text = "";
            txtCantidad.Text = "0.00";
            frmControlArea.IsVisible = false;
            btnScanControl.IsVisible = false;
            lblAreaAsignada.Text = "";
            lblCantidadAsignada.Text = "0.00";
            swReasignar.IsToggled = false;
            txtObservacionReasignar.Text = "";
            bControlArea = false;
            bUbicacionCapturada = false;
            bControlAreaValido = false;
            sAreaAsignado = "";
            dCantidadAsignado = 0;
            identrada = 0;
        }

        private void LlenarNivel3()
        {
            cbNivel3.ItemsSource = Funciones.LlenarCentroCostoN3();
        }

        private void LlenarNivel4()
        {
            cbNivel4.ItemsSource = Funciones.LlenarCentroCostoN4();
        }

        private async void obtenerDatosArticulo(string codArticulo)
        {
            Result = $"{codArticulo}";
            string Codigo = Result;
            Codigo = Codigo.Substring(1, 5);

            // Para pruebas
            //Result = "0U01U2U3U4";
            //0 - U01 Estanteria - U2 Nivel - U3 Tarima - U4 Contenedor
            //BuscarUbicacion();
            //string[] Valores;
            //string Codigo = "00391";

            string Parametros = $"{Codigo}";
            try
            {
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_DatosArticulo");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r[16].ToString() == "1")
                        {
                            await DisplayAlertAsync("Informacion", "Articulo ya se encuentran en proceso de inventario", "OK");
                            break;
                        }

                        lblClave.Text = r[0].ToString().Trim();
                        lblDescripcion.Text = r[1].ToString().Trim();
                        lblMedida.Text = r[5].ToString().Trim();
                        lblMarca.Text = r[6].ToString().Trim();
                        lblParte.Text = r[7].ToString().Trim();
                        lblFamilia.Text = r[2].ToString().Trim();
                        lblLinea.Text = r[3].ToString().Trim();
                        lblGrupo.Text = r[4].ToString().Trim();

                        nUbicaciones = int.Parse(r[8].ToString().Trim());
                        switch (nUbicaciones)
                        {
                            case 0:
                                lblSeccion.Text = "No Disponible";
                                lblPasillo.Text = "**";
                                lblEstanteria.Text = "**";
                                lblNivel.Text = "**";
                                lblTarima.Text = "**";
                                lblContenedor.Text = "**";
                                lblExistenciaUbicacion.Text = "0.00";
                                bUbicacionCapturada = true;
                                break;

                            case 1:
                                lblSeccion.Text = r[9].ToString().Trim() + " - " + r[19].ToString().Trim();
                                lblPasillo.Text = r[10].ToString().Trim();
                                lblEstanteria.Text = r[11].ToString().Trim() + " - " + r[20].ToString().Trim();
                                lblNivel.Text = r[12].ToString().Trim();
                                lblTarima.Text = r[13].ToString().Trim();
                                lblContenedor.Text = r[14].ToString().Trim();
                                lblExistenciaUbicacion.Text = r[15].ToString().Trim();
                                bUbicacionCapturada = true;
                                break;

                            default:
                                lblSeccion.Text = "Escanear Ubicacion";
                                lblPasillo.Text = "**";
                                lblEstanteria.Text = "**";
                                lblNivel.Text = "**";
                                lblTarima.Text = "**";
                                lblContenedor.Text = "**";
                                lblExistenciaUbicacion.Text = "0.00";
                                bUbicacionCapturada = false;
                                break;
                        }

                        lblUnidad.Text = r[16].ToString().Trim();
                        lblExistenciaglobal.Text = string.IsNullOrEmpty(r[17].ToString().Trim()) ? "0.00" : r[17].ToString().Trim();
                        txtCantidad.Text = "1.00";
                        bControlArea = Boolean.Parse(r[21].ToString().Trim());
                        if (bControlArea)
                        {
                            await DisplayAlertAsync("Control por area", "Escanee el codigo del control por area asignado", "OK");
                            btnScanControl.IsVisible = true;
                        }
                    }
                }

                if (nUbicaciones > 1)
                {
                    await DisplayAlertAsync("Informacion", "El articulo cuenta con mas ubicaciones, por favor escanee el codigo de la ubicacion", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Hubo un error y no fue posible agregar el Articulo. ({ex.Message})", "OK");
            }
        }

        private async void obtenerDatosUbicacion(string codUbicacion)
        {
            if (string.IsNullOrEmpty(codUbicacion))
            {
                Result = "";
            }
            else
            {
                Result = $"{codUbicacion}";
                //string strUbicacion = "0U19U3U0U8";
                string Codigo = lblClave.Text;
                //Result = "0U01U2U3U4";
                // 0 - U01 Estanteria - U2 Nivel - U3 Tarima - U4 Contenedor
                //BuscarUbicacion();

                //string Codigo = Result;
                //string[] Valores;
                //Codigo = Codigo.Substring(2, 5);
                //string Codigo = "00391";
                string[] Ubicacion;
                Ubicacion = Result.ToString().Split('U');
                if (Ubicacion[0] == "0")
                {
                    string Parametros = $"{Codigo},{Ubicacion[1]},{Ubicacion[2]},{Ubicacion[3]},{Ubicacion[4]}";
                    HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_DatosUbicacion");
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
                                    lblSeccion.Text = r[2].ToString().Trim();
                                    lblPasillo.Text = r[3].ToString().Trim();
                                    lblEstanteria.Text = r[5].ToString().Trim();
                                    lblNivel.Text = r[6].ToString().Trim();
                                    lblTarima.Text = r[7].ToString().Trim();
                                    lblContenedor.Text = r[8].ToString().Trim();
                                    lblExistenciaUbicacion.Text = r[10].ToString().Trim();
                                    bUbicacionCapturada = true;
                                    break;
                            }
                        }
                    }
                }
                if (Ubicacion[0] != "0")
                {
                    await DisplayAlertAsync("Informacion", "La Ubicacion no existe, favor de verificar la información.", "OK");
                }
                else if (double.Parse(lblExistenciaUbicacion.Text) < 1)
                {
                    await DisplayAlertAsync("Informacion", "El articulo no cuenta con existencia en la ubicacion seleccionada", "OK");
                }
            }
        }

        private async void obtenerDatosControl(string codControl)
        {
            Boolean ControlEncontrado = false;
            if (string.IsNullOrEmpty(codControl))
            {
                Result2 = "";
            }
            else
            {
                Result2 = $"{codControl}";
                //Result = "0183257";

                string Codigo = Result2;
                //string[] Valores;
                //Codigo = Codigo.Substring(1, 6);
                identrada = int.Parse(Codigo);

                frmControlArea.IsVisible = false;
                bControlAreaValido = false;

                string Parametros = "area, centro_costo, cast(cantidad as numeric(18,2)) cantidad, " +
                                    "(select top(1) tmp.descripcion from catalogo_areas as tmp where tmp.clave_area=detalle_documentos_almacen.area and tmp.psucursal=detalle_documentos_almacen.psucursal) descarea, " +
                                    "codigo_articulo, ControlAreaUsado";
                string Condicion = $"id='{identrada}' and codigo_articulo='{lblClave.Text}' and ControlAreaUsado=0";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "detalle_documentos_almacen", Condicion, "SELECT", "");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    if (resp != "[]")
                    {
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                        foreach (DataRow r in dt.Rows)
                        {
                            sAreaAsignado = r[0].ToString().Trim();
                            dCantidadAsignado = double.Parse(r[2].ToString().Trim());

                            swReasignar.IsToggled = false;
                            lblAreaAsignada.Text = r[3].ToString().Trim();
                            lblCantidadAsignada.Text = r[2].ToString().Trim();
                            txtObservacionReasignar.Text = "";
                            bControlAreaValido = true;
                            frmControlArea.IsVisible = true;

                            ControlEncontrado = true;
                        }
                    }
                }

                if (!ControlEncontrado)
                {
                    await DisplayAlertAsync("Informacion", "Etiqueta invalida", "OK");
                }
                else
                {
                    if (sAreaAsignado != Global.strArea)
                    {
                        await DisplayAlertAsync("Informacion", "Los articulos asignados pertenecen a otra area, debera solicitar la autorizacion para entregarlos", "OK");
                    }
                }
            }
        }

        private async void OnTextResultArtGenerated(string result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(result))
                {
                    DisplayAlertAsync("Informacion", "No se ha escaneado un código válido", "OK");
                    return;
                }

                obtenerDatosArticulo(result);
            });
        }

        private async void btnScanArticulo_Clicked(Object sender, EventArgs e)
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
                    var barcodePage = new BarcodePage();
                    barcodePage.ParentPageName = "Lectura_Codigo";
                    barcodePage.TextResultGenerated += OnTextResultArtGenerated;
                    await Navigation.PushAsync(barcodePage);
                }
                else
                {
                    await DisplayAlertAsync("Error", "La funcionalidad de escaneo no es compatible con este dispositivo.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlertAsync("Informacion", $"El permiso fue denegado.", "OK");
            }
        }

        private async void OnTextResultUbiGenerated(string result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(result))
                {
                    DisplayAlertAsync("Informacion", "No se ha escaneado un código válido", "OK");
                    return;
                }

                obtenerDatosUbicacion(result);
            });
        }

        private async void btnScanUbicacion_Clicked(Object sender, EventArgs e)
        {
            //Result = "0U01U2U3U4";
            // 0 - U01 Estanteria - U2 Nivel - U3 Tarima - U4 Contenedor

            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            if (status == PermissionStatus.Granted)
            {
                if (BarcodeScanning.IsSupported)
                {
                    if (lblClave.Text == "")
                    {
                        await DisplayAlertAsync("Informacion", "Capture primero el codigo del articulo", "OK");
                        return;
                    }

                    if (nUbicaciones <= 1)
                    {
                        await DisplayAlertAsync("Informacion", "El articulo tiene una ubicacion predeterminada", "OK");
                        return;
                    }

                    var barcodePage = new BarcodePage();
                    barcodePage.ParentPageName = "Lectura_Codigo";
                    barcodePage.TextResultGenerated += OnTextResultUbiGenerated;
                    await Navigation.PushAsync(barcodePage);
                }
                else
                {
                    await DisplayAlertAsync("Error", "La funcionalidad de escaneo no es compatible con este dispositivo.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlertAsync("Informacion", $"El permiso fue denegado.", "OK");
            }
        }

        private async void OnTextResultCtrlGenerated(string result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(result))
                {
                    DisplayAlertAsync("Informacion", "No se ha escaneado un código válido", "OK");
                    return;
                }

                obtenerDatosControl(result);
            });
        }

        private async void btnScanControl_Clicked(Object sender, EventArgs e)
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
                    var barcodePage = new BarcodePage();
                    barcodePage.ParentPageName = "Lectura_Codigo";
                    barcodePage.TextResultGenerated += OnTextResultCtrlGenerated;
                    await Navigation.PushAsync(barcodePage);
                }
                else
                {
                    await DisplayAlertAsync("Error", "La funcionalidad de escaneo no es compatible con este dispositivo.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlertAsync("Informacion", $"El permiso fue denegado.", "OK");
            }
        }

        private void cbNivel3_SelectedIndexChanged(Object sender, EventArgs e)
        {
            string[] stmp;
            try
            {
                stmp = cbNivel3.SelectedItem.ToString().Split('-');
                Global.strCCnivel3 = stmp[0].ToString().Trim();
            }
            catch (Exception ex)
            {
                Global.strCCnivel3 = "";
                return;
            }
            LlenarNivel4();
        }

        private async void btnAgregar_Clicked(Object sender, EventArgs e)
        {
            if (Global.Editando)
            {
                Global.Editando = false;
                Global.validar = true;
                Global.ArticuloSalidaEditando.cantidad = double.Parse(txtCantidad.Text);
                foreach (clsArticuloSalida item_ in Global.regArticulosSalida)
                {
                    if (item_.ID == Global.ArticuloSalidaEditando.ID)
                    {
                        item_.cantidad = double.Parse(txtCantidad.Text);
                    }
                }
                await Navigation.PopAsync();
            }
            else
            {
                if (!bUbicacionCapturada)
                {
                    await DisplayAlertAsync("Informacion", "Ubicacion invalida", "OK");
                    return;
                }

                if (bControlArea)
                {
                    if (!bControlAreaValido)
                    {
                        await DisplayAlertAsync("Informacion", "Es necesario escanear el codigo del control area", "OK");
                        return;
                    }

                    if (sAreaAsignado != Global.strArea && !swReasignar.IsToggled)
                    {
                        await DisplayAlertAsync("Control Area", "El articulo esta asignado a otra area, es necesario autorizar esta salida", "OK");
                        return;
                    }

                    if (dCantidadAsignado < double.Parse(txtCantidad.Text) && !swReasignar.IsToggled)
                    {
                        await DisplayAlertAsync("Control Area", "la cantidad es superior a lo asignado, es necesario autorizar esta salida", "OK");
                        return;
                    }

                    if (swReasignar.IsToggled && txtObservacionReasignar.Text == "")
                    {
                        await DisplayAlertAsync("Control Area", "Debe ingresar una observacion sobre la autorizacion", "OK");
                        return;
                    }
                }

                string[] sSeccion;
                sSeccion = lblSeccion.Text.ToString().Split('-');

                string[] sEstanteria;
                sEstanteria = lblEstanteria.Text.ToString().Split('-');

                double dCantidad = 0;
                try
                {
                    dCantidad = double.Parse(txtCantidad.Text.Trim());
                }
                catch (Exception ex)
                {
                    dCantidad = 1;
                }

                if (dCantidad == 0)
                {
                    await DisplayAlertAsync("Informacion", "La cantidad de salida es invalida (no puede ser cero)", "OK");
                    return;
                }

                switch (nUbicaciones)
                {
                    case 0:
                        if (dCantidad > double.Parse(lblExistenciaglobal.Text))
                        {
                            await DisplayAlertAsync("Informacion", "Articulo no cuenta con la existencia suficiente", "OK");
                            return;
                        }
                        break;

                    case 1:
                        if (dCantidad > double.Parse(lblExistenciaUbicacion.Text))
                        {
                            await DisplayAlertAsync("Informacion", "Articulo no cuenta con la existencia suficiente en la ubicacion", "OK");
                            return;
                        }
                        break;

                    default:
                        if (dCantidad > double.Parse(lblExistenciaUbicacion.Text))
                        {
                            await DisplayAlertAsync("Informacion", "Articulo no cuenta con la existencia suficiente en la ubicacion", "OK");
                            return;
                        }
                        break;
                }

                string[] stmp;
                try
                {
                    stmp = cbNivel3.SelectedItem.ToString().Split('-');
                    Global.strCCnivel3 = stmp[0].ToString().Trim();
                }
                catch (Exception ex)
                {
                    Global.strCCnivel3 = "";
                }

                try
                {
                    stmp = cbNivel4.SelectedItem.ToString().Split('-');
                    Global.strCCnivel4 = stmp[0].ToString().Trim();
                }
                catch (Exception ex)
                {
                    Global.strCCnivel4 = "";
                }

                string tmpSeccion = "0";
                string tmpDescSeccion = "";
                string tmpPasillo = "0";
                string tmpEstanteria = "0";
                string tmpDescEstanteria = "";
                string tmpNivel = "0";
                string tmpTarima = "0";
                string tmpContenedor = "0";
                if (nUbicaciones == 0)
                {
                    tmpSeccion = "0";
                    tmpDescSeccion = "";
                    tmpPasillo = "0";
                    tmpDescEstanteria = "0";
                    tmpDescSeccion = "";
                    tmpNivel = "0";
                    tmpTarima = "0";
                    tmpContenedor = "0";
                }
                else
                {
                    tmpSeccion = sSeccion[0].Trim();
                    tmpDescSeccion = sSeccion[1].Trim();
                    tmpPasillo = lblPasillo.Text.Trim();
                    tmpEstanteria = sEstanteria[0].Trim();
                    tmpDescEstanteria = sEstanteria[1].Trim();
                    tmpNivel = lblNivel.Text.Trim();
                    tmpTarima = lblTarima.Text.Trim();
                    tmpContenedor = lblContenedor.Text.Trim();
                }

                Global.regArticulosSalida.Add(new clsArticuloSalida
                {
                    codigo_articulo = lblClave.Text.Trim(),
                    descripcion_general = lblDescripcion.Text.Trim(),
                    desc_familia = lblFamilia.Text.Trim(),
                    desc_linea = lblLinea.Text.Trim(),
                    desc_grupo = lblGrupo.Text.Trim(),
                    desc_medida = lblMedida.Text.Trim(),
                    desc_marca = lblMarca.Text.Trim(),
                    desc_parte = lblParte.Text.Trim(),
                    noubicaciones = double.Parse(nUbicaciones.ToString()),
                    Seccion = tmpSeccion,
                    Pasillo = double.Parse(tmpPasillo),
                    Estanteria = tmpEstanteria,
                    Nivel = double.Parse(tmpNivel),
                    Tarima = double.Parse(tmpTarima),
                    Contenedor = double.Parse(tmpContenedor),
                    ExistenciaUbicacion = double.Parse(lblExistenciaUbicacion.Text.Trim()),
                    UnidadControlUbicacion = lblUnidad.Text.Trim(),
                    ExistenciaKardex = double.Parse(lblExistenciaglobal.Text.Trim()),
                    cantidad = dCantidad,
                    desc_seccion = tmpDescSeccion,
                    desc_estanteria = tmpDescEstanteria,
                    ccsucursal = Global.strSucursal,
                    ccarea = Global.strArea,
                    ccnivel1 = Global.strCCnivel1,
                    ccnivel2 = Global.strCCnivel2,
                    ccnivel3 = Global.strCCnivel3,
                    ccnivel4 = Global.strCCnivel4,
                    ControlArea = bControlArea,
                    Reasignado = swReasignar.IsToggled,
                    AreaAsignado = sAreaAsignado,
                    CantidadAsignado = dCantidadAsignado,
                    ObservacionAsignado = txtObservacionReasignar.Text,
                    identrada = identrada
                });

                var answer = await DisplayAlertAsync("Informaciòn", "¿Desea capturar un nuevo articulo?", "Si", "No");
                if (answer == false)
                {
                    LimpiarCampos();
                    Global.validar = true;
                    await Navigation.PopAsync();
                    return;
                }
                else
                {
                    LimpiarCampos();
                }
            }
        }

        private async void btnmanual_Clicked(Object sender, EventArgs e)
        {
            string Codigo = "32999";
            string result = await DisplayPromptAsync("Informacion", "Ingrese la clave del Articulo", "OK", "Cancelar", "Ejemplo: 32999", 5, Keyboard.Numeric);
            if (!string.IsNullOrEmpty(result))
            {
                Codigo = result;
                try
                {
                    HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Codigo, "wsp_DatosArticulo");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound) return;
                        string resp = reader.ReadToEnd();
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                        foreach (DataRow r in dt.Rows)
                        {
                            if (r[16].ToString() == "1")
                            {
                                await DisplayAlertAsync("Informacion", "Articulo ya se encuentran en proceso de inventario", "OK");
                                break;
                            }

                            lblClave.Text = r[0].ToString().Trim();
                            lblDescripcion.Text = r[1].ToString().Trim();
                            lblMedida.Text = r[5].ToString().Trim();
                            lblMarca.Text = r[6].ToString().Trim();
                            lblParte.Text = r[7].ToString().Trim();
                            lblFamilia.Text = r[2].ToString().Trim();
                            lblLinea.Text = r[3].ToString().Trim();
                            lblGrupo.Text = r[4].ToString().Trim();
                            nUbicaciones = int.Parse(r[8].ToString().Trim());
                            switch (nUbicaciones)
                            {
                                case 0:
                                    lblSeccion.Text = "No Disponible";
                                    lblPasillo.Text = "**";
                                    lblEstanteria.Text = "**";
                                    lblNivel.Text = "**";
                                    lblTarima.Text = "**";
                                    lblContenedor.Text = "**";
                                    lblExistenciaUbicacion.Text = "0.00";
                                    bUbicacionCapturada = true;
                                    break;

                                case 1:
                                    lblSeccion.Text = r[9].ToString().Trim() + " - " + r[19].ToString().Trim();
                                    lblPasillo.Text = r[10].ToString().Trim();
                                    lblEstanteria.Text = r[11].ToString().Trim() + " - " + r[20].ToString().Trim();
                                    lblNivel.Text = r[12].ToString().Trim();
                                    lblTarima.Text = r[13].ToString().Trim();
                                    lblContenedor.Text = r[14].ToString().Trim();
                                    lblExistenciaUbicacion.Text = r[15].ToString().Trim();
                                    bUbicacionCapturada = true;
                                    break;

                                default:
                                    lblSeccion.Text = "Escanear Ubicacion";
                                    lblPasillo.Text = "**";
                                    lblEstanteria.Text = "**";
                                    lblNivel.Text = "**";
                                    lblTarima.Text = "**";
                                    lblContenedor.Text = "**";
                                    lblExistenciaUbicacion.Text = "0.00";
                                    bUbicacionCapturada = false;
                                    break;
                            }

                            lblUnidad.Text = r[16].ToString().Trim();
                            lblExistenciaglobal.Text = string.IsNullOrEmpty(r[17].ToString().Trim()) ? "0.00" : r[17].ToString().Trim();
                            txtCantidad.Text = "1.00";
                            bControlArea = Boolean.Parse(r[21].ToString().Trim());
                            if (bControlArea)
                            {
                                await DisplayAlertAsync("Control por area", "Escanee el codigo del control por area asignado", "OK");
                                btnScanControl.IsVisible = true;
                            }
                        }
                    }

                    if (nUbicaciones > 1)
                    {
                        await DisplayAlertAsync("Informacion", "El articulo cuenta con mas ubicaciones, por favor escanee el codigo de la ubicacion", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlertAsync("Error", $"Hubo un error y no fue posible agregar el Articulo. ({ex.Message})", "OK");
                }
            }
        }
    }
}