using SQLite;

namespace iAlmacen.Clases
{
    public class clsArticuloEnResguardo
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

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

        public override string ToString()
        {
            return string.Format("[clsArticuloEnResguardo: ID={0}, folio={1}, codigo={2}, descripcion={3}, marca={4}, medida={5}, parte={6}, " +
            "unidadmedida={7}, cantidad={8}, serie={9}, inventario={10}, fechainventario={11}, condicion={12}, aplicado={13}, cerrado={14}] fechaentregado={15}]",
                                 ID, folio, codigo, descripcion, marca, medida, parte,
                                 unidadmedida, cantidad, serie, inventario, fechainventario, condicion, aplicado, cerrado, fechaentregado);
        }
    }
}