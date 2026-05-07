namespace iAlmacen.Models;

public class Item_InventarioDetalle
{
    public int id { get; set; }
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
}