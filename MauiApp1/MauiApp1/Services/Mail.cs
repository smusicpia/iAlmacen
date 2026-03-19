using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace iAlmacen.Services;

public class Mail
{
    public bool Smtp { get; set; }
    public string servidor_smtp { get; set; }
    public string puerto_smtp { get; set; }
    public string CEmailFrom, CContraseña_CORREO, CEmailTo, CContenido;

    public Mail()
    {
        Get_Servidor_Correos();
    }

    public void Get_Servidor_Correos()
    {
        string Parametros = "*";
        string Condicion = $"1=1";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "Configuracion_Servidor_Correos", Condicion, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            foreach (DataRow r in dt.Rows)
            {
                Smtp = true;
                puerto_smtp = r[1].ToString().Trim();
                servidor_smtp = r[2].ToString().Trim();
            }
        }
    }

    public string GenerarTablaHTML(DataTable dt)
    {
        if (dt.Rows.Count <= 0)
            return "";
        string table = "<table style='border: 3px #707070 solid; width: 100%' border='3'>";
        string thead = "<thead><tr><th colspan=\"6\" align=\"center\">DETALLE ORDEN DE COMPRA</th></tr>";
        thead += "<tr>";
        thead += "<th style=\"width: 8%;\">CANT.</th>";
        thead += "<th style=\"width: 10%;\">UNIDAD</th>";
        thead += "<th style=\"width: 17%\">NO. PARTE</th>";
        thead += "<th style=\"width: 12%\">MARCA</th>";
        thead += "<th style=\"width: 12%\">ART&Iacute;CULO</th>";
        thead += "<th>DESCRIPC&Oacute;N</th></tr></thead>";

        string tbody = "<tbody>";
        string tr = "";
        string td = "";
        string filas = "";
        foreach (DataRow row in dt.Rows)
        {
            tr = "<tr>";
            td = $"<td align=\"center\">{row[0].ToString().Trim()}</td>";
            td += $"<td align=\"center\">{row[1].ToString().Trim()}</td>";
            td += $"<td align=\"center\">{row[2].ToString().Trim()}</td>";
            td += $"<td align=\"center\">{row[3].ToString().Trim()}</td>";
            td += $"<td align=\"center\">{row[4].ToString().Trim()}</td>";
            td += $"<td>{row[5].ToString().Trim()}</td>";
            filas += tr + td + "</tr>";
        }
        string closetable = "</tbody></table><br>";
        return table + thead + tbody + filas + closetable;
    }

    public DataTable ObtenerDetalleOrdenRecoleccion(string OrdenCompra, string Requisicion, int ordenRecoleccion, string Cotizacion)
    {
        DataTable dtDetalleOrdenCompra = new DataTable();
        string strSelect = "cantidad, unidad_medida, numero_parte, marca, cve_articulo, concepto";
        string strWhere = $"FolioOrdenRecoleccion = {ordenRecoleccion} and FolioOrdenCompra = '{OrdenCompra}' and FolioRequisicion = '{Requisicion}' and FolioCotizacion = '{Cotizacion}' and (Seccion IS NOT NULL or Pasillo IS NOT NULL or Estanteria IS NOT NULL)";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", strSelect, "wsp_execute_qwerty", "Detalle_OrdenRecoleccion", strWhere, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return dtDetalleOrdenCompra;
            string resp = reader.ReadToEnd();
            if (resp == "[]") { return dtDetalleOrdenCompra; }
            dtDetalleOrdenCompra = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
        }
        return dtDetalleOrdenCompra;
    }

    public void Armar_Correos_OrdenCompra(string PrestadorServicio, string DirArchivo_Adjunto, string Asunto, string Entrada, string Prov, string FRequisicion, string FOrdenCompra, int OrdenRecoleccion, string FCotizacion)
    {
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        if (ConfigAPI.TipoProyecto == "D") ConfigAPI.Prueba = true;
        else ConfigAPI.Prueba = false;
        string AsuntoCorreo = "Orden de Compra " + FOrdenCompra + Asunto + ". ";
        DataTable Dt_CorreoUsuarioTo = new DataTable();
        string strSelect = "ltrim(rtrim(correo_Notificaciones)) As correo";
        string strWhere = $"nombre = '{PrestadorServicio}'";
        HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", strSelect, "wsp_execute_qwerty", "usuarios_exchange", strWhere, "SELECT");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode == HttpStatusCode.NotFound) return;
            string resp = reader.ReadToEnd();
            if (resp == "[]") return;
            Dt_CorreoUsuarioTo = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
            //Modificar cuando se mande a produccion, cambiar datos predefinidos
            CEmailFrom = "noreply@mapsayuc.com"; // Dt_CorreoUsuarioFrom.Rows(0).Item("password_correo").ToString.Trim
            CContraseña_CORREO = "l13mOG4u3dk6"; // Dt_CorreoUsuarioFrom.Rows(0).Item("password_correo").ToString.Trim
            if (ConfigAPI.TipoProyecto == "P")
                CEmailTo = Dt_CorreoUsuarioTo.Rows[0]["correo"].ToString().Trim();
            else
                CEmailTo = "s_musicpia@yahoo.com.mx";
        }

        List<string> correos = new List<string>();
        correos.Add(CEmailTo);
        correos.Add("shernandez@mapsayuc.com");
        if ((PrestadorServicio != ""))
        {
            string Mensaje_Correo = "<html><body><table height=200 border=0 width=600>" +
                                    "<tr><td><img src ='https://redmaq.mx/images/micrositio/redmaq-ddf12f45.png' sizes=\"auto\"></td>" +
                                    "<td><font size=4><b>ALMACEN</b></font></td></tr>" +
                                    $"<tr><td colspan='2'><font size=2><b>{Entrada} {OrdenRecoleccion} </b><BR>" +
                                    $"REQUISICI&Oacute;N: <b>{FRequisicion}</b><BR>" +
                                    $"ORDEN COMPRA: <b>{FOrdenCompra}</b><BR></font>" +
                                    $"COTIZACI&Oacute;N: <b>{FCotizacion}</b><BR></font>" +
                                    "_____________________________________________________________________________________________________<BR><BR>" +
                                    $"{GenerarTablaHTML(ObtenerDetalleOrdenRecoleccion(FOrdenCompra, FRequisicion, OrdenRecoleccion, FCotizacion))}" +
                                    "Atte.<BR><BR>" + "Departamento Almacen <br> " +
                                    "Materiales Anillo Perif&eacute;rico SA de CV.</td></tr></table></body>" +
                                    "<footer><p>_____________________________________________________________________________________________________</p>" +
                                    "<table height=auto border=0 width=800><tr><td align=\"center\">" +
                                    "<font size=1><b>" +
                                    "La información transmitida está destinada únicamente a la persona a quien va dirigida. Si usted recibe este mensaje por error, " +
                                    "favor de contactar al remitente y eliminar el material de cualquier computadora. " +
                                    "Este correo electrónico fue enviado desde una dirección solamente de notificaciones que no puede aceptar correo electrónico entrante. Por favor no respondas a este mensaje." +
                                    "</b></font></td></tr></table></footer>" +
                                    "</html>";

            ////Mandar_Correos(CEmailFrom, CContraseña_CORREO, DirArchivo_Adjunto, AsuntoCorreo, CEmailTo, Mensaje_Correo);
            EmailSender sm = new EmailSender();
            var respuestaEnvioCorreo = sm.SendEmail(AsuntoCorreo, Mensaje_Correo, correos, DirArchivo_Adjunto);
        }
    }

    public Boolean Mandar_Correos(string EmailFrom, string PassFrom, string DirArchivo_Adjunto, string Asunto, string EmailTo, string Mensaje_Correo)
    {
        System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        bool resultado = false;

        if ((EmailFrom != ""))
        {
            // Declaro la variable para enviar el correo
            System.Net.Mail.MailMessage ObCorreo = new System.Net.Mail.MailMessage();
            ObCorreo.From = new System.Net.Mail.MailAddress(EmailFrom);

            ObCorreo.Subject = Asunto;
            ObCorreo.To.Add(EmailTo);
            ObCorreo.IsBodyHtml = true; // PARA MANDAR correo con tablas

            ObCorreo.Body = Mensaje_Correo;
            if (DirArchivo_Adjunto != "")
            {
                System.Net.Mail.Attachment archivo = new System.Net.Mail.Attachment(DirArchivo_Adjunto);
                ObCorreo.Attachments.Add(archivo);
            }

            // Configuracion del servidor
            System.Net.Mail.SmtpClient Servidor = new System.Net.Mail.SmtpClient("netsol-smtp-oxcs.hostingplatform.com");
            Servidor.Host = servidor_smtp;
            Servidor.Port = Int16.Parse(puerto_smtp);
            Servidor.EnableSsl = true;
            Servidor.Credentials = new System.Net.NetworkCredential(EmailFrom, PassFrom);
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            try
            {
                Servidor.Send(ObCorreo);
                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
            }

            Servidor.Dispose();
        }

        return resultado;
    }
}