namespace iAlmacen.Models
{
    public class DocumentoAlmacenDetalle
    {
        public decimal id { get; set; }
        public string folio_documento { get; set; }
        public string serie_documento { get; set; }
        public string folio_entrada { get; set; }
        public DateTime fecha_documento { get; set; }
        public DateTime fecha_entrada { get; set; }
        public string codigo_proveedor { get; set; }
        public string tipo_documento { get; set; }
        public decimal taza_iva { get; set; }
        public bool taza_exento { get; set; }
        public string orden_compra { get; set; }
        public string area { get; set; }
        public string centro_costo { get; set; }
        public string referencia_documento { get; set; }
        public string codigo_articulo { get; set; }
        public decimal cantidad { get; set; }

        //public decimal precio_extranjero { get; set; }
        //public decimal precio_peso { get; set; }
        //public decimal procentaje_desc_extranjero { get; set; }
        //public decimal procentaje_desc_peso { get; set; }
        //public decimal importe_desc_extranjero { get; set; }
        //public decimal importe_desc_peso { get; set; }
        //public decimal iva_extrajero { get; set; }
        //public decimal iva_peso { get; set; }
        //public decimal importe_extranjero { get; set; }
        //public decimal importe_peso { get; set; }
        public decimal precio { get; set; }

        public decimal porcentaje_descuento { get; set; }
        public decimal importe_descuento { get; set; }
        public decimal iva { get; set; }
        public decimal importe { get; set; }
        public string tipo_articulo { get; set; }
        public string codigo_embalaje { get; set; } //medida
        public double tipo_cambio { get; set; }
        public decimal costo_anterior { get; set; }
        public decimal costo_actual { get; set; }
        public string iva_exento { get; set; }
        public string nota_credito { get; set; }
        public string folio_nota_credito { get; set; }
        public decimal cantidad_devolucion { get; set; }
        public decimal diferencia_precio { get; set; }
        public string status_documento { get; set; }
        public string psucursal { get; set; }
        public bool Aplica_Autorizacion { get; set; }
        public string Responsable_Autorizacion { get; set; }
        public decimal IEPS { get; set; }
        public bool SalidaVirtual { get; set; }
        public int FolioOrdenRecoleccion { get; set; }
        public bool Proyecto { get; set; }
        public string Seccion { get; set; }
        public int Pasillo { get; set; }
        public string Estanteria { get; set; }
        public int Nivel { get; set; }
        public int Tarima { get; set; }
        public int Contenedor { get; set; }
        public bool Control_Area { get; set; }
        public bool ImpresionControlArea { get; set; }
        public bool ControlAreaUsado { get; set; }
        public string cc_nivel2 { get; set; }
        public string cc_nivel3 { get; set; }
        public string cc_nivel4 { get; set; }
        public string folio_cotizacion { get; set; }
    }
}