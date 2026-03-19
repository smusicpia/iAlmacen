using SQLite;

namespace iAlmacen.Clases;

public class RegArticulo
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string CodigoActual { get; set; }
    public string CodigoAnterior { get; set; }
    public string Descripcion { get; set; }
    public string ClaveFamilia { get; set; }
    public string ClaveLinea { get; set; }
    public string ClaveGrupo { get; set; }
    public string desc_familia { get; set; }
    public string desc_linea { get; set; }
    public string desc_grupo { get; set; }
    public string DescMarca { get; set; }
    public string DescMedida { get; set; }
    public string DescParte { get; set; }
    public double existencia { get; set; }
    public double Fisico { get; set; }
    public string FechaCapturado { get; set; }
    public string FechaAplicado { get; set; }
    public string Inventario { get; set; }
    public string Aplicado { get; set; }

    public string Fecha_ { get; set; }
    public string Usuario_ { get; set; }

    public string UnidadControl { get; set; }
    public double Costo { get; set; }

    public string Seccion { get; set; }
    public string DescSeccion { get; set; }
    public string Pasillo { get; set; }
    public string Estanteria { get; set; }
    public string DescEstanteria { get; set; }
    public string Nivel { get; set; }
    public string Tarima { get; set; }
    public string Contenedor { get; set; }
    public double ExisUbi { get; set; }

    public override string ToString()
    {
        return string.Format("[RegArticulo: ID={0}, CodigoActual={1}, CodigoAnterior={2}, Descripcion={3}, ClaveFamilia={4}, ClaveLinea={5}, ClaveGrupo={6}, " +
                             "DescFamilia={7}, DescLinea={8}, DescGrupo={9}, DescMarca={10}, DescMedida={11}, DescParte={12}, existencia={13}, Fisico={14}, " +
                             "FechaCapturado={15}, FechaAplicado={16}, Inventario={17}, Aplicado={18}, Fecha_={19}, Usuario_={20}, UnidadControl={21}, Costo={22}, " +
                             "Seccion={23}, Pasillo={24}, Estanteria={25}, Nivel={26}, Tarima={27}, Contenedor={28}, ExisUbi={29}]",
                             ID, CodigoActual, CodigoAnterior, Descripcion, ClaveFamilia, ClaveLinea, ClaveGrupo,
                             desc_familia, desc_linea, desc_grupo, DescMarca, DescMedida, DescParte, existencia, Fisico,
                             FechaCapturado, FechaAplicado, Inventario, Aplicado, Fecha_, Usuario_, UnidadControl, Costo,
                             Seccion, Pasillo, Estanteria, Nivel, Tarima, Contenedor, ExisUbi);
    }

    //[System.ComponentModel.Browsable(false)]
    //public int Count { get; }
}