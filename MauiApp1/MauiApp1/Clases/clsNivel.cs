using SQLite;

namespace iAlmacen.Clases;

public class clsNivel
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string codigo_pasillo { get; set; }
    public string codigo_anaquel { get; set; }
    public string Nivel { get; set; }
    public string psucursal { get; set; }

    public override string ToString()
    {
        return string.Format("[clsNivel: ID={0}, codigo_pasillo={1}, codigo_anaquel={2}, " +
                             "Nivel={3}, psucursal={4}]",
                             ID, codigo_pasillo, codigo_anaquel, Nivel, psucursal);
    }
}