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

    public static async Task<HttpResponseMessage> GetPostAPI_NvaPlantillaHAsync(string Controllador, string MetodoAPI, ObservableCollection<InventarioAlmacen> Obj)
    {
        try
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(Obj, _jsonOptions), Encoding.UTF8, "application/json");
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
		var json = JsonSerializer.Serialize(Obj, _jsonOptions);
		var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
		string requestUri = ConfigAPI.Servidor + $"/{Controllador}?tProyecto={tProyecto}&OrdenRecoleccion={OrdenRecoleccion}&Responsable={Responsable}&Autorizado={Autorizado}";

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Espera máximo 10 segundos

		try
		{
			var response = await _httpClient.PostAsync(requestUri, jsonContent, cts.Token).ConfigureAwait(false);
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