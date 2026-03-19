namespace iAlmacen.Models
{
    public class Item
    {
        public float id { get; set; }

        public string Color_ { get; set; }

        public string texto_1 { get; set; }
        public string texto_2 { get; set; }

        //public string Description_{ get; set; }
        //public float cantidad { get; set; }
        public string codigo_articulo { get; set; }

        public string codigo_anterior { get; set; }
        public string descripcion_general { get; set; }
        public string desc_marca { get; set; }
        public string desc_medida { get; set; }
        public string desc_parte { get; set; }
        public float cantidad { get; set; }
        public float cantidad_sistema { get; set; }
        public string unidad { get; set; }
        public float mes { get; set; }
        public float anio { get; set; }
        public float cantidad_inventario { get; set; }
    }
}