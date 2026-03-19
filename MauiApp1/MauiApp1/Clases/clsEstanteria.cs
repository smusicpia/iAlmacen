using SQLite;

namespace iAlmacen.Clases;

public class clsEstanteria
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string Clave { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public string Seccion { get; set; }
    public int Pasillo { get; set; }
    public int NumeroNiveles { get; set; }
    public Boolean Tarimas { get; set; }
    public int NumeroTarimas { get; set; }
    public Boolean Cajas { get; set; }
    public Boolean ReiniciarNumeracionCajas { get; set; }
    public int NumeroCajas { get; set; }
    public int NumeroCajasTarima { get; set; }
    public string Sucursal { get; set; }

    public override string ToString()
    {
        return string.Format("[clsEstanteria: ID={0}, Clave={1}, Tipo={2}, Descripcion={3}, " +
                             "Seccion={4}, Pasillo={5}, NumeroNiveles={6}," +
                             "Tarimas={7}, NumeroTarimas={8}, Cajas={9}," +
                             "ReiniciarNumeracionCajas={10}, NumeroCajas={11}, NumeroCajasTarima={12}, Sucursal={13}]",
                             ID, Clave, Tipo, Descripcion, Seccion, Pasillo, NumeroNiveles, Tarimas, NumeroTarimas, Cajas,
                             ReiniciarNumeracionCajas, NumeroCajas, NumeroCajasTarima, Sucursal);
    }
}