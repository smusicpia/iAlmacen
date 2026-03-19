using SQLite;

namespace iAlmacen.Clases;

public class clsSucursal
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string Clave { get; set; }
    public string Descripcion { get; set; }

    public override string ToString()
    {
        return string.Format("[clsSeccion: ID={0}, Clave={1}, Descripcion={2}", ID, Clave, Descripcion);
    }
}