namespace iAlmacen.Models
{
    public class Item_ArticuloSalida
    {
        public int index { get; set; }
        public int id { get; set; }
        public string codigo_articulo { get; set; }
        public string descripcion_general { get; set; }
        public string desc_familia { get; set; }
        public string desc_linea { get; set; }
        public string desc_grupo { get; set; }
        public string desc_medida { get; set; }
        public string desc_marca { get; set; }
        public string desc_parte { get; set; }
        public double noubicaciones { get; set; }
        public string Seccion { get; set; }
        public int Pasillo { get; set; }
        public string Estanteria { get; set; }
        public int Nivel { get; set; }
        public int Tarima { get; set; }
        public int Contenedor { get; set; }
        public double ExistenciaUbicacion { get; set; }
        public string UnidadControlUbicacion { get; set; }
        public double ExistenciaKardex { get; set; }
        public double cantidad { get; set; }
        public string desc_seccion { get; set; }
        public string desc_estanteria { get; set; }
        public string sucursal { get; set; }
		public string area { get; set; }
		public string desc_area { get; set; }
		public string ccnivel1 { get; set; }
        public string desc_nivel1 { get; set; }
		public string ccnivel2 { get; set; }
		public string desc_nivel2 { get; set; }
		public string ccnivel3 { get; set; }
		public string desc_nivel3 { get; set; }
		public string ccnivel4 { get; set; }
		public string desc_nivel4 { get; set; }
		public Boolean ControlArea { get; set; }
        public Boolean Reasignado { get; set; }
        public bool AreaAsignado { get; set; }
        public double CantidadAsignado { get; set; }
        public string ObservacionAsignado { get; set; }
        public int identrada { get; set; }
        public int consecutivo { get; set; }
        public string Empleado { get; set; }
        public string Responsable { get; set; }
        public string nom_responsable { get; set; }
        public string Autorizador { get; set; }
        public string nom_autorizador { get; set; }
        public string FechaConfirmado { get; set; }
		public string Usuario { get; set; }
		public string Nombre_Usuario { get; set; }
		public string unidad { get; set; }
		public int FolioOrdenRecoleccion { get; set; }
		public string FolioOrdenCompra { get; set; }
		public string FolioPedido { get; set; }
		public string FolioRequisicion { get; set; }
		public string FolioCotizacion { get; set; }
		//public string solicitante { get; set; }
	}
}