namespace iAlmacen.Models;

public class Item_orden_compra
{
    //public float id_ { get; set; }
    //public string captura_almacen_ { get; set; }
    //public string captura_ { get; set; }
    //public string vigilante_ { get; set; }
    //public string observacion_ { get; set; }
    //public string hora_ { get; set; }
    //public string fecha_ { get; set; }
    //public string proveedor_ { get; set; }
    //public string codigo_proovedor_ { get; set; }
    //public string folio_orden_ { get; set; }

    public float id_orden_ { get; set; }
    public string tipo_orden_ { get; set; }
    public string folio_orden_ { get; set; }
    public string clave_proveedor_ { get; set; }
    public string nombre_proveedor_ { get; set; }
    public string dirigido_ { get; set; }
    public DateTime fecha_orden_ { get; set; }
    public DateTime hora_orden_ { get; set; }
    public string moneda_ { get; set; }
    public double subtotal_orden_ { get; set; }
    public double iva_orden_ { get; set; }
    public double descuento_orden_ { get; set; }
    public double importe_descuento_ { get; set; }
    public double importe_gastos_ { get; set; }
    public double total_orden_ { get; set; }
    public double tipo_cambio_ { get; set; }
    public string comprador_ { get; set; }
    public string solicitante_general_ { get; set; }
    public string status_ { get; set; }
    public string folio_cotizacion_ { get; set; }
    public string surtido_ { get; set; }
    public string psucursal_ { get; set; }
    public float id_detalle_ { get; set; }
    public string solicitante_ { get; set; }
    public string area_ { get; set; }
    public string nivel1_ { get; set; }
    public string nivel2_ { get; set; }
    public string nivel3_ { get; set; }
    public string nivel4_ { get; set; }
    public double cantidad_ { get; set; }
    public string codigo_articulo_ { get; set; }
    public string concepto_ { get; set; }
    public string medida_ { get; set; }
    public string parte_ { get; set; }
    public string marca_ { get; set; }
    public string unidad_ { get; set; }
    public double precio_item_ { get; set; }
    public double descuento_item_ { get; set; }
    public double importe_descuento_item_ { get; set; }
    public double importe_item_ { get; set; }
    public string iva_item_ { get; set; }
    public double importe_iva_item_ { get; set; }
    public string consecutivo_ { get; set; }
    public double saldo_ { get; set; }
    public double saldo_original_ { get; set; }
    public string codigo_familia_ { get; set; }
    public string codigo_linea_ { get; set; }
    public string codigo_grupo_ { get; set; }
    public string observacion_ { get; set; }

    public string familia_ { get; set; }
    public string linea_ { get; set; }
    public string grupo_ { get; set; }
    public Boolean herramienta_ { get; set; }

    public string codigo_anterior_ { get; set; }

    //public double cantidad_captura_ { get; set; }
    //public double precio_captura_ { get; set; }
    //public double descuento_captura_ { get; set; }
    //public string iva_captura_ { get; set; }

    public double cantidad_original_ { get; set; }
    public double precio_original_ { get; set; }

    public double subtotal_calculo_ { get; set; }
    public double descuento_calculo_ { get; set; }
    public double iva_calculo_ { get; set; }
    public double total_calculo_ { get; set; }

    public Boolean autorizado_super_ { get; set; }
    public Boolean autorizado_admin_ { get; set; }

    public string autorizador_ { get; set; }

    public double nivel_autorizado_ { get; set; }
    public Boolean Aplica_Autorizacion_ { get; set; }

    //public string folio_entrada_ { get; set; }
    //public float total_items_ { get; set; }

    public string unidad_standar_ { get; set; }

    //false = articulo      true = activo
    public Boolean tipo_articulo_ { get; set; }

    public Boolean gasto_flete_ { get; set; }
    public double retencion_item_ { get; set; }

    public Boolean Ubicaciones { get; set; }
    public string uSeccion { get; set; }
    public string uPasillo { get; set; }
    public string uEstanteria { get; set; }
    public string uNivel { get; set; }
    public string uTarima { get; set; }
    public string uContenedor { get; set; }
    public Boolean uCapturado { get; set; }
    public Boolean Resico { get; set; }
    public Boolean ProcesoInventario { get; set; }
}