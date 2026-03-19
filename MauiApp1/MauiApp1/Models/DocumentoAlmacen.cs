namespace iAlmacen.Models
{
    public class DocumentoAlmacen
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

        //public decimal iva_extranjero { get; set; }
        //public decimal iva_peso { get; set; }
        //public decimal subtotal_extranjero { get; set; }
        //public decimal subtotal_peso { get; set; }
        //public decimal flete_extranjero { get; set; }
        //public decimal flete_peso { get; set; }
        //public decimal ret_flete_extranjero { get; set; }
        //public decimal ret_flete_peso { get; set; }
        //public decimal gastos_extranjero { get; set; }
        //public decimal gastos_peso { get; set; }
        //public decimal total_extranjero { get; set; }
        //public decimal total_peso { get; set; }
        public decimal iva { get; set; }

        public decimal subtotal { get; set; }
        public decimal flete { get; set; }
        public decimal ret_flete { get; set; }
        public decimal ret_iva_peso { get; set; }
        public decimal ret_iva_extranjero { get; set; }
        public decimal gastos { get; set; }
        public decimal total { get; set; }
        public DateTime fecha_vencimiento { get; set; }
        public decimal bonificacion_documento_ext { get; set; }
        public decimal bonificacion_documento { get; set; }
        public decimal bonificacion_documento_otro_ext { get; set; }
        public decimal bonificacion_documento_otro { get; set; }
        public decimal anticipo_documento { get; set; }
        public decimal saldo_documento { get; set; }
        public decimal saldo_extranjero { get; set; }
        public string orden_compra { get; set; }
        public decimal tipo_cambio { get; set; }
        public string credito_contado { get; set; }
        public string nacional_importacion { get; set; }
        public decimal ganancia_cambiaria { get; set; }
        public decimal perdida_cambiaria { get; set; }
        public string centro_costo { get; set; }
        public string referencia_documento { get; set; }
        public string concepto_gasto { get; set; }
        public string tipo_moneda { get; set; }
        public bool pagado { get; set; }
        public string tipo_descuento { get; set; }
        public decimal descuento { get; set; }

        //public decimal importe_descuento_general_pesos { get; set; }
        //public decimal importe_descuento_general_ext { get; set; }
        public decimal importe_descuento_general { get; set; }

        public TimeSpan hora_entrada { get; set; }
        public string usuario { get; set; }
        public bool nota_credito { get; set; }
        public string folio_nota_credito { get; set; }
        public string status_documento { get; set; }
        public DateTime fecha_cancelacion { get; set; }
        public string observacion { get; set; }
        public string psucursal { get; set; }
        public string concepto_cancelacion { get; set; }
        public DateTime fecha_captura_nota_credito { get; set; }
        public DateTime fecha_nota_credito { get; set; }
        public bool captura_movil { get; set; }
        public string nombre_dispositivo { get; set; }
        public DateTime actualizado { get; set; }
        public bool impreso { get; set; }
        public string tipo_nota_credito { get; set; }
        public string folio_docto_vinculado { get; set; }
        public string serie_docto_vinculado { get; set; }
        public DateTime fecha_docto_vinculado { get; set; }
        public string SistemaCaptura { get; set; }
        public decimal ImporteIEPS { get; set; }
        public bool Resico { get; set; }
        public decimal FactorResico { get; set; }
        public decimal ImporteResico { get; set; }
    }
}