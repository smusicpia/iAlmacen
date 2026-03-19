namespace iAlmacen.Models;

public class Item_proterm
{
    public float id { get; set; }

    public string Color_ { get; set; }

    public string texto_1 { get; set; }
    public string texto_2 { get; set; }
    //public string Description_{ get; set; }
    //public float cantidad { get; set; }

    public string codigo_articulo { get; set; }
    public string descripcion_general { get; set; }
    public string desc_marca { get; set; }
    public string desc_medida { get; set; }
    public string desc_parte { get; set; }
    public float existencia { get; set; }
    public string unidad_existencia { get; set; }
    public float inventario_cantidad { get; set; }

    public string ifecha { get; set; }
    public string ihora { get; set; }
}