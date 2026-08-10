using iAlmacen.Views;

using System.Windows.Input;

namespace iAlmacen.ViewModels;

public class LoginViewModel : BaseViewModel
{
	public bool AutoRefreshToken { get; set; }
	public Command LoginCommand { get; }
	public ICommand CheckCameraPermissionCommand { get; set; }
	public ICommand CheckPhotosPermissionCommand { get; set; }
	public ICommand CheckPhotosAddOnlyPermissionCommand { get; set; }
	public ICommand CheckConnectedCommand { get; set; }

	public LoginViewModel()
    {
        LoginCommand = new Command(OnLoginClicked);
		CheckCameraPermissionCommand = new Command(async () => await CheckAndRequestCameraPermission());
		CheckPhotosPermissionCommand = new Command(async () => await CheckAndRequestPhotosPermission());
		CheckPhotosAddOnlyPermissionCommand = new Command(async () => await CheckAndRequestPhotosAddOnlyPermission());
		CheckConnectedCommand = new Command(async () => await CheckAndRequestConection());
	}

	private async Task CheckAndRequestCameraPermission()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

		if (status == PermissionStatus.Granted)
			return;

		if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
		{
			// Prompt the user to turn on in settings
			// On iOS once a permission has been denied it may not be requested again from the application
			return;
		}

		if (Permissions.ShouldShowRationale<Permissions.Camera>())
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		status = await Permissions.RequestAsync<Permissions.Camera>();

		if (status != PermissionStatus.Granted)
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		return;
	}

	private async Task CheckAndRequestPhotosPermission()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Photos>();

		if (status == PermissionStatus.Granted)
			return;

		if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
		{
			// Prompt the user to turn on in settings
			// On iOS once a permission has been denied it may not be requested again from the application
			return;
		}

		if (Permissions.ShouldShowRationale<Permissions.Photos>())
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		status = await Permissions.RequestAsync<Permissions.Photos>();

		if (status != PermissionStatus.Granted)
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		return;
	}

	private async Task CheckAndRequestPhotosAddOnlyPermission()
	{
		PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PhotosAddOnly>();

		if (status == PermissionStatus.Granted)
			return;

		if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
		{
			// Prompt the user to turn on in settings
			// On iOS once a permission has been denied it may not be requested again from the application
			return;
		}

		if (Permissions.ShouldShowRationale<Permissions.PhotosAddOnly>())
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		status = await Permissions.RequestAsync<Permissions.PhotosAddOnly>();

		if (status != PermissionStatus.Granted)
		{
			await Shell.Current.DisplayAlertAsync("Permiso requerido", "Esta aplicación necesita acceso a la cámara para funcionar correctamente.", "OK");
		}

		return;
	}

	private async Task CheckAndRequestConection()
	{
		foreach (var item in Connectivity.Current.ConnectionProfiles)
		{
			if (item == ConnectionProfile.WiFi || item == ConnectionProfile.Cellular)
			{
				return;
			}
		}
		return;
	}

	private async void OnLoginClicked(object obj)
    {
        // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
        await Shell.Current.GoToAsync($"//{nameof(LoginView)}");
    }
}