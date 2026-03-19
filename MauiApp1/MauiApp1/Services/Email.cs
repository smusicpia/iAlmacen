namespace iAlmacen.Services;

public class EmailSender : ContentPage
{
    public async Task SendEmail(string subject, string body, List<string> recipients, string Adjunto)
    {
        try
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients
            };
            message.BodyFormat = EmailBodyFormat.Html;

            if (!String.IsNullOrEmpty(Adjunto))
            {
                var fn = "Attachment.txt";
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                File.WriteAllText(file, "Hello World");

                message.Attachments.Add(new EmailAttachment(file));
            }

            await Email.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException fbsEx)
        {
            await DisplayAlertAsync("Error Orden de Recoleccion" + Clases.Global.cidsql_, fbsEx.ToString(), "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Advertencia", "Folio Incorrecto", "OK");
        }
    }
}