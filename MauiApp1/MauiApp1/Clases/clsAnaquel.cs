using SQLite;

namespace iAlmacen.Clases
{
    public class clsAnaquel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string codigo_pasillo { get; set; }
        public string codigo_anaquel { get; set; }
        public string descripcion_anaquel { get; set; }
        public string psucursal { get; set; }
        public string TipoAnaquel { get; set; }
        public int NumeroNiveles { get; set; }

        public override string ToString()
        {
            return string.Format("[clsAnaquel: ID={0}, codigo_pasillo={1}, codigo_anaquel={2}, " +
                                 "descripcion_anaquel={3}, psucursal={4}, TipoAnaquel={5}, NumeroNiveles={6}]",
                                 ID, codigo_pasillo, codigo_anaquel, descripcion_anaquel, psucursal, TipoAnaquel, NumeroNiveles);
        }
    }
}