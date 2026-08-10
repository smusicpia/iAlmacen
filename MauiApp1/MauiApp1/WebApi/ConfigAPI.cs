using iAlmacen.Clases;
using iAlmacen.Handlers;
using iAlmacen.Models;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Text.Json;

namespace iAlmacen.WebApi
{
    public class ConfigAPI
    {
        private static readonly HttpClient _httpClient;
		private static readonly JsonSerializerOptions _jsonOptions;
        //**************************************************
        //** TipoProyecto = D (Pruebas, Desarrollo, Test) **
        //** TipoProyecto = P (Produccion)                **
        //**************************************************

        public static string Servidor = "http://192.168.0.204:8055/";
        public static string TipoProyecto = "P";     // "P" = Produccion, "D" = Desarrollo
        public static bool Prueba = TipoProyecto=="P" ? false : true;

        //public static string Servidor = "https://localhost:44398/";
        //public static string TipoProyecto = "D";    // "P" = Produccion, "D" = Desarrollo
        //public static bool Prueba = true;

        public static string Metodo = "api/Usuario";
        public static string ContentType = "application/json";
		//private readonly IAlertDialogService alertDialogService = DependencyService.Get<IAlertDialogService>();

		static ConfigAPI()
		{
			//HttpClientHandler insecureHandler = GetInsecureHandler();

			// 1. Instancia el manejador final que se conecta a internet
			var networkHandler = new HttpClientHandler();

			// 2. Crea tu manejador de refresco pasándole el de red como su "InnerHandler"
			
            var refreshHandler = new RefreshTokenHandler()
			{
				InnerHandler = networkHandler
			};

			_httpClient = new HttpClient(refreshHandler)
			{
				Timeout = TimeSpan.FromSeconds(5), // Ajusta el tiempo de espera según tus necesidades
				BaseAddress = new Uri(ConfigAPI.Servidor), // Cambia esto por la URL de tu API
			};
			_jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				//PropertyNameCaseInsensitive = true,
				//DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
				WriteIndented = true
			};
		}

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

        //public static HttpWebResponse GetAPI(string MetodoHttp, string Controllador, string Parametros, string MetodoAPI, string Tabla = "", string Condicion = "", string Accion = "", string Campos = "*")
        //{
        //    //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    HttpStatusCode wRespStatusCode;
        //    switch (MetodoHttp)
        //    {
        //        case "POST":
        //            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
        //            request.Method = ConfigAPI.HttpMethods.Post;
        //            break;

        //        case "PUT":
        //            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
        //            request.Method = ConfigAPI.HttpMethods.Put_Modify;
        //            break;

        //        case "PATCH":
        //            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}") as HttpWebRequest;
        //            request.Method = ConfigAPI.HttpMethods.Patch_Modify;
        //            break;

        //        case "DELETE":
        //            request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
        //            request.Method = ConfigAPI.HttpMethods.Delete;
        //            break;

        //        default:
        //            switch (Accion)
        //            {
        //                case "":
        //                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}") as HttpWebRequest;
        //                    break;

        //                default:
        //                    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}&Accion={Accion}&Campos={Campos}") as HttpWebRequest;
        //                    break;
        //            }
        //            request.Method = ConfigAPI.HttpMethods.Get;
        //            break;
        //    }

        //    if (MetodoAPI == "LoginWebserver")
        //    {
        //        request.Headers.Add("aes1", $"{Convert.ToBase64String(Global.Key)}");
        //        request.Headers.Add("aes2", $"{Convert.ToBase64String(Global.IV)}");
        //    }

        //    //TODO: Authorization y Authentication tokenAPI
        //    request.Accept = "application/json";
        //    if (MetodoAPI != "LoginWebserver" && (Global.tokenAPI != "" || Global.refreshTokenAPI != ""))
        //    {
        //        request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
        //        //TODO: Refresh tokenAPI
        //        request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
        //    }

