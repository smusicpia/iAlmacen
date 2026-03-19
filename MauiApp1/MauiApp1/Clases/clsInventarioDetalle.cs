using SQLite;

namespace iAlmacen.Clases;

public class clsInventarioDetalle
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string folioInventario { get; set; }
    public string idReferencia { get; set; }
    public string Seccion { get; set; }
    public string DescripcionSeccion { get; set; }    //*
    public int Pasillo { get; set; }
    public string Estanteria { get; set; }
    public string DescripcionEstanteria { get; set; }    //*
    public int Nivel { get; set; }
    public int Tarima { get; set; }
    public int Caja { get; set; }
    public string Familia { get; set; }
    public string DescFamilia { get; set; }
    public string Linea { get; set; }
    public string Grupo { get; set; }
    public string CodigoArticulo { get; set; }
    public double Existencia { get; set; }
    public string UnidadControl { get; set; }
    public string Fecha { get; set; }
    public string Hora { get; set; }
    public string Usuario { get; set; }
    public string Sucursal { get; set; }
    public string nserie { get; set; }
    public string Clasificacion { get; set; }
    //public string tProyecto { get; set; }
    //public string Metodo { get; set; }

    public override string ToString()
    {
        return string.Format("[clsInventarioDetalle: ID={0},folioInventario={1},idReferencia={2}," +
                             "CodigoArticulo={3},Sucursal={4},Seccion={5},Pasillo={6}," +
                             "Estanteria={7},Nivel={8},Familia={9},Linea={10}," +
                             "Grupo={11},Tarima={12},Caja={13},UnidadControl={14}," +
                             "Existencia={15},DescFamilia={16},nserie={17}],Clasificacion={18}",
                             ID,
                             folioInventario,
                             idReferencia,
                             CodigoArticulo,
                             Sucursal,
                             Seccion,
                             Pasillo,
                             Estanteria,
                             Nivel,
                             Familia,
                             Linea,
                             Grupo,
                             Tarima,
                             Caja,
                             UnidadControl,
                             Existencia, DescFamilia, nserie, Clasificacion);
    }
}