using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace iAlmacen.Views;

public partial class BarcodePage : ContentPage
{
    public event Action<byte[]> ImageStreamGenerated;

    public event Action<string> TextResultGenerated;

    private string _parentPageName;

    public string ParentPageName { get => _parentPageName; set => _parentPageName = value; }

    public BarcodePage()
    {
        InitializeComponent();

        barcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = true
        };
    }

    protected void barcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var first = e.Results?.FirstOrDefault();
        if (first is not null)
        {
            Dispatcher.Dispatch(async () =>
            {
                // Update BarcodeGeneratorView
                barcodeGenerator.ClearValue(BarcodeGeneratorView.ValueProperty);
                barcodeGenerator.Format = first.Format;
                barcodeGenerator.Value = first.Value;

                // Update Label
                ResultLabel.Text = $"Barcodes: {first.Format} -> {first.Value}";

                switch (ParentPageName)
                {
                    case "Lectura_Codigo":
                        await SendBarcodeTextAsync(first.Value);
                        break;

                    default:
                        await GenerateAndSaveBarcodeImageAsync(barcodeGenerator, "barcode.png");
                        break;
                }
            });
        }
    }

    private void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        barcodeReaderView.CameraLocation = barcodeReaderView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
    }

    private void TorchButton_Clicked(object sender, EventArgs e)
    {
        barcodeReaderView.IsTorchOn = !barcodeReaderView.IsTorchOn;
    }

    public async Task SendBarcodeTextAsync(string textConverterBarcode)
    {
        // Esperar 3 segundos
        await Task.Delay(1000);

        //// Invocar el evento con el stream de la imagen generada
        //ImageStreamGenerated?.Invoke(imageBytes);
        TextResultGenerated?.Invoke(textConverterBarcode);

        await Navigation.PopAsync();
    }

    public async Task GenerateAndSaveBarcodeImageAsync(BarcodeGeneratorView barcodeGeneratorView, string fileName)
    {
        // Renderizar el contenido visual en una imagen
        var imageBytes = await RenderBarcodeToImageAsync(barcodeGeneratorView);
        //var imageStream = ByteArrayToStream(imageBytes);

        //// Guardar la imagen en el almacenamiento local
        //await SaveBarcodeImageAsync(imageStream, fileName);
        // Esperar 3 segundos
        await Task.Delay(3000);

        //// Invocar el evento con el stream de la imagen generada
        ImageStreamGenerated?.Invoke(imageBytes);

        await Navigation.PopAsync();
    }

    public async Task<byte[]> RenderBarcodeToImageAsync(BarcodeGeneratorView barcodeGeneratorView)
    {
        // Renderizar el contenido visual del BarcodeGeneratorView en una imagen
        var screenshotResult = await barcodeGeneratorView.CaptureAsync();

        // Convertir la captura en un stream
        using var stream = await screenshotResult.OpenReadAsync();

        // Leer el stream en un array de bytes
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}