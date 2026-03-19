//using Plugin.Media.Abstractions;
//using Plugin.Media;
namespace iAlmacen.ViewModels
{
    public class OrdenViewModel : BaseViewModel_Orden
    {
        public Command LoadItemsCommand_orden { get; set; }

        //private MediaFile file;

        //private ImageSource imageSource;

        //public ImageSource ImageSource
        //{
        //    get { return this.imageSource; }
        //    set { imageSource = value; }
        //}

        public OrdenViewModel()
        {
            //this.ImageSource = "local:firma";

            //LoadItemsCommand_orden = new Command(() =>
            //{
            //    if (Clases.Global.cidsql_ != 0)
            //        buscar_datos();
            //    OnPropertyChanged(nameof(ImageSource));
            //});
        }

        //static byte[] Base64StringIntoImage(string str64)
        //{
        //    byte[] img = Convert.FromBase64String(str64);
        //    return img;
        //}

        //private void buscar_datos()
        //{
        //    string fromText = "OrdenRecoleccion AS [ordenR] inner join Detalle_OrdenRecoleccion dor ";
        //    fromText += "   On ordenR.id = dor.FolioOrdenRecoleccion and ordenR.FolioOrden = dor.FolioOrdenCompra left join RelacionFirmas rf ";
        //    fromText += "   On dor.Folio_DocumentoSalida = rf.folio ";

        //    string Parametros = "rf.firma";
        //    string Condicion = $"ordenR.id = {Clases.Global.cidsql_} and (dor.Seccion IS NOT NULL or dor.Pasillo IS NOT NULL or dor.Estanteria IS NOT NULL)";
        //    HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", fromText, Condicion, "SELECT");
        //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        //    {
        //        if (response.StatusCode == HttpStatusCode.NotFound) return;
        //        string resp = reader.ReadToEnd();
        //        if (resp == "[]") return;
        //        DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
        //        foreach (DataRow r in dt.Rows)
        //        {
        //            this.ImageSource = ImageSource.FromStream(() => new MemoryStream(Base64StringIntoImage(dt.Rows[0][36].ToString().Trim())));
        //        }
        //    }
        //}
    }
}