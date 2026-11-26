namespace iAlmacen.WebApi
{
	public class ConfigAPI
	{
		//**************************************************
		//** TipoProyecto = D (Pruebas, Desarrollo, Test) **
		//** TipoProyecto = P (Produccion)                **
		//**************************************************

		//public static string Servidor = "http://192.168.0.204:8055/";
		public static string TipoProyecto = "D";     // "P" = Produccion, "D" = Desarrollo
		public static bool Prueba = TipoProyecto == "P" ? false : true;
		public static string Servidor = "http://192.168.0.235:51/"; //"https://localhost:44301/";
																	//public static string TipoProyecto = "D";    // "P" = Produccion, "D" = Desarrollo
																	//public static bool Prueba = true;
	}
}