using iAlmacen.Clases;
using iAlmacen.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;

namespace iAlmacen.WebApi;

public class APIService
{
    private static readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions;
    static APIService()
    {
        //HttpClientHandler insecureHandler = GetInsecureHandler();
        _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(20), // Ajusta el tiempo de espera según tus necesidades
            BaseAddress = new Uri(ConfigAPI.Servidor) // Cambia esto por la URL de tu API
        };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //PropertyNameCaseInsensitive = true,
            //DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
        _httpClient.DefaultRequestHeaders.Add("RefreshToken", $"Bearer {Global.refreshTokenAPI}");
    }

    // This method must be in a class in a platform project, even if
    // the HttpClient object is constructed in a shared project.
    //public static HttpClientHandler GetInsecureHandler()
    //{
    //    HttpClientHandler handler = new HttpClientHandler();
    //    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    //    {
    //        if (cert.Issuer.Equals("CN=localhost"))
    //            return true;
    //        return errors == System.Net.Security.SslPolicyErrors.None;
    //    };
    //    return handler;
    //}

    public static async Task<HttpResponseMessage> GetPostAPI_NvaPlantillaHAsync(string Controllador, string MetodoAPI, ObservableCollection<InventarioAlmacen> Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsync($"{Controllador}/{MetodoAPI}", jsonContent);
            //response.EnsureSuccessStatusCode();
            //var json = await response.Content.ReadAsStringAsync();
            //return JsonSerializer.Deserialize<DataTable>(json, _jsonOptions) ?? new DataTable();
            var response = await _httpClient.PostAsync(ConfigAPI.Servidor + $"/{Controllador}/CrearPlantilla/?tProyecto={ConfigAPI.TipoProyecto}", jsonContent).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            // Manejo de errores (puedes loguear o lanzar una excepción personalizada)
            throw new ApplicationException($"Error al obtener datos de {$"{Controllador}/{MetodoAPI}"}: {ex.Message}", ex);
        }
    }

    public static async Task<DataTable> PostAPI_DocumentoAlmacenDetalle(string Controllador, string MetodoAPI, ObservableCollection<DocumentoAlmacenDetalle> Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{Controllador}/{MetodoAPI}", jsonContent);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DataTable>(json, _jsonOptions) ?? new DataTable();
        }
        catch (Exception ex)
        {
            // Manejo de errores (puedes loguear o lanzar una excepción personalizada)
            throw new ApplicationException($"Error al obtener datos de {$"{Controllador}/{MetodoAPI}"}: {ex.Message}", ex);
        }
    }

    public static async Task<DataTable> PostAPI_DocumentoAlmacen(string Controllador, string MetodoAPI, ObservableCollection<DocumentoAlmacen> Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{Controllador}/{MetodoAPI}", jsonContent);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DataTable>(json, _jsonOptions) ?? new DataTable();
        }
        catch (Exception ex)
        {
            // Manejo de errores (puedes loguear o lanzar una excepción personalizada)
            throw new ApplicationException($"Error al obtener datos de {$"{Controllador}/{MetodoAPI}"}: {ex.Message}", ex);
        }
    }

	public static async Task<DataTable> PostAPI_GenerarNumerosSeries(string Controllador, string MetodoAPI, ObservableCollection<CatalogoArticuloNumeroSeries> Obj)
	{
		try
		{
			var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(ConfigAPI.Servidor + $"{Controllador}/{MetodoAPI}/?tProyecto={ConfigAPI.TipoProyecto}", jsonContent);
			response.EnsureSuccessStatusCode();
			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<DataTable>(json, _jsonOptions) ?? new DataTable();
		}
		catch (Exception ex)
		{
			// Manejo de errores (puedes loguear o lanzar una excepción personalizada)
			throw new ApplicationException($"Error al obtener datos de {$"{Controllador}/{MetodoAPI}"}: {ex.Message}", ex);
		}
	}

	public static async Task<HttpResponseMessage> PostAPI_GuardarInventario(string Controllador, ObservableCollection<Item_InventarioDetalle> Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
            //var request = new HttpRequestMessage(HttpMethod.Post, ConfigAPI.Servidor + $"/{Controllador}/GuardarInventario/?tProyecto={ConfigAPI.TipoProyecto}");
            //request.Content = jsonContent;
            //HttpResponseMessage response = await _httpClient.SendAsync(request);
            var response = await _httpClient.PostAsync(ConfigAPI.Servidor + $"/{Controllador}/GuardarInventario/?tProyecto={ConfigAPI.TipoProyecto}", jsonContent).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            // Manejo de errores (puedes loguear o lanzar una excepción personalizada)
            throw new ApplicationException($"Error al obtener datos de {$"{Controllador}/GuardarInventario"}: {ex.Message}", ex);
        }
    }

	public static async Task<HttpResponseMessage> PostAPI_GenerarSalida(string Controllador, string tProyecto, bool OrdenRecoleccion, string Responsable, string Autorizado, ObservableCollection<SalidaAlmacenEntity> Obj)
	{
		try
		{
			var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(ConfigAPI.Servidor + $"/{Controllador}?tProyecto={tProyecto}&OrdenRecoleccion={OrdenRecoleccion}&Responsable={Responsable}&Autorizado={Autorizado}", jsonContent).ConfigureAwait(false);
			return response;
		}
		catch (Exception ex)
		{
			// Manejo de errores (puedes loguear o lanzar una excepción personalizada)
			throw new ApplicationException($"Error al obtener datos de {$"{Controllador}"}: {ex.Message}", ex);
		}
	}

	public static async Task<HttpResponseMessage> PostAPI_Firma(string Controllador, FirmaEntity Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
            //var request = new HttpRequestMessage(HttpMethod.Post, ConfigAPI.Servidor + $"/{Controllador}/GuardarInventario/?tProyecto={ConfigAPI.TipoProyecto}");
            //request.Content = jsonContent;
            //HttpResponseMessage response = await _httpClient.SendAsync(request);
            var response = await _httpClient.PostAsync(ConfigAPI.Servidor + $"/{Controllador}", jsonContent).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            // Manejo de errores (puedes loguear o lanzar una excepción personalizada)
            throw new ApplicationException($"Error al obtener datos de {$"{Controllador}"}: {ex.Message}", ex);
        }
    }

}