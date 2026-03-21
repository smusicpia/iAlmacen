using iAlmacen.Clases;
using iAlmacen.WebApi;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.Inventario_Reposicion;

public partial class Item_inventario_reposicion : ContentPage
{
    private string filePath = string.Empty;
    public byte[] byteData;
    private MemoryStream ms = new MemoryStream();

    public Item_inventario_reposicion()
    {
        InitializeComponent();
        clave_actual.Text = clave_actual.Text + Global.item_select.codigo_articulo;
        clave_anterior.Text = clave_anterior.Text + Global.item_select.codigo_anterior;
        descripcion.Text = descripcion.Text + Global.item_select.descripcion_general;
        medida.Text = medida.Text + Global.item_select.desc_medida;
        marca.Text = marca.Text + Global.item_select.desc_marca;
        parte.Text = parte.Text + Global.item_select.desc_parte;
        unidad.Text = unidad.Text + Global.item_select.unidad;
        cantidad.Text = cantidad.Text + Global.item_select.cantidad;
        if (Global.item_select.cantidad_inventario > 0)
            cantidad_inventario.Text = Global.item_select.cantidad_inventario.ToString();

        //takePhoto.Clicked += async (sender, args) =>
        //{
        //    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        //    {
        //        return;
        //    }

        //    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
        //    {
        //        Directory = "Test",
        //        SaveToAlbum = true,
        //        CompressionQuality = 75,
        //        CustomPhotoSize = 50,
        //        PhotoSize = PhotoSize.Small,
        //        DefaultCamera = CameraDevice.Rear
        //    });

        //    if (file == null)
        //        return;

        //    filePath = file.Path;
        //    byteData = File.ReadAllBytes(filePath);
        //    image.Source = ImageSource.FromStream(() =>
        //    {
        //        var stream = file.GetStream();
        //        file.Dispose();
        //        return stream;
        //    });
        //};

        //pickPhoto.Clicked += async (sender, args) =>
        //{
        //    if (!CrossMedia.Current.IsPickPhotoSupported)
        //    {
        //        return;
        //    }
        //    var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
        //    {
        //        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
        //        MaxWidthHeight = 500,
        //    });

        //    if (file == null)
        //        return;

        //    filePath = file.Path;
        //    byteData = File.ReadAllBytes(filePath);
        //    image.Source = ImageSource.FromStream(() =>
        //    {
        //        var stream = file.GetStream();
        //        file.Dispose();
        //        return stream;
        //    });
        //};

        BindingContext = this;
    }

    private async void Handle_Clicked(object sender, EventArgs e)
    {
        if (cantidad_inventario.Text == null)
        {
            await DisplayAlertAsync("Advertencia", "Capture La Cantidad", "OK");
            return;
        }

        if (cantidad_inventario.Text == "")
        {
            await DisplayAlertAsync("Advertencia", "Capture La Cantidad", "OK");
            return;
        }

        if (byteData == null)
        {
            goto no_imagen;
        }

        Stream memoryStream = new MemoryStream(byteData);
        string Parametros = $"{Global.item_select.codigo_articulo}";
        HttpWebResponse response = ConfigAPI.PostAPI_Imagen("api/Firma", Parametros, "spset_foto_reposicion", memoryStream);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            await DisplayAlertAsync("Advertencia", "Error al cargar la Imagen", "OK");
            return;
        }

    no_imagen:
        Parametros = $"I,{Global.item_select.codigo_articulo},{float.Parse(cantidad_inventario.Text)},";
        response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_template_reposiciones");
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                await DisplayAlertAsync("Advertencia", "Error Al Actualizar", "OK");
                return;
            }
        }

        Global.item_return = true;
        Global.item_select.Color_ = "Teal";
        Global.item_select.cantidad_inventario = float.Parse(cantidad_inventario.Text);
        Global.item_select.texto_2 = "Cantidad Sistema: " + Global.item_select.cantidad + "   /   " + "Cantidad Inventario: " + cantidad_inventario.Text;
        await Navigation.PopAsync();
    }

    private async void takePhoto_Clicked(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        if (status == PermissionStatus.Granted)
        {
            var foto = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                CompressionQuality = 75,
                MaximumHeight = 500,
                MaximumWidth = 500,
                Title = "Tomar Foto"
            });
            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                image.Source = ImageSource.FromStream(() => stream);
            }
        }
        else
        {
            await DisplayAlertAsync("Informacion", $"El permiso fue denegado.", "OK");
        }
    }

    private async void pickPhoto_Clicked(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        if (status == PermissionStatus.Granted)
        {
            var foto = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                SelectionLimit = 1,
                CompressionQuality = 50,
                PreserveMetaData = false,
                Title = "Seleccionar Foto"
            });
            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                image.Source = ImageSource.FromStream(() => stream);
            }
        }
    }
}