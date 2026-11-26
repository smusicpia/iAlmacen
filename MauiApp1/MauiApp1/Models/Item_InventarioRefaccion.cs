namespace iAlmacen.Models
{
	public class Item_InventarioRefaccion
	{
		public string articulo { get; set; }
		public double fisico { get; set; }
		public double existencia { get; set; }
		public string FechaCaptura { get; set; }
		public string fechaInventario { get; set; }
		public string Usuario { get; set; }
		public string Movimiento { get; set; }
	}
}