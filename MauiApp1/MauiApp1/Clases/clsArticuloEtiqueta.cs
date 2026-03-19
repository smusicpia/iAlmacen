using SQLite;

namespace iAlmacen.Clases
{
    public class clsArticuloEtiqueta
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string codigo_articulo { get; set; }
        public string descripcion_general { get; set; }
        public string desc_marca { get; set; }
        public string desc_medida { get; set; }
        public string desc_parte { get; set; }
        public Boolean Si { get; set; }
        public Boolean No { get; set; }
        public Boolean Nunca { get; set; }
        public double Cantidad { get; set; }
        public Boolean ControlArea { get; set; }

        public override string ToString()
        {
            return string.Format("[clsArticuloEtiqueta: ID={0}, codigo_articulo={1}, descripcion_general={2}, desc_marca={3}, desc_medida={4}, desc_parte={5}, " +
                                 "Si={6}, No={7}, Nunca={8}, Cantidad={9}, ControlArea={10}]",
                                 ID, codigo_articulo, descripcion_general, desc_marca, desc_medida, desc_parte,
                                 Si, No, Nunca, Cantidad, ControlArea);
        }
    }
}