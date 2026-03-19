using SQLite;

namespace iAlmacen.Clases
{
    public class clsSeccion
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public Boolean Pasillos { get; set; }
        public int NumeroPasillos { get; set; }
        public Boolean Estanterias { get; set; }
        public string Sucursal { get; set; }

        public override string ToString()
        {
            return string.Format("[clsSeccion: ID={0}, Clave={1}, Descripcion={2}, Pasillos={3}, NumeroPasillos={4}, Estanterias={5}, Sucursal={6}]",
                                 ID, Clave, Descripcion, Pasillos, NumeroPasillos, Estanterias, Sucursal);
        }
    }
}