        //    request.ContentType = "application/json";
        //    try
        //    {
        //        response = (HttpWebResponse)request.GetResponse();
        //        wRespStatusCode = response.StatusCode;
        //        return response;
        //    }
        //    catch (WebException ex)
        //    {
        //        using (WebResponse responsed = ex.Response)
        //        {
        //            response = (HttpWebResponse)responsed;
        //            request = null;
        //        }
        //        Preferences.Remove("logueado", string.Empty);
        //        Preferences.Default.Remove("tokenAPI", string.Empty);
        //        Preferences.Default.Remove("refreshTokenAPI", string.Empty);
        //    }
        //    return response;
        //}

        private static HttpRequestMessage HttpRequestHeader(HttpRequestMessage httpRequest, string MetodoAPI, string Content = "")
        {
            httpRequest.Headers.Add("ContentType", "application/json");
			if (MetodoAPI == "LoginWebserver")
			{
				httpRequest.Headers.Add("aes1", $"{Convert.ToBase64String(Global.Key)}");
				httpRequest.Headers.Add("aes2", $"{Convert.ToBase64String(Global.IV)}");
			}

			//TODO: Authorization y Authentication tokenAPI
			if (MetodoAPI != "LoginWebserver" && (Global.tokenAPI != "" || Global.refreshTokenAPI != ""))
			{
                if (_httpClient.DefaultRequestHeaders.Authorization == null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Global.tokenAPI);
                    //            if (_httpClient.DefaultRequestHeaders.Contains("RefreshToken"))
                    //            {
                    //                _httpClient.DefaultRequestHeaders.GetValues("RefreshToken").ToList().ForEach(x => _httpClient.DefaultRequestHeaders.Remove("RefreshToken"));
                    //}
                }
                httpRequest.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");

				//_httpClient.DefaultRequestHeaders.Add("RefreshToken", $"{Global.refreshTokenAPI}");
			}
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }

			var content = new StringContent(Content, null, "application/json");
			httpRequest.Content = content;

			return httpRequest;
		}

		public static async Task<HttpResponseMessage> GetAPI(string MetodoHttp, string Controllador, string Parametros, string MetodoAPI, string Tabla = "", string Condicion = "", string Accion = "", string Campos = "*")
		{
			var request = new HttpRequestMessage();
			string jsonresponse = string.Empty;

			HttpStatusCode wRespStatusCode;
			switch (MetodoHttp)
			{
				case "POST":
                    request = new HttpRequestMessage(HttpMethod.Post, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
                    HttpRequestHeader(request, MetodoAPI);
					//response = await _httpClient.PostAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}", null);
					break;

				case "PUT":
					//response = await _httpClient.PutAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}", null);
                    request = new HttpRequestMessage(HttpMethod.Put, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
					HttpRequestHeader(request, MetodoAPI);
					break;

				case "PATCH":
					//response = await _httpClient.PatchAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}", null);
                    request = new HttpRequestMessage(HttpMethod.Patch, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}");
					HttpRequestHeader(request, MetodoAPI);
					break;

				case "DELETE":
					//response = await _httpClient.DeleteAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
					request = new HttpRequestMessage(HttpMethod.Delete, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
					HttpRequestHeader(request, MetodoAPI);
					break;

				default:
					switch (Accion)
					{
						case "":
							//jsonresponse = await _httpClient.GetStringAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
                            request = new HttpRequestMessage(HttpMethod.Get, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&parametros={Parametros}");
							HttpRequestHeader(request, MetodoAPI);
							break;

						default:
							//jsonresponse = await _httpClient.GetStringAsync(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}&Accion={Accion}&Campos={Campos}");
                            request = new HttpRequestMessage(HttpMethod.Get, ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}&Tabla={Tabla}&parametros={Parametros}&Condicion={Condicion}&Accion={Accion}&Campos={Campos}");
							HttpRequestHeader(request, MetodoAPI);
							break;
					}
					//request.Method = ConfigAPI.HttpMethods.Get;
					break;
			}

			try
			{
				var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
				//var response = await _httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();
                return response;
			}
			catch (WebException ex)
			{
				Preferences.Remove("logueado", string.Empty);
				Preferences.Default.Remove("tokenAPI", string.Empty);
				Preferences.Default.Remove("refreshTokenAPI", string.Empty);
				return new HttpResponseMessage(HttpStatusCode.BadRequest);
			}
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

            try
            {
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

        //public static DataTable PostAPI_GuardarInventario(string Controllador, string MetodoAPI, ObservableCollection<Item_InventarioDetalle> Obj)
        //{
        //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    HttpStatusCode wRespStatusCode;
        //    DataTable dt = new DataTable();

        //    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
        //    string json = JsonConvert.SerializeObject(Obj);
        //    request.Method = ConfigAPI.HttpMethods.Post;
        //    //TODO: Authorization y Authentication tokenAPI
        //    request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
        //    request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
        //    request.ContentType = "application/json";
        //    request.Accept = "application/json";

        //    try
        //    {
        //        using (var streamWrite = new StreamWriter(request.GetRequestStream()))
        //        {
        //            streamWrite.Write(json);
        //            streamWrite.Flush();
        //            streamWrite.Close();
        //        }

        //        string responseBody;
        //        using (response = (HttpWebResponse)request.GetResponse())
        //        {
        //            using (Stream strReader = response.GetResponseStream())
        //            {
        //                if (strReader == null) return dt;
        //                using (StreamReader objReader = new StreamReader(strReader))
        //                {
        //                    responseBody = objReader.ReadToEnd();
        //                }
        //            }
        //        }

        //        dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
        //        wRespStatusCode = response.StatusCode;
        //        return dt;
        //    }
        //    catch (WebException ex)
        //    {
        //        using (WebResponse responsed = ex.Response)
        //        {
        //            response = (HttpWebResponse)responsed;
        //            request = null;
        //            if (response.StatusCode == HttpStatusCode.Unauthorized)
        //            {
        //                HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
        //            }
        //        }
        //    }
        //    return dt;
        //}

        //public static DataTable PostAPI_DocumentoAlmacen(string Controllador, string MetodoAPI, ObservableCollection<DocumentoAlmacen> Obj)
        //{
        //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ConfigAPI.AceptarTodosLosCertificados);
        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    HttpStatusCode wRespStatusCode;
        //    DataTable dt = new DataTable();

        //    request = WebRequest.Create(ConfigAPI.Servidor + Controllador + $"/?tProyecto={TipoProyecto}&Metodo={MetodoAPI}") as HttpWebRequest;
        //    string json = JsonConvert.SerializeObject(Obj);
        //    request.Method = ConfigAPI.HttpMethods.Post;
        //    //TODO: Authorization y Authentication tokenAPI
        //    request.Headers.Add("Authorization", $"Bearer {Global.tokenAPI}");
        //    request.Headers.Add("RefreshToken", $"{Global.refreshTokenAPI}");
        //    request.ContentType = "application/json";
        //    request.Accept = "application/json";
        //    using (var streamWrite = new StreamWriter(request.GetRequestStream()))
        //    {
        //        streamWrite.Write(json);
        //        streamWrite.Flush();
        //        streamWrite.Close();
        //    }
        //    try
        //    {
        //        string responseBody;
        //        using (response = (HttpWebResponse)request.GetResponse())
        //        {
        //            using (Stream strReader = response.GetResponseStream())
        //            {
        //                if (strReader == null) return dt;
        //                using (StreamReader objReader = new StreamReader(strReader))
        //                {
        //                    responseBody = objReader.ReadToEnd();
        //                }
        //            }
        //        }

        //        dt = (DataTable)JsonConvert.DeserializeObject(responseBody, (typeof(DataTable)));
        //        wRespStatusCode = response.StatusCode;
        //        return dt;
        //    }
        //    catch (WebException ex)
        //    {
        //        using (WebResponse responsed = ex.Response)
        //        {
        //            response = (HttpWebResponse)responsed;
        //            request = null;
        //            if (response.StatusCode == HttpStatusCode.Unauthorized)
        //            {
        //                HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
        //            }
        //        }
        //    }
        //    return dt;
        //}

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