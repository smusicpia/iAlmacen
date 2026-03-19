namespace iAlmacen.Clases;

public class CatalogoArticuloNumeroSeries
{
    public int id { get; set; }
    public string Codigo_Articulo { get; set; }
    public int Cantidad { get; set; }
    public string Numero_Serie { get; set; }
    public string Status_Herramienta { get; set; }
    public string Folio_Captura { get; set; }
    public string Numero_Serie_Anterior { get; set; }
    public bool Aplica { get; set; }
    public string Psucursal { get; set; }
    public string Seccion { get; set; }
    public int? Pasillo { get; set; }
    public string Estanteria { get; set; }
    public int? Nivel { get; set; }
    public int? Tarima { get; set; }
    public int? Contenedor { get; set; }
    public string Orden_Compra { get; set; }
    public string Folio_Documento { get; set; }
    public string Serie_Documento { get; set; }
    public string Folio_Entrada { get; set; }
    public string FolioInventario { get; set; }
}