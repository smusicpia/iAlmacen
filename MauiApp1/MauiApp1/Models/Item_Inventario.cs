namespace iAlmacen.Models;

public class Item_Inventario
{
    public int id { get; set; }
    public string Folio { get; set; }
    public string Fecha { get; set; }
    public double NoArticulos { get; set; }
    public double Capturados { get; set; }
    public double Restantes { get; set; }
}