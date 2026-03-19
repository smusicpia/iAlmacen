using SQLite;

namespace iAlmacen.Clases;

public class clsInventario
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string Folio { get; set; }
    public string Fecha { get; set; }
    public double NoArticulos { get; set; }
    public double Capturados { get; set; }
    public double Restantes { get; set; }

    public override string ToString()
    {
        return string.Format("[clsInventario: ID={0}, Folio={1}, Fecha={2}, NoArticulos={3}, Capturados={4}, Restantes={5}]",
                             ID, Folio, Fecha, NoArticulos, Capturados, Restantes);
    }
}