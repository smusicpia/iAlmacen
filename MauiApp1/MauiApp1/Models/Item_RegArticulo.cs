namespace iAlmacen.Models;

public class Item_RegArticulo
{
    public int id { get; set; }
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
}