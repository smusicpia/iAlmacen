using iAlmacen.WebApi;
using Newtonsoft.Json;
using iAlmacen.Clases;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Security.Cryptography;

namespace iAlmacen.Clases
{
    public class Funciones
    {
        private static List<string> lAreas { get; set; }
        private static List<string> lCentroCostoN1 { get; set; }
        private static List<string> lCentroCostoN2 { get; set; }
        private static List<string> lCentroCostoN3 { get; set; }
        private static List<string> lCentroCostoN4 { get; set; }
        private static List<string> Responsable { get; set; }
        private static List<string> Autorizado { get; set; }
        private static List<string> Almacenistas { get; set; }
        private static List<string> CatFamilias { get; set; }
        private static List<string> CatLineas { get; set; }
        private static List<string> CatGrupo { get; set; }
        private static List<string> lSeries { get; set; }
        private static ObservableCollection<Clases.clsSeccion> Secciones { get; set; }
        private static ObservableCollection<clsEstanteria> Estanterias { get; set; }

        public static HttpStatusCode Login(string user, string pass)
        {
            using (var myAes = Aes.Create())
            {
                Global.Key = myAes.Key;
                Global.IV = myAes.IV;
            }

            string Parametros = $"{Convert.ToBase64String(SecurityManager.Encrypt(user, Global.Key, Global.IV))}," +
                                $"{Convert.ToBase64String(SecurityManager.Encrypt(pass, Global.Key, Global.IV))}";

            //TODO: Authorization y Authentication tokenAPI
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/login/Authenticate", Parametros, "LoginWebserver");
            //HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "LoginWebserver");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return response.StatusCode;
                    string resp = reader.ReadToEnd();
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        Global.clave_usuario = user;
                        Global.nombre_usuario = r["usuario"].ToString();
                        Global.privilegio_usuario = dt.Rows[0]["privilegio"].ToString();
                        Global.pass = Convert.ToBase64String(SecurityManager.Encrypt(pass, Global.Key, Global.IV));

                        Global.Mitem_almacen = Boolean.Parse(r["almacen"].ToString());
                        Global.Mitem_proterm = Boolean.Parse(r["producto_terminado"].ToString());
                        Global.Mitem_bascula = Boolean.Parse(r["bascula"].ToString());

                        Global.mensaje_sistema = r["mensaje"].ToString();
                        Global.cierra_modulo = Boolean.Parse(r["cierra_modulo"].ToString());
                        Global.modulo_afectado = r["modulo_afectado"].ToString();
                        Global.cierra_sistema = Boolean.Parse(r["cierra_sistema"].ToString());

                        Global.tasa_super = double.Parse(r["porcentaje_supervisor"].ToString());
                        Global.tasa_admin = double.Parse(r["porcentaje_administrador"].ToString());
                        Global.tasa_oper = double.Parse(r["porcentaje_operador"].ToString());
                        //TODO: Authorization y Authentication tokenAPI
                        Global.tokenAPI = r["TokenApi"].ToString();
                        //TODO: Refresh tokenAPI
                        Global.refreshTokenAPI = r["RefreshTokenApi"].ToString();
                        Preferences.Default.Set("tokenAPI", Global.tokenAPI);
                        Preferences.Default.Set("refreshTokenAPI", Global.refreshTokenAPI);
                        if (Global.tokenAPI != "")
                            break;
                    }
                }
            }

            return response.StatusCode;
        }

        public static List<string> LlenarArea()
        {
            lAreas = new List<String>();
            lAreas.Add("");
            string Parametros = "rtrim(clave_area) clave_area, rtrim(descripcion) descripcion";
            string Condicion = $"psucursal='{Global.strSucursal}' order by descripcion";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "catalogo_areas", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lAreas;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lAreas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lAreas;
        }

        public static List<string> LlenarAreaH()
        {
            lAreas = new List<String>();
            lAreas.Add("");
            string Parametros = "distinct rtrim(salida.codigo_area)codigo_area,rtrim(area.descripcion) descripcion";
            string From = "salidas_resguardo_herramientas as salida inner join catalogo_areas as area on area.clave_area=salida.codigo_area";
            string Condicion = "not (select count(tmp.status_herramienta) from detalle_salidas_resguardo_herramientas as tmp where tmp.folio_resguardo=salida.folio_resguardo and tmp.status_herramienta='AR') = 0 " +
                               "order by codigo_area";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", From, Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lAreas;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lAreas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lAreas;
        }

        public static List<string> LlenarCentroCostoN1()
        {
            lCentroCostoN1 = new List<string>();
            lCentroCostoN1.Add("");
            string Parametros = "codigo_nivel1, descripcion";
            string Condicion = $"psucursal='{Global.strSucursal}' and clave_area='{Global.strArea}' and status_centro_n1 = 1 order by descripcion";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "centro_costos_lvl1", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lCentroCostoN1;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lCentroCostoN1.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lCentroCostoN1;
        }

        public static List<string> LlenarCentroCostoN2()
        {
            lCentroCostoN2 = new List<string>();
            lCentroCostoN2.Add("");
            string Parametros = "codigo_nivel2,descripcion";
            string Condicion = $"psucursal='{Global.strSucursal}' and clave_area='{Global.strArea}' and codigo_nivel1='{Global.strCCnivel1}' and status_centro_n2 = 1 order by descripcion";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "centro_costos_lvl2", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lCentroCostoN2;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lCentroCostoN2.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lCentroCostoN2;
        }

        public static List<string> LlenarCentroCostoN3()
        {
            lCentroCostoN3 = new List<string>();
            lCentroCostoN3.Add("");
            string Parametros = "codigo_nivel3, descripcion";
            string Condicion = $"psucursal='{Global.strSucursal}' and clave_area='{Global.strArea}' and codigo_nivel1='{Global.strCCnivel1}' and codigo_nivel2='{Global.strCCnivel2}' and status_centro_n3 = 1 order by descripcion";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "centro_costos_lvl3", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lCentroCostoN3;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return lCentroCostoN3;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lCentroCostoN3.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lCentroCostoN3;
        }

        public static List<string> LlenarCentroCostoN4()
        {
            lCentroCostoN4 = new List<string>();
            lCentroCostoN4.Add("");
            string Parametros = "codigo_nivel4, descripcion";
            string Condicion = $"psucursal='{Global.strSucursal}' and clave_area='{Global.strArea}' and codigo_nivel1='{Global.strCCnivel1}' and codigo_nivel2='{Global.strCCnivel2}' and codigo_nivel3='{Global.strCCnivel3}' and status_centro_n4 = 1 order by descripcion";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "centro_costos_lvl4", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lCentroCostoN4;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return lCentroCostoN4;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lCentroCostoN4.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return lCentroCostoN4;
        }

        public static List<string> LlenarResponsables()
        {
            Responsable = new List<string>();
            Responsable.Add("");
            string Parametros = "rtrim(clave_solicitante)clave_solicitante, rtrim(nombre_solicitante)nombre_solicitante";
            string Condicion = $"psucursal='{Global.strSucursal}' order by nombre_solicitante";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "solicitantes", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return Responsable;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return Responsable;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Responsable.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return Responsable;
        }

        public static List<string> LlenarAutorizado()
        {
            Autorizado = new List<string>();
            Autorizado.Add("");
            string Parametros = "rtrim(codigo_autorizado)codigo_autorizado, rtrim(nombre_autorizado)nombre_autorizado";
            string Condicion = $"psucursal='{Global.strSucursal}' and clave_area='{Global.strArea}' order by nombre_autorizado";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "catalogo_autorizados", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return Autorizado;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return Autorizado;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Autorizado.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return Autorizado;
        }

        public static List<string> LlenarAlmacenistas()
        {
            Almacenistas = new List<string>();
            Almacenistas.Add("");
            string Parametros = "ClaveIpad,usuario,nombre";
            string Condicion = "ClaveIpad is not null";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "usuarios", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return Almacenistas;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return Almacenistas;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Almacenistas.Add(r[0].ToString().Trim() + " - " + r[2].ToString().Trim());
                }
            }
            return Almacenistas;
        }

        public static List<string> LlenarCategoriaFamilias(bool EsHerramienta = false)
        {
            string Condicion = string.Empty;
            string From = string.Empty;
            string Parametros = string.Empty;
            CatFamilias = new List<string>();
            CatFamilias.Add("");

            if (EsHerramienta)
            {
                Parametros = "distinct cf.codigo_familia, cf.descripcion";
                From = "catalogo_familia cf inner join catalogo_articulo as ca on ca.codigo_familia=cf.codigo_familia";
                Condicion = "ca.uso_herramienta = 1";
            }
            else
            {
                Parametros = "cf.codigo_familia, cf.descripcion";
                From = "catalogo_familia cf";
                Condicion = "Almacen = 1 and status_familia = 1";
            }
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", From, Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return CatFamilias;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return CatFamilias;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    CatFamilias.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return CatFamilias;
        }

        public static List<string> LlenarCategoriaLineas(string familia)
        {
            CatLineas = new List<string>();
            CatLineas.Add("");
            string Parametros = "codigo_linea, descripcion";
            string Condicion = $"codigo_familia = '{familia}' and status_linea = 1";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "catalogo_linea", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return CatLineas;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return CatLineas;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    CatLineas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return CatLineas;
        }

        public static List<string> LlenarCategoriaGrupo(string familia, string linea)
        {
            CatGrupo = new List<string>();
            CatGrupo.Add("");
            string Parametros = "codigo_grupo, descripcion";
            string Condicion = $"codigo_familia = '{familia}' and codigo_linea = '{linea}' and status_grupo = 1";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "catalogo_grupo", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return CatGrupo;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return CatGrupo;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    CatGrupo.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
                }
            }
            return CatGrupo;
        }

        public static ObservableCollection<Clases.clsSeccion> LlenarSecciones(string Sucursal = "", string Encontrado = "", bool EsHerramienta = false)
        {
            Secciones = new ObservableCollection<clsSeccion>();
            string Parametros = "id,Clave,Descripcion,Pasillos,NumeroPasillos,Estanterias,Sucursal";
            string Condicion = "1=1";

            if (!EsHerramienta)
            {
                if (Encontrado != "") Condicion = $"Clave in ({Encontrado}) and Sucursal='{Sucursal}'";
            }
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "CatalogoSecciones", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return Secciones;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return Secciones;

                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Secciones.Add(new Clases.clsSeccion
                    {
                        ID = int.Parse(r[0].ToString().Trim()),
                        Clave = r[1].ToString().Trim(),
                        Descripcion = r[2].ToString().Trim(),
                        Pasillos = Boolean.Parse(r[3].ToString().Trim()),
                        NumeroPasillos = int.Parse(r[4].ToString().Trim()),
                        Estanterias = Boolean.Parse(r[5].ToString().Trim()),
                        Sucursal = r[6].ToString().Trim()
                    });
                }
            }
            return Secciones;
        }

        public static ObservableCollection<Clases.clsEstanteria> LlenarEstanterias(string Sucursal, string Seccion, string Pasillo = "")
        {
            Estanterias = new ObservableCollection<clsEstanteria>();
            string Parametros = "id,Clave,Tipo,Descripcion,Seccion,Pasillo,NumeroNiveles,Tarimas,NumeroTarimas,Contenedores,ReiniciarNumeracionContenedores,NumeroContenedores,NumeroContenedoresTarima,Sucursal";
            string Condicion = $"Seccion='{Seccion}' and Sucursal='{Sucursal}'";
            if (!string.IsNullOrEmpty(Pasillo))
                Condicion += $" and Pasillo = '{Pasillo}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "CatalogoEstanterias", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return Estanterias;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return Estanterias;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    Estanterias.Add(new Clases.clsEstanteria
                    {
                        ID = int.Parse(r[0].ToString().Trim()),
                        Clave = r[1].ToString().Trim(),
                        Tipo = r[2].ToString().Trim(),
                        Descripcion = r[3].ToString().Trim(),
                        Seccion = r[4].ToString().Trim(),
                        Pasillo = int.Parse(r[5].ToString().Trim()),
                        NumeroNiveles = int.Parse(r[6].ToString().Trim()),
                        Tarimas = Boolean.Parse(r[7].ToString().Trim()),
                        NumeroTarimas = int.Parse(r[8].ToString().Trim()),
                        Cajas = Boolean.Parse(r[9].ToString().Trim()),
                        ReiniciarNumeracionCajas = Boolean.Parse(r[10].ToString().Trim()),
                        NumeroCajas = int.Parse(r[11].ToString().Trim()),
                        NumeroCajasTarima = int.Parse(r[12].ToString().Trim()),
                        Sucursal = r[13].ToString().Trim(),
                    });
                }
            }
            return Estanterias;
        }

        public static List<string> LlenarSeries(string CodigoArticulo)
        {
            lSeries = new List<string>();
            lSeries.Add("");
            string Parametros = $"{CodigoArticulo}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/InventarioAlmacenH", Parametros, "wsp_NumerosSeriexArticulo");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return lSeries;
                string resp = reader.ReadToEnd();
                if (resp == "[]") return lSeries;
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    lSeries.Add(r[0].ToString().Trim());
                }
            }
            return lSeries;
        }
    }
}