namespace iAlmacen.Models
{
    public class Item_ArticuloEnResguardo
    {
        public int id { get; set; }
        public string folio { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string marca { get; set; }
        public string medida { get; set; }
        public string parte { get; set; }
        public string unidadmedida { get; set; }
        public int cantidad { get; set; }
        public string serie { get; set; }
        public Boolean inventario { get; set; }
        public string fechainventario { get; set; }
        public string condicion { get; set; }
        public Boolean aplicado { get; set; }
        public Boolean cerrado { get; set; }
        public string fechaentregado { get; set; }
    }
}