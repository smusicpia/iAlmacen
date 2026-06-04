namespace iAlmacen.Models
{
    public class Item_ArticuloSalida
    {
        public int index { get; set; }
        public int id { get; set; }
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
    }
}
