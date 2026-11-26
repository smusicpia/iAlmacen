using iAlmacen.Models;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Data;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace iAlmacen.Clases;

public class Funciones
{
	public static HttpContent jsonContent<T>(T obj)
	{
		var json = System.Text.Json.JsonSerializer.Serialize(obj, System.Text.Json.JsonSerializerOptions.Default);
		return new StringContent(json, Encoding.UTF8, "application/json");
	}

	//public static string streamToByteArray(Stream input)
	//{
	//	byte[] byteArray;
	//	using (var memoryStream = new MemoryStream())
	//	{
	//		input.CopyTo(memoryStream);
	//		byteArray = memoryStream.ToArray();
	//	}
	//	return Convert.ToBase64String(byteArray, Base64FormattingOptions.InsertLineBreaks);
	//}

	public static string ConvertToBase64String(Stream stream)
	{
		byte[] imageBytes;
		using (BinaryReader reader = new BinaryReader(stream))
		{
			imageBytes = reader.ReadBytes((int)stream.Length);
		}
		return Convert.ToBase64String(imageBytes);
	}

	public async static Task<HttpStatusCode> Login(string user, string pass)
	{
		HttpClient client = HttpClienteInstancia.ClientLogin;
		UserLogin usuario = new UserLogin();
		usuario.Username = user;
		usuario.Password = pass;

		using (var myAes = Aes.Create())
		{
			Global.Key = myAes.Key;
			Global.IV = myAes.IV;
		}

		HttpResponseMessage response;
		try
		{
			response = await client.PostAsync($"api/login/Authenticate?tProyecto={ConfigAPI.TipoProyecto}", jsonContent(usuario));
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({client.BaseAddress}): {ex}");
			return HttpStatusCode.ServiceUnavailable;
		}
		if (response.IsSuccessStatusCode)
		{
			string resp = await response.Content.ReadAsStringAsync();
			DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
			foreach (DataRow r in dt.Rows)
			{
				Global.clave_usuario = user;
				Global.nombre_usuario = ConfigAPI.TipoProyecto == "D" && string.IsNullOrEmpty(r["usuario"].ToString()) ? "Servando Hernandez" : r["usuario"].ToString();
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
				//TODO Agregar Guid
				Global.guid = r["Guid"].ToString();
				Preferences.Default.Set("tokenAPI", Global.tokenAPI);
				Preferences.Default.Set("refreshTokenAPI", Global.refreshTokenAPI);
				//TODO Agregar Guid
				Preferences.Default.Set("Guid", Global.guid);
				Preferences.Default.Set("clave_usuario", Global.clave_usuario);
				Preferences.Default.Set("Password", Global.pass);
				if (Global.tokenAPI != "")
					break;
			}
		}

		return response.StatusCode;
	}

	public static bool ChkConnected()
	{
		IEnumerable<ConnectionProfile> profiles = Connectivity.Current.ConnectionProfiles;
		if (profiles.Contains(ConnectionProfile.WiFi))
		{
			return true;
		}
		return false;
	}

	public async static Task<DataTable?> LlenarArea()
	{
		DataTable? dtArea = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/Areas?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={(string.IsNullOrEmpty(Global.strSucursal) ? "M" : Global.strSucursal)}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtArea = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtArea;
	}

	public async static Task<DataTable?> LlenarAreaH()
	{
		DataTable? dtAreaH = new DataTable();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/AreasH?tProyecto={ConfigAPI.TipoProyecto}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtAreaH = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtAreaH;
	}

	public async static Task<DataTable?> LlenarCentroCostoN1()
	{
		DataTable? dtccN1 = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/centroscostoN1?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}&Area={Global.strArea}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtccN1 = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtccN1;
	}

	public async static Task<DataTable?> LlenarCentroCostoN2()
	{
		DataTable? dtccN2 = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/centroscostoN2?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}&Area={Global.strArea}&ccN1={Global.strCCnivel1}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtccN2 = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtccN2;
	}

	public async static Task<DataTable?> LlenarCentroCostoN3()
	{
		DataTable? dtccN3 = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/centroscostoN3?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}&Area={Global.strArea}&ccN1={Global.strCCnivel1}&ccN2={Global.strCCnivel2}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtccN3 = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtccN3;
	}

	public async static Task<DataTable?> LlenarCentroCostoN4()
	{
		DataTable? dtccN4 = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/centroscostoN4?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}&Area={Global.strArea}&ccN1={Global.strCCnivel1}&ccN2={Global.strCCnivel2}&ccN3={Global.strCCnivel3}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtccN4 = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtccN4;
	}

	public async static Task<DataTable?> LlenarResponsables()
	{
		DataTable? dtResponsable = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/responsable?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtResponsable = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtResponsable;
	}

	public async static Task<DataTable?> LlenarAutorizado()
	{
		DataTable? dtAutorizado = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/autorizador?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Global.strSucursal}&Area={Global.strArea}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtAutorizado = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtAutorizado;
	}

	public async static Task<DataTable?> LlenarAlmacenistas()
	{
		DataTable? dtAlmacenistas = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/almacenistas?tProyecto={ConfigAPI.TipoProyecto}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtAlmacenistas = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtAlmacenistas;
	}

	public async static Task<DataTable?> LlenarCategoriaFamilias(bool EsHerramienta = false)
	{
		DataTable? dtfamilias = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/familias?tProyecto={ConfigAPI.TipoProyecto}&esHerramienta={EsHerramienta}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtfamilias = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtfamilias;
	}

	public async static Task<DataTable?> LlenarCategoriaLineas(string familia)
	{
		DataTable? dtLineas = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/lineas?tProyecto={ConfigAPI.TipoProyecto}&familia={familia}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtLineas = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtLineas;
	}

	public async static Task<DataTable?> LlenarCategoriaGrupo(string familia, string linea)
	{
		DataTable? dtGrupos = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/grupos?tProyecto={ConfigAPI.TipoProyecto}&familia={familia}&linea={linea}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtGrupos = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtGrupos;
	}

	public async static Task<DataTable?> ObtenerFirma(int folio)
	{
		DataTable? dtfirma = new();

		string firma = string.Empty;
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/Operacion/firma?tProyecto={ConfigAPI.TipoProyecto}&folio_={folio}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtfirma = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtfirma;
	}

	public async static Task<DataTable?> LlenarSecciones(string Sucursal = "M", string Encontrado = "")
	{
		DataTable? dtSecciones = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/secciones?tProyecto={ConfigAPI.TipoProyecto}{(!string.IsNullOrEmpty(Sucursal) ? $"&Sucursal={Sucursal}" : "&Sucursal=M")}{(!string.IsNullOrEmpty(Encontrado) ? $"&encontrado={Encontrado}" : "")}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtSecciones = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtSecciones;
	}

	public async static Task<DataTable?> LlenarEstanterias(string Sucursal, string Seccion, string? Pasillo = "")
	{
		DataTable? dtEstanterias = new();

		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/estanterias?tProyecto={ConfigAPI.TipoProyecto}&Sucursal={Sucursal}&Seccion={Seccion}{(!string.IsNullOrEmpty(Pasillo) ? $"&Pasillo={Pasillo}" : "")}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtEstanterias = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtEstanterias;
	}

	public async static Task<DataTable?> LlenarSeries(string CodigoArticulo)
	{
		DataTable? dtSeries = new();
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.GetAsync($"api/catalogo/series?tProyecto={ConfigAPI.TipoProyecto}&CodigoArticulo={CodigoArticulo}");
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				dtSeries = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		return dtSeries;
	}
}