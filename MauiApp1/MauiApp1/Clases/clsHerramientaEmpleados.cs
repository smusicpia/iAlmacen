using SQLite;

namespace iAlmacen.Clases
{
    public class clsHerramientaEmpleados
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string area { get; set; }
        public string clave { get; set; }
        public string empleado { get; set; }
        public string fecha { get; set; }
        public int articulos { get; set; }

        public override string ToString()
        {
            return string.Format("[clsHerramientaEmpleados: ID={0}, area={1}, clave={2}, empleado={3}, fecha={4}, articulos={5}]",
                                 ID, area, clave, empleado, fecha, articulos);
        }
    }
}