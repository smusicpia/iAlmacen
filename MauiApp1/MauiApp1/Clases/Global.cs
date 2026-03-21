using iAlmacen.Models;
using System.Collections.ObjectModel;

namespace iAlmacen.Clases;

public class Global
{
    public static Double version_ = 1.2;

    public static string weburl = "";
    public static string nombre_usuario = "";
    public static string privilegio_usuario = "";
    public static string pass = "";
    public static byte[] Key;
    public static byte[] IV;
    public static string clave_usuario = "";
    public static string clave_autotizacion = "";
    public static string tokenAPI = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InNoZXJuYW4iLCJuYmYiOjE3NDE5ODQ1ODcsImV4cCI6MTc0MTk4NjM4NywiaWF0IjoxNzQxOTg0NTg3LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDM5OC8iLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo0NDM5OC8ifQ.yKDxWEi2SCk_uXzmO2m7GcQex92naBkKebUbmtV_o0M";
    public static string refreshTokenAPI = "";

    public static Boolean Mitem_almacen = false;
    public static Boolean Mitem_proterm = false;
    public static Boolean Mitem_bascula = false;

    public static string mensaje_sistema = "";
    public static Boolean cierra_modulo = false;
    public static string modulo_afectado = "";
    public static Boolean cierra_sistema = false;

    public static double tasa_oper = 0;
    public static double tasa_super = 0;
    public static double tasa_admin = 0;

    public static string filtro_ = "";

    public static Boolean item_return = false;
    public static Item item_select;

    //public static Item_proterm item_protern;
    public static string fecha_inventario = "";

    public static string hora_inventario = "";
    public static string folio_orden_ = "";
    public static string folio_requisicion_ = "";
    public static string folio_cotizacion_ = "";
    public static string ParametrosControlArea = "";

    public static ObservableCollection<Item_orden_compra> Items_orden_ { get; set; }
    public static ObservableCollection<Item_RegArticulo> Items_RegArticulo { get; set; }
    public static ObservableCollection<Item_Virtual_Recoleccion> Items_recoleccion_ { get; set; }
    public static int folio_entrada_ = 0;

    public static Boolean validar = false;
    public static Boolean cmodo_prueba = false;
    public static string cfiltro_ = "";
    public static int cidsql_ = 0;

    #region Herramientas

    //Tablas SQlite
    public static string PathHerramienta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "db_herramientas.db");

    public static string HClaveEmpleado = "";
    public static string HNombreEmpleado = "";
    public static string HArea = "";
    #endregion Herramientas

    #region Articulos
    public static string PathCatalogo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "db_Catalogo.db");
    #endregion Articulos

    #region Inventarios
    public static string FolioInventario = "";
    public static RegArticulo ArticuloEnInventario;
    public static Item_RegArticulo HerramientaEnInventario;

    public static clsHerramientaEmpleados EmpleadoHerramienta;
    #endregion Inventarios

    #region Salidas
    public static ObservableCollection<clsArticuloSalida> regArticulosSalida; //= new ObservableCollection<Clases.clsArticuloSalida>();
    public static string strSucursal;
    public static string strArea;
    public static string strCCnivel1;
    public static string strCCnivel2;
    public static string strCCnivel3;
    public static string strCCnivel4;
    public static Boolean Editando;
    public static Clases.clsArticuloSalida ArticuloSalidaEditando;
    #endregion Salidas

    #region
    public static string ImpresoraEtiquetas;
    #endregion
}