namespace iAlmacen.Clases;

public class ClsFamilia
{
    public string desc_familia { get; set; }

    public override string ToString()
    {
        return string.Format("{0}", desc_familia);
    }
}