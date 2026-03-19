using SQLite;

namespace iAlmacen.Clases;

public class RegHerramienta
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string folio_resguardo { get; set; }
    public string fecha_entrega { get; set; }

    //public string hora_entrega { get; set; }
    //public string fecha_devolucion { get; set; }
    //public string indefinido { get; set; }
    public string codigo_area { get; set; }

    public string desc_area { get; set; }
    public string codigo_autorizado { get; set; }
    public string nombre_empleado { get; set; }
    public string codigo_articulo { get; set; }
    public string descripcion_general { get; set; }
    public string desc_marca { get; set; }
    public string desc_medida { get; set; }
    public string desc_parte { get; set; }
    public string medida { get; set; }
    public double cantidad { get; set; }

    //public string saldo { get; set; }
    public string numero_serie { get; set; }

    //public string status_herramienta { get; set; }
    public string psucursal { get; set; }

    public string desc_familia { get; set; }
    public string desc_linea { get; set; }
    public string desc_grupo { get; set; }

    public string status_ { get; set; }
    public string icon_ { get; set; }

    public string fecha_inventario_ { get; set; }
    public string fecha_captura_ { get; set; }
    public string hora_captura_ { get; set; }
    public string usuario_captura_ { get; set; }

    public Boolean Sincronizado_ { get; set; }

    public override string ToString()
    {
        return string.Format("[RegHerramienta: ID={0}, folio_resguardo={1}, fecha_entrega={2}, codigo_area={3}, desc_area={4}, codigo_autorizado={5}, nombre_empleado={6}, " +
                             "codigo_articulo={7}, descripcion_general={8}, desc_marca={9}, desc_medida={10}, desc_parte={11}, medida={12}, cantidad={13}, numero_serie={14}, " +
                             "psucursal={15}, desc_familia={16}, desc_linea={17}, desc_grupo={18}, status_={19}, icon_={20}, " +
                             "fecha_inventario_={21}, fecha_captura_={22}, hora_captura_={23}, usuario_captura_={24}, Sincronizado_={25}]",
                             ID, folio_resguardo, fecha_entrega, codigo_area, desc_area, codigo_autorizado, nombre_empleado,
                             codigo_articulo, descripcion_general, desc_marca, desc_medida, desc_parte, medida, cantidad, numero_serie,
                             psucursal, desc_familia, desc_linea, desc_grupo, status_, icon_,
                             fecha_inventario_, fecha_captura_, hora_captura_, usuario_captura_, Sincronizado_);
    }

    //[System.ComponentModel.Browsable(false)]
    //public int Count { get; }
}