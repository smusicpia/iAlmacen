using iAlmacen.Clases;
using iAlmacen.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;

namespace iAlmacen.WebApi
{
    public class ConfigAPI
    {
        //**************************************************
        //** TipoProyecto = D (Pruebas, Desarrollo, Test) **
        //** TipoProyecto = P (Produccion)                **
        //**************************************************

        public static string Servidor = "http://192.168.0.204:8055/";
        public static string TipoProyecto = "P";     // "P" = Produccion, "D" = Desarrollo
        public static bool Prueba = false;

        //public static string Servidor = "http://192.168.0.204:8056/";
        //public static string TipoProyecto = "P";    // "P" = Produccion, "D" = Desarrollo
        //public static bool Prueba = false;

        public static string Metodo = "api/Usuario";
        public static string ContentType = "application/json";
        //private readonly IAlertDialogService alertDialogService = DependencyService.Get<IAlertDialogService>();

        public static class HttpMethods
        {
            public static string Get = "GET";
            public static string Post = "POST";
            public static string Put_Modify = "PUT";
            public static string Patch_Modify = "PATCH";
            public static string Delete = "DELETE";
        }

        private static string streamToByteArray(Stream input)
        {
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                byteArray = memoryStream.ToArray();
            }
            return Convert.ToBase64String(byteArray, Base64FormattingOptions.InsertLineBreaks);
        }

        public static bool AceptarTodosLosCertificados(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static HttpWebResponse GetAPI(string MetodoHttp, string Controllador, string Parametros, string MetodoAPI, string Tabla = "", string Condicion = "", string Accion = "", string Campos = "")
        {
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            switch (MetodoHttp)
            {
                case "POST":
                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
                    request.Method = ConfigAPI.HttpMethods.Post;
                    break;

                case "PUT":
                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
                    request.Method = ConfigAPI.HttpMethods.Put_Modify;
                    break;

                case "PATCH":
                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}") as HttpWebRequest;
                    request.Method = ConfigAPI.HttpMethods.Patch_Modify;
                    break;

                case "DELETE":
                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
                    request.Method = ConfigAPI.HttpMethods.Delete;
                    break;

                default:
                    switch (Accion)
                    {
                        case "":
                            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
                            break;

                        default:
                            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}&Accion={Accion}&Campos={Campos}") as HttpWebRequest;
                            break;
                    }
                    request.Method = ConfigAPI.HttpMethods.Get;
                    break;
            }

            if (MetodoAPI == "LoginWebserver")
            {
                request.Headers.Add("Aes1", $"{Convert.ToBase64String(Global.Key)}");
                request.Headers.Add("Aes2", $"{Convert.ToBase64String(Global.IV)}");
            }

            //TODO: Authorization y Authentication tokenAPI
            request.Accept = "application/json";
            if (MetodoAPI != "LoginWebserver" && (Global.tokenAPI != "" || Global.refreshTokenAPI != ""))
            {
                request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
                //TODO: Refresh tokenAPI
                request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            }

            request.ContentType = "application/json";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                wRespStatusCode = response.StatusCode;
                return response;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                }
                Preferences.Remove("logueado", string.Empty);
                Preferences.Default.Remove("tokenAPI", string.Empty);
                Preferences.Default.Remove("refreshTokenAPI", string.Empty);
            }
            return response;
        }

        public static HttpWebResponse PostAPI_InventarioAlmacenDet(string Controllador, ObservableCollection<clsInventarioDetalle> inventarioDetalle)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador) as HttpWebRequest;
            string json = JsonConvert.SerializeObject(inventarioDetalle);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return response;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                wRespStatusCode = response.StatusCode;
                return response;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return response;
        }

        public static DataTable PostAPI_NvaPlantillaH(string Controllador, string MetodoAPI, ObservableCollection<InventarioAlmacen> Obj)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
            string json = JsonConvert.SerializeObject(Obj);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }

        public static DataTable PostAPI_GenerarNumerosSeries(string Controllador, string MetodoAPI, ObservableCollection<CatalogoArticuloNumeroSeries> Obj)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
            string json = JsonConvert.SerializeObject(Obj);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }

        public static DataTable PostAPI_GuardarInventario(string Controllador, string MetodoAPI, ObservableCollection<clsInventarioDetalle> Obj)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
            string json = JsonConvert.SerializeObject(Obj);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }

        public static DataTable PostAPI_DocumentoAlmacen(string Controllador, string MetodoAPI, ObservableCollection<DocumentoAlmacen> Obj)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
            string json = JsonConvert.SerializeObject(Obj);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }

        public static DataTable PostAPI_DocumentoAlmacenDetalle(string Controllador, string MetodoAPI, ObservableCollection<DocumentoAlmacenDetalle> Obj)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
            string json = JsonConvert.SerializeObject(Obj);
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }

        public static HttpWebResponse PostAPI_Firma(string Controllador, string Parametros, string MetodoAPI, Stream firma)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador) as HttpWebRequest;
            string json = $"{{\"tProyecto\":\"{TipoProyecto}\",\"Metodo\":\"{MetodoAPI}\",\"folio\":\"{Parametros}\",\"firma\":\"{streamToByteArray(firma)}\"}}";
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return response;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                wRespStatusCode = response.StatusCode;
                return response;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return response;
        }

        public static HttpWebResponse PostAPI_Imagen(string Controllador, string Parametros, string MetodoAPI, Stream Imagen)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador) as HttpWebRequest;
            string json = $"{{\"tProyecto\":\"{TipoProyecto}\",\"Metodo\":\"{MetodoAPI}\",\"folio\":\"{Parametros}\",\"firma\":\"{streamToByteArray(Imagen)}\"}}";
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return response;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                wRespStatusCode = response.StatusCode;
                return response;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return response;
        }

        public static DataTable PostAPI_Foto_Proterm(string Controllador, string Parametros, string MetodoAPI, Stream Imagen)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            HttpStatusCode wRespStatusCode;
            DataTable dt = new DataTable();

            request = WebRequest.Create(ConfigAPI.Servidor + Controllador) as HttpWebRequest;
            string json = $"{{\"tProyecto\":\"{TipoProyecto}\",\"Metodo\":\"{MetodoAPI}\",\"folio\":\"{Parametros}\",\"firma\":\"{streamToByteArray(Imagen)}\"}}";
            request.Method = ConfigAPI.HttpMethods.Post;
            //TODO: Authorization y Authentication tokenAPI
            request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
            request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
            request.ContentType = "application/json";
            request.Accept = "application/json";
            using (var streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(json);
                streamWrite.Flush();
                streamWrite.Close();
            }
            try
            {
                string responseBody;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return dt;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
                wRespStatusCode = response.StatusCode;
                return dt;
            }
            catch (WebException ex)
            {
                using (WebResponse responsed = ex.Response)
                {
                    response = (HttpWebResponse)responsed;
                    request = null;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
                    }
                }
            }
            return dt;
        }
    }
}