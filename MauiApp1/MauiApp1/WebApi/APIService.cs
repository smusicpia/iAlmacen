using iAlmacen.Clases;
using iAlmacen.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Text.Json;

namespace iAlmacen.WebApi;

public class APIService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public APIService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ConfigAPI.Servidor) // Cambia esto por la URL de tu API
        };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
        _httpClient.DefaultRequestHeaders.Add("RefreshToken", $"Bearer {Global.refreshTokenAPI}");
    }

    public async Task<DataTable> GetPostAPI_NvaPlantillaHAsync(string Controllador, string MetodoAPI, ObservableCollection<InventarioAlmacen> Obj)
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
}