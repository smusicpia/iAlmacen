using SQLite;

namespace iAlmacen.Clases
{
    public class clsArticuloSalida
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string codigo_articulo { get; set; }
        public string descripcion_general { get; set; }
        public string desc_familia { get; set; }
        public string desc_linea { get; set; }
        public string desc_grupo { get; set; }
        public string desc_medida { get; set; }
        public string desc_marca { get; set; }
        public string desc_parte { get; set; }
        public double noubicaciones { get; set; }
        public string Seccion { get; set; }
        public double Pasillo { get; set; }
        public string Estanteria { get; set; }
        public double Nivel { get; set; }
        public double Tarima { get; set; }
        public double Contenedor { get; set; }
        public double ExistenciaUbicacion { get; set; }
        public string UnidadControlUbicacion { get; set; }
        public double ExistenciaKardex { get; set; }
        public double cantidad { get; set; }
        public string desc_seccion { get; set; }
        public string desc_estanteria { get; set; }
        public string ccsucursal { get; set; }
        public string ccarea { get; set; }
        public string ccnivel1 { get; set; }
        public string ccnivel2 { get; set; }
        public string ccnivel3 { get; set; }
        public string ccnivel4 { get; set; }
        public Boolean ControlArea { get; set; }
        public Boolean Reasignado { get; set; }
        public string AreaAsignado { get; set; }
        public double CantidadAsignado { get; set; }
        public string ObservacionAsignado { get; set; }
        public int identrada { get; set; }
        public int consecutivo { get; set; }
        public string Empleado { get; set; }

        public override string ToString()
        {
            return string.Format("[clsArticuloSalida: ID={0}, codigo_articulo={1}, descripcion_general={2}, desc_familia={3}, desc_linea={4}, desc_grupo={5}, desc_medida={6}, " +
                                 "desc_marca={7}, desc_parte={8}, noubicaciones={9}, Seccion={10}, Pasillo={11}, Estanteria={12}, Nivel={13}, Tarima={14}, " +
                                 "Contenedor={15}, ExistenciaUbicacion={16}, UnidadControlUbicacion={17}, ExistenciaKardex={17}, cantidad={18}, " +
                                 "desc_seccion={19}, desc_estanteria={20}, ccsucursal={21}, ccarea={22}, ccnivel1={23}, ccnivel2={24}, ccnivel3={25}, ccnivel4={26}, " +
                                 "ControlArea={27}, Reasignado={28}, AreaAsignado={29}, CantidadAsignado={30}, ObservacionAsignado={31}, identrada={32}]",
                                 ID, codigo_articulo, descripcion_general, desc_familia, desc_linea, desc_grupo, desc_medida,
                                 desc_marca, desc_parte, noubicaciones, Seccion, Pasillo, Estanteria, Nivel, Tarima,
                                 Contenedor, ExistenciaUbicacion, UnidadControlUbicacion, ExistenciaKardex, cantidad, desc_seccion, desc_estanteria,
                                 ccsucursal, ccarea, ccnivel1, ccnivel2, ccnivel3, ccnivel4,
                                 ControlArea, Reasignado, AreaAsignado, CantidadAsignado, ObservacionAsignado, identrada);
        }
    }
}