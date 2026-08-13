namespace iAlmacen.Models
{
	public class SalidaAlmacenEntity
	{
		public int FolioOrdenRecoleccion { get; set; }
		public string FolioOrdenCompra { get; set; }
		public string FolioPedido { get; set; }
		public string FolioRequisicion { get; set; }
		public string FolioCotizacion { get; set; }
		public string solicitante { get; set; }
		public string area { get; set; }
		public string ccn1 { get; set; }
		public string ccn2 { get; set; }
		public string ccn3 { get; set; }
		public string ccn4 { get; set; }
		public double cantidad { get; set; }
		public string concepto { get; set; }
		public string unidad_medida { get; set; }
		public string unidad { get; set; }
		public string codigo_articulo { get; set; }
		public int consecutivo_mov { get; set; }
		public string status_requisicion { get; set; }
		public string numero_parte { get; set; }
		public string marca { get; set; }
		public string condicion_herramienta { get; set; }
		public string Seccion { get; set; }
		public int Pasillo { get; set; }
		public string Estanteria { get; set; }
		public int Nivel { get; set; }
		public int Tarima { get; set; }
		public int Contenedor { get; set; }
		public string TipoDocumento { get; set; }
		public string Folio_DocumentoSalida { get; set; }
		public string Codigo_Responsable { get; set; }
		public string Codigo_Autorizado { get; set; }
		public string Sucursal { get; set; }
		public string Usuario { get; set; }
		public string Nombre_Usuario { get; set; }
		public bool ControlArea { get; set; }
		public bool Reasignado { get; set; }
		public string AreaAsignado { get; set; }
		public double CantidadAsignado { get; set; }
		public string ObservacionAsignado { get; set; }
		public int IdEntrada { get; set; }
		public string Fecha { get; set; }
	}
}