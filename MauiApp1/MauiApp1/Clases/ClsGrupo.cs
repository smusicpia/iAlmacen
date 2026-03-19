namespace iAlmacen.Clases;

public class ClsGrupo
{
    public string desc_grupo { get; set; }

    public override string ToString()
    {
        return string.Format("{0}", desc_grupo);
    }
}