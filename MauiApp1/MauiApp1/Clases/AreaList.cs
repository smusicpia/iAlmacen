namespace iAlmacen.Clases;

public class AreaList
{
    //[PrimaryKey]
    //public int UniqueId { get; set; }
    public string desc_area { get; set; }

    //public string Group { get; set; }

    public override string ToString()
    {
        return string.Format("{0}", desc_area);
    }
}