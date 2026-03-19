using SQLite;

namespace iAlmacen.Clases;

public class clsPasillos
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string codigo_pasillo { get; set; }
    public string descripcion_pasillo { get; set; }
    public string psucursal { get; set; }

    public override string ToString()
    {
        return string.Format("[clsPasillos: ID={0}, codigo_pasillo={1}, descripcion_pasillo={2}, psucursal={3}]",
                             ID, codigo_pasillo, descripcion_pasillo, psucursal);
    }
}