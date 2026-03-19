namespace iAlmacen.Clases;

public class ClsLinea
{
    public string desc_linea { get; set; }

    public override string ToString()
    {
        return string.Format("{0}", desc_linea);
    }
}