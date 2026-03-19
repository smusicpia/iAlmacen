namespace iAlmacen.Models
{
    public class InventarioAlmacen
    {
        public int id { get; set; }
        public string FolioInventario { get; set; }
        public string CodigoArticulo { get; set; }
        public string FechaInventario { get; set; }
        public string HoraInventario { get; set; }
        public string UnidadControl { get; set; }
        public double ExistenciaSistema { get; set; }
        public double InventarioAlmacen_ { get; set; }
        public double InventarioContabilidad { get; set; }
        public double EntradasContabilidad { get; set; }
        public double SalidasContabilidad { get; set; }
        public bool Aplicado { get; set; }
        public string FechaAplicacion { get; set; }
        public string HoraAplicacion { get; set; }
        public bool Cancelado { get; set; }
        public bool Capturado { get; set; }
        public double Costo { get; set; }
        public double CostoCapturado { get; set; }
        public double Importe { get; set; }
        public string UsuarioResponsable { get; set; }
        public string ClaveResponsable { get; set; }
        public bool Cerrado { get; set; }
        public bool Duplicado { get; set; }
        public bool Muestreo { get; set; }
        public bool uso_herramienta { get; set; }
    }
}