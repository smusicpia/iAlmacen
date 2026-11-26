using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;

using Controls.UserDialogs.Maui;

using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using iAlmacen.WebApi;

using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace iAlmacen.Almacen_Refacciones.Salida_Almacen;

public partial class Almacen_Salidas : ContentPage
{
	private Point[] points;
	private bool capturando = true;
	public List<string> lAreas { get; set; }
	public ObservableCollection<clsArticuloSalida> ArticulosEliminados { get; set; }
	private bool cArea = false;
	private bool ccn1 = false;
	private bool ccn2 = false;
	private bool ccn3 = false;
	private bool ccn4 = false;
	public List<string> lCentroCostoN1 { get; set; }
	public List<string> lCentroCostoN2 { get; set; }
	public List<string> lCentroCostoN3 { get; set; }
	public List<string> lCentroCostoN4 { get; set; }
	public List<string> lResponsable { get; set; }
	public List<string> lAutorizado { get; set; }
	public bool OrdenRecoleccion = false;
	public string Origen = string.Empty;

	public ObservableCollection<Item_ArticuloSalida> Items { get; set; }
	public Command LoadItemsCommand_Articulo { get; set; }
	private ItemsViewModel_ArticuloSalida viewModel_ArticuloSalida;

	private bool _isNavigated = false;

	public Almacen_Salidas()
	{
		InitializeComponent();
		NavigationPage.SetBackButtonTitle(this, "Atras");
		ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();

		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_Articulo = new Command(async () => await cargar());

		LimpiarCampos();
		Global.strSucursal = "";
		Global.strArea = "";
		Global.strCCnivel1 = "";
		Global.strCCnivel2 = "";
		Global.strCCnivel3 = "";
		Global.strCCnivel4 = "";

		cbSucursal.SelectedItem = "MÉRIDA";
		cbSucursal.IsEnabled = true;
		cbArea.IsEnabled = true;
		cbNivel1.IsEnabled = true;
		cbNivel2.IsEnabled = true;
		cbNivel3.IsEnabled = true;
		cbNivel4.IsEnabled = true;
		btnRefacciones.IsEnabled = true;
		btnGenerarSalida.IsEnabled = true;
		dtFechaSalida.Date = DateTime.Now;
		Item_ArticuloSalida item_ArticuloSalida = new Item_ArticuloSalida();
		BindingContext = viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(item_ArticuloSalida);

		buscar_datos(false);
	}

	/// <summary>
	/// Cuando se genera una salida de refacciones, sin orden de recoleccion, se le pasa un objeto vacio para cargar la informacion del articulo a agregar en la salida.
	/// </summary>
	/// <param name="item_ArticuloSalida"></param>
	public Almacen_Salidas(Item_ArticuloSalida item_ArticuloSalida)
	{
		InitializeComponent();
		NavigationPage.SetBackButtonTitle(this, "Atras");
		ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();

		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_Articulo = new Command(async () => await cargar());

		LimpiarCampos();
		Global.strSucursal = "";
		Global.strArea = "";
		Global.strCCnivel1 = "";
		Global.strCCnivel2 = "";
		Global.strCCnivel3 = "";
		Global.strCCnivel4 = "";

		LlenarArea();
		LlenarCentroCostoN1();
		LlenarCentroCostoN2();
		LlenarCentroCostoN3();
		LlenarCentroCostoN4();
		LlenarResponsables();

		cbSucursal.SelectedItem = "MÉRIDA";
		cbSucursal.IsEnabled = true;
		cbArea.IsEnabled = true;
		cbNivel1.IsEnabled = true;
		cbNivel2.IsEnabled = true;
		cbNivel3.IsEnabled = true;
		cbNivel4.IsEnabled = true;
		btnRefacciones.IsEnabled = true;
		btnGenerarSalida.IsEnabled = true;
		dtFechaSalida.Date = DateTime.Now;

		BindingContext = viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(item_ArticuloSalida);
	}

	/// <summary>
	/// Cuando Viene de una Orden de Recoleccion y se le pasa un Folio de Orden.
	/// </summary>
	/// <param name="Orden_Lista"></param>
	public Almacen_Salidas(bool Orden_Lista)
	{
		InitializeComponent();
		NavigationPage.SetBackButtonTitle(this, "Atras");
		OrdenRecoleccion = Orden_Lista;
		this.Title = $"Orden de Compra ({Global.folio_orden_}) / Recoleccion ({Global.cidsql_})";

		ArticulosEliminados = new ObservableCollection<clsArticuloSalida>();

		Items = new ObservableCollection<Item_ArticuloSalida>();
		LoadItemsCommand_Articulo = new Command(async () => await cargar());

		Global.strSucursal = "";
		Global.strArea = "";
		Global.strCCnivel1 = "";
		Global.strCCnivel2 = "";
		Global.strCCnivel3 = "";
		Global.strCCnivel4 = "";
		cbSucursal.IsEnabled = false;
		cbArea.IsEnabled = false;
		cbNivel1.IsEnabled = false;
		cbNivel2.IsEnabled = false;
		cbNivel3.IsEnabled = false;
		cbNivel4.IsEnabled = false;
		btnRefacciones.IsEnabled = false;
		btnGenerarSalida.IsEnabled = true;
		dtFechaSalida.Date = DateTime.Now;
		BindingContext = viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(Global.cidsql_, "RL");
		viewModel_ArticuloSalida.LoadItemsCommand_ArticuloSalida.Execute(null);

		buscar_datos();
	}

	/// <summary>
	/// Cuando se desea visualizar el Historico de Ordenes de Recoleccion
	/// </summary>
	/// <param name="Orden_Lista"></param>
	/// <param name="historico"></param>
	public Almacen_Salidas(bool Orden_Lista, Boolean historico)
	{
		InitializeComponent();
		NavigationPage.SetBackButtonTitle(this, "Atras");
		this.Title = $"Orden de Compra ({Global.folio_orden_}) / Recoleccion ({Global.cidsql_})";
		Origen = "H";
		OrdenRecoleccion = Orden_Lista;
		Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
		Global.strSucursal = "";
		Global.strArea = "";
		Global.strCCnivel1 = "";
		Global.strCCnivel2 = "";
		Global.strCCnivel3 = "";
		Global.strCCnivel4 = "";
		cbSucursal.IsEnabled = false;
		cbArea.IsEnabled = false;
		cbNivel1.IsEnabled = false;
		cbNivel2.IsEnabled = false;
		cbNivel3.IsEnabled = false;
		cbNivel4.IsEnabled = false;
		cbResonsable.IsEnabled = false;
		cbAutorizado.IsEnabled = false;
		btnRefacciones.IsEnabled = false;
		btnGenerarSalida.IsEnabled = false;

		spvFirma.IsEnabled = false;
		spvFirma.IsVisible = false;
		signatureSample.IsEnabled = false;
		signatureSample.IsVisible = false;

		imgFirma.IsVisible = true;
		srcFirma.IsVisible = true;

		dtFechaSalida.IsEnabled = false;
		BindingContext = viewModel_ArticuloSalida = new ItemsViewModel_ArticuloSalida(Global.cidsql_, "H");
		viewModel_ArticuloSalida.LoadItemsCommand_ArticuloSalida.Execute(null);

		buscar_datos(historico);
	}

	private async Task cargar()
	{ }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (_isNavigated) return;

		_isNavigated = true;

		if (Global.validar == true)
		{
			Global.validar = false;
		}
		else
		{
			Global.validar = false;
		}
	}

	private async Task LlenarArea()
	{
		lAreas = new List<String>();
		lAreas.Add("");
		DataTable? dt = await Funciones.LlenarArea();
		foreach (DataRow r in dt.Rows)
		{
			lAreas.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbArea.ItemsSource = lAreas;
	}

	private async Task LlenarCentroCostoN1()
	{
		lCentroCostoN1 = new List<String>();
		lCentroCostoN1.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN1();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN1.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel1.ItemsSource = lCentroCostoN1;
	}

	private async Task LlenarCentroCostoN2()
	{
		lCentroCostoN2 = new List<String>();
		lCentroCostoN2.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN2();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN2.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel2.ItemsSource = lCentroCostoN2;
	}

	private async Task LlenarCentroCostoN3()
	{
		lCentroCostoN3 = new List<String>();
		lCentroCostoN3.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN3();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN3.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel3.ItemsSource = lCentroCostoN3;
	}

	private async Task LlenarCentroCostoN4()
	{
		lCentroCostoN4 = new List<String>();
		lCentroCostoN4.Add("");
		DataTable? dt = await Funciones.LlenarCentroCostoN4();
		foreach (DataRow r in dt.Rows)
		{
			lCentroCostoN4.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbNivel4.ItemsSource = lCentroCostoN4;
	}

	private async Task LlenarResponsables()
	{
		lResponsable = new List<String>();
		lResponsable.Add("");
		DataTable? dt = await Funciones.LlenarResponsables();
		foreach (DataRow r in dt.Rows)
		{
			lResponsable.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbResonsable.ItemsSource = lResponsable;
	}

	private async Task LlenarAutorizador()
	{
		lAutorizado = new List<String>();
		lAutorizado.Add("");
		DataTable? dt = await Funciones.LlenarAutorizado();
		foreach (DataRow r in dt.Rows)
		{
			lAutorizado.Add(r[0].ToString().Trim() + " - " + r[1].ToString().Trim());
		}
		cbAutorizado.ItemsSource = lAutorizado;
	}

	private async void LimpiarCampos()
	{
		Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
		ArticulosEliminados.Clear();
		cbArea.SelectedIndex = 0;
		capturando = false;

		Global.strSucursal = "";
		Global.strArea = "";
		Global.strCCnivel1 = "";
		Global.strCCnivel2 = "";
		Global.strCCnivel3 = "";
		Global.strCCnivel4 = "";

		await LlenarArea();
		await LlenarCentroCostoN1();
		await LlenarCentroCostoN2();
		await LlenarCentroCostoN3();
		await LlenarCentroCostoN4();
		await LlenarResponsables();
		capturando = true;

		Global.Editando = false;
		Global.cidsql_ = 0;
		Global.folio_orden_ = string.Empty;
		Global.folio_requisicion_ = string.Empty;
		Global.folio_cotizacion_ = string.Empty;
		OrdenRecoleccion = false;
		cbSucursal.SelectedIndex = -1;
		cbResonsable.SelectedIndex = -1;
		cbAutorizado.SelectedIndex = -1;
		signatureSample.Clear();
	}

	private async void buscar_datos()
	{
		capturando = false;
		await LlenarArea();
		await LlenarCentroCostoN1();
		await LlenarCentroCostoN2();
		await LlenarCentroCostoN3();
		await LlenarCentroCostoN4();
		await LlenarResponsables();

		foreach (Item_ArticuloSalida item in viewModel_ArticuloSalida.Items)
		{
			cbArea.SelectedItem = item.area.ToString().Trim() + " - " + item.desc_area.ToString().Trim();
			if (cbNivel1.ItemsSource.Count <= 1)
				await LlenarCentroCostoN1();
			cbNivel1.SelectedItem = item.ccnivel1.ToString().Trim() + " - " + item.desc_nivel1.ToString().Trim();
			cbNivel2.SelectedItem = item.ccnivel2.ToString().Trim() + " - " + item.desc_nivel2.ToString().Trim();
			if (!string.IsNullOrEmpty(item.ccnivel3))
			{
				cbNivel3.SelectedItem = item.ccnivel3.ToString().Trim() + " - " + item.desc_nivel3.ToString().Trim();
			}
			if (!string.IsNullOrEmpty(item.ccnivel4))
			{
				cbNivel4.SelectedItem = item.ccnivel4.ToString().Trim() + " - " + item.desc_nivel4.ToString().Trim();
			}

			switch (item.sucursal.ToString().Trim())
			{
				case "M":
					cbSucursal.SelectedItem = "MÉRIDA";
					Global.strSucursal = "M";
					break;

				case "T":
					cbSucursal.SelectedItem = "TEBEC";
					Global.strSucursal = "T";
					break;
			}
			cbResonsable.SelectedItem = item.Responsable + " - " + item.nom_responsable;
			cbAutorizado.SelectedItem = item.Autorizador + " - " + item.nom_autorizador;
			if (!string.IsNullOrEmpty(item.FechaConfirmado))
				dtFechaSalida.Date = DateTime.Parse(item.FechaConfirmado);
		}

		capturando = true;
	}

	public static void SaveByte(string fileName, byte[] data)
	{
		var filePath = "local:firma.jpg";
		if (!File.Exists(filePath))
			File.Delete(filePath);
		File.WriteAllBytes(filePath, data);
	}

	private async void buscar_datos(bool Historico)
	{
		capturando = false;
		await LlenarArea();
		await LlenarCentroCostoN1();
		await LlenarCentroCostoN2();
		await LlenarCentroCostoN3();
		await LlenarCentroCostoN4();
		await LlenarResponsables();

		if (Historico)
		{
			foreach (Item_ArticuloSalida item in viewModel_ArticuloSalida.Items)
			{
				cbArea.SelectedItem = item.area.ToString().Trim() + " - " + item.desc_area.ToString().Trim();
				cbNivel1.SelectedItem = item.ccnivel1.ToString().Trim() + " - " + item.desc_nivel1.ToString().Trim();
				cbNivel2.SelectedItem = item.ccnivel2.ToString().Trim() + " - " + item.desc_nivel2.ToString().Trim();
				if (!string.IsNullOrEmpty(item.ccnivel3))
				{
					cbNivel3.SelectedItem = item.ccnivel3.ToString().Trim() + " - " + item.desc_nivel3.ToString().Trim();
				}
				if (!string.IsNullOrEmpty(item.ccnivel4))
				{
					cbNivel4.SelectedItem = item.ccnivel4.ToString().Trim() + " - " + item.desc_nivel4.ToString().Trim();
				}

				switch (item.sucursal.ToString().Trim())
				{
					case "M":
						cbSucursal.SelectedItem = "MÉRIDA";
						Global.strSucursal = "M";
						break;

					case "T":
						cbSucursal.SelectedItem = "TEBEC";
						Global.strSucursal = "T";
						break;
				}
				cbResonsable.SelectedItem = item.Responsable + " - " + item.nom_responsable;
				cbAutorizado.SelectedItem = item.Autorizador + " - " + item.nom_autorizador;
				if (!string.IsNullOrEmpty(item.FechaConfirmado))
					dtFechaSalida.Date = DateTime.Parse(item.FechaConfirmado);
			}

			spvFirma.IsVisible = false;
			imgFirma.IsVisible = true;
			DataTable? dt = await Funciones.ObtenerFirma(Global.cidsql_);
			string? firma = string.Empty;
			foreach (DataRow r in dt.Rows)
			{
				firma = dt.Rows[0][0].ToString().Trim();
			}
			if (!string.IsNullOrEmpty(firma))
			{
				srcFirma.Source = ImageSource.FromStream(() =>
				{
					var stream = new MemoryStream(Base64StringIntoImage(firma));
					return stream;
				});
			}
		}

		capturando = true;
	}

	private static byte[] Base64StringIntoImage(string str64)
	{
		byte[] img = Convert.FromBase64String(str64);
		return img;
	}

	private async void cbSucursal_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			lAreas = new List<String>();
			lCentroCostoN1 = new List<string>();
			lCentroCostoN2 = new List<string>();
			lCentroCostoN3 = new List<string>();
			lCentroCostoN4 = new List<string>();

			cbArea.ItemsSource = lAreas;
			cbNivel1.ItemsSource = lCentroCostoN1;
			cbNivel2.ItemsSource = lCentroCostoN2;
			cbNivel3.ItemsSource = lCentroCostoN3;
			cbNivel4.ItemsSource = lCentroCostoN4;
			try
			{
				Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
			}
			catch (Exception ex)
			{
				Global.strArea = "";
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel1.ItemsSource = lCentroCostoN1;
				cbNivel2.ItemsSource = lCentroCostoN2;
				cbNivel3.ItemsSource = lCentroCostoN3;
				cbNivel4.ItemsSource = lCentroCostoN4;
				return;
			}

			Global.strSucursal = cbSucursal.SelectedItem.ToString().Substring(0, 1);
			await LlenarArea();
			await LlenarResponsables();
		}
	}

	private async void cbArea_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			string[] stmp;
			lCentroCostoN1 = new List<string>();
			lCentroCostoN2 = new List<string>();
			lCentroCostoN3 = new List<string>();
			lCentroCostoN4 = new List<string>();

			cbNivel1.ItemsSource = lCentroCostoN1;
			cbNivel2.ItemsSource = lCentroCostoN2;
			cbNivel3.ItemsSource = lCentroCostoN3;
			cbNivel4.ItemsSource = lCentroCostoN4;

			try
			{
				stmp = cbArea.SelectedItem.ToString().Split('-');
				Global.strArea = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strArea = "";
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel1.ItemsSource = lCentroCostoN1;
				cbNivel2.ItemsSource = lCentroCostoN2;
				cbNivel3.ItemsSource = lCentroCostoN3;
				cbNivel4.ItemsSource = lCentroCostoN4;
				return;
			}
			await LlenarCentroCostoN1();
			if (cbArea.SelectedItem != null || cbArea.SelectedIndex > 0)
			{
				if (cbResonsable.SelectedItem != null || cbResonsable.SelectedIndex > 0)
				{
					await LlenarAutorizador();
				}
			}
			cbArea.Unfocus();
		}
	}

	private async void cbNivel1_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			if (cbArea.SelectedItem == null || (string.IsNullOrEmpty(cbArea.SelectedItem.ToString()) && ccn1))
			{
				DisplayAlertAsync("Alerta", "Debe seleccionar un Area para continuar.", "OK");
				cbNivel1.Unfocus();
				ccn2 = false;
				return;
			}
			if (cbNivel1.SelectedItem == null || (string.IsNullOrEmpty(cbNivel1.SelectedItem.ToString()) && ccn1))
			{
				cbNivel1.Unfocus();
				ccn2 = false;
				return;
			}
			string[] stmp;
			cbNivel2.SelectedIndex = -1;

			try
			{
				if (cbNivel1.SelectedItem == null) return;
				stmp = cbNivel1.SelectedItem.ToString().Split('-');
				Global.strCCnivel1 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel1 = "";
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";

				cbNivel2.SelectedIndex = -1;
				cbNivel3.SelectedIndex = -1;
				cbNivel4.SelectedIndex = -1;
				return;
			}
			await LlenarCentroCostoN2();
			cbNivel1.Unfocus();
		}
	}

	private async void cbNivel2_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			if (cbNivel1.SelectedItem == null || (string.IsNullOrEmpty(cbNivel1.SelectedItem.ToString()) && ccn2))
			{
				await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo para continuar.", "OK");
				cbNivel2.Unfocus();
				ccn2 = false;
				return;
			}
			if (cbNivel2.SelectedItem == null || (string.IsNullOrEmpty(cbNivel2.SelectedItem.ToString()) && ccn2))
			{
				await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo de Nivel 2 para continuar.", "OK");
				cbNivel2.Unfocus();
				ccn2 = false;
				return;
			}
			string[] stmp;
			cbNivel3.SelectedIndex = -1;

			try
			{
				stmp = cbNivel2.SelectedItem.ToString().Split('-');
				Global.strCCnivel2 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel2 = "";
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel3.SelectedIndex = -1;
				cbNivel4.SelectedIndex = -1;
				return;
			}
			await LlenarCentroCostoN3();
			cbNivel2.Unfocus();
		}
	}

	private async void cbNivel3_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			if (cbNivel2.SelectedItem == null || (string.IsNullOrEmpty(cbNivel2.SelectedItem.ToString()) && ccn3))
			{
				await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo de Nivel 2 para continuar.", "OK");
				cbNivel3.Unfocus();
				ccn3 = false;
				return;
			}
			if (cbNivel3.SelectedItem == null || (string.IsNullOrEmpty(cbNivel3.SelectedItem.ToString()) && ccn3))
			{
				//await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo de Nivel 3 para continuar.", "OK");
				cbNivel3.Unfocus();
				ccn3 = false;
				return;
			}
			string[] stmp;
			cbNivel4.SelectedIndex = -1;

			try
			{
				stmp = cbNivel3.SelectedItem.ToString().Split('-');
				Global.strCCnivel3 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel3 = "";
				Global.strCCnivel4 = "";
				cbNivel4.SelectedIndex = -1;
				return;
			}
			await LlenarCentroCostoN4();
			cbNivel3.Unfocus();
		}
	}

	private async void cbNivel4_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (capturando)
		{
			if (cbNivel3.SelectedItem == null || (string.IsNullOrEmpty(cbNivel3.SelectedItem.ToString()) && ccn4))
			{
				await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo de Nivel 3 para continuar.", "OK");
				cbNivel4.Unfocus();
				ccn4 = false;
				return;
			}

			if (cbNivel4.SelectedItem == null || (string.IsNullOrEmpty(cbNivel4.SelectedItem.ToString()) && ccn4))
			{
				//await DisplayAlertAsync("Alerta", "Debe seleccionar un Centro de Costo de Nivel 4 para continuar.", "OK");
				cbNivel4.Unfocus();
				ccn4 = false;
				return;
			}
			string[] stmp;
			cbNivel4.SelectedIndex = -1;

			try
			{
				stmp = cbNivel4.SelectedItem.ToString().Split('-');
				Global.strCCnivel4 = stmp[0].ToString().Trim();
			}
			catch (Exception ex)
			{
				Global.strCCnivel4 = "";
				cbNivel4.SelectedIndex = -1;
				return;
			}
			cbNivel4.Unfocus();
		}
	}

	private async void imEditar_Clicked(Object sender, EventArgs e)
	{
		if (Origen == "H") return;
		Global.ArticuloSalidaEditando = new clsArticuloSalida();
		Global.ArticuloSalidaEditando = (sender as MenuItem).BindingContext as clsArticuloSalida;
		Global.Editando = true;
		await Navigation.PushAsync(new Almacen_Salidas_Articulos(Global.strCCnivel3, Global.strCCnivel4, viewModel_ArticuloSalida));
	}

	private void imEliminar_Clicked(Object sender, EventArgs e)
	{
		if (Origen == "H") return;
		else if (Origen.Contains("N") || Origen.Contains("RL"))
		{
			clsArticuloSalida Item_;
			Item_ = (sender as MenuItem).BindingContext as clsArticuloSalida;
			if (OrdenRecoleccion)
				ArticulosEliminados.Add(Item_);

			Global.regArticulosSalida.Remove(Item_);
		}
		else
		{
			Item_ArticuloSalida item_;
			item_ = (sender as MenuItem).BindingContext as Item_ArticuloSalida;
			viewModel_ArticuloSalida.Items.Remove(item_);
		}
	}

	private async void cbResonsable_SelectedIndexChanged(Object sender, EventArgs e)
	{
		if (cbArea.SelectedItem == null || (string.IsNullOrEmpty(cbArea.SelectedItem.ToString()) || cbArea.SelectedIndex <= 0))
		{
			await DisplayAlertAsync("Alerta", "Debe seleccionar un Area para continuar.", "OK");
			return;
		}
		if (string.IsNullOrEmpty(cbResonsable.SelectedItem.ToString()) || cbResonsable.SelectedIndex <= 0) return;

		await LlenarAutorizador();
	}

	private async void btnAgregarItems_Clicked(Object sender, EventArgs e)
	{
		if (Global.strSucursal == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "" || Global.strArea == "") //|| Global.strCCnivel3 == ""
		{
			await DisplayAlertAsync("Informacion", "Llenar primero los datos de centros de costo", "OK");
			return;
		}
		string nivel3 = "", nivel4 = "";
		if (cbNivel3.Items.Count > 0 && cbNivel3.SelectedIndex >= 0)
			if (!string.IsNullOrEmpty(cbNivel3.SelectedItem.ToString())) nivel3 = cbNivel3.SelectedItem.ToString();
		if (cbNivel4.Items.Count > 0 && cbNivel4.SelectedIndex >= 0)
			if (!string.IsNullOrEmpty(cbNivel4.SelectedItem.ToString())) nivel4 = cbNivel4.SelectedItem.ToString();

		await Navigation.PushAsync(new Almacen_Salidas_Articulos(nivel3, nivel4, viewModel_ArticuloSalida));
	}

	private async void btnGenerarSalida_Clicked(Object sender, EventArgs e)
	{
		if (Global.strArea == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "")  //|| Global.strCCnivel3 == ""
		{
			await DisplayAlertAsync("Informacion", "Informacion de centros de costo incompleta", "OK");
			return;
		}

		if (viewModel_ArticuloSalida.Items.Count == 0)
		{
			await DisplayAlertAsync("Informacion", "No hay articulos relacionados a la salida", "OK");
			return;
		}

		if (cbResonsable.SelectedIndex == -1 || cbAutorizado.SelectedIndex == -1 || String.IsNullOrEmpty(cbResonsable.SelectedItem.ToString()))
		{
			await DisplayAlertAsync("Informacion", "Responsable y autorizado invalidos", "OK");
			return;
		}

		var answer = await DisplayAlertAsync("Informaciòn", "¿Desea generar la salida?", "Si", "No");
		if (answer == false)
		{
			return;
		}

		UserDialogs.Instance.ShowLoading("Guardando");
		btnGenerarSalida.IsEnabled = false;
		string strResponsable = "";
		string strAutorizado = "";
		try
		{
			string[] stmp;
			stmp = cbResonsable.SelectedItem.ToString().Split('-');
			strResponsable = stmp[0].ToString().Trim();
		}
		catch (Exception ex)
		{
			//return;
		}

		try
		{
			string[] stmp;
			stmp = cbAutorizado.SelectedItem.ToString().Split('-');
			strAutorizado = stmp[0].ToString().Trim();
		}
		catch (Exception ex)
		{
			//return;
		}

		string FechaFormateada;
		try
		{
			FechaFormateada = dtFechaSalida.Date.Value.Day.ToString().PadLeft(2, '0') + '/' +
				dtFechaSalida.Date.Value.Month.ToString().PadLeft(2, '0') + '/' +
				dtFechaSalida.Date.Value.Year.ToString();
		}
		catch (Exception ex)
		{
			FechaFormateada = DateTime.Now.ToShortDateString();
		}

		string sFolioSalida = "";
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.PostAsync($"api/Operacion/GenerarSalida?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}&OrdenRecoleccion={OrdenRecoleccion}&Responsable={strResponsable}&Autorizado={strAutorizado}", Funciones.jsonContent(viewModel_ArticuloSalida.Items));
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				int i = 1;
				foreach (DataRow r in dt.Rows)
				{
					sFolioSalida = r[0].ToString().Trim().PadLeft(6, '0');
				}
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		if (OrdenRecoleccion)
		{
			//Eliminar los Articulos quitados de la Orden de Recoleccion
			if (ArticulosEliminados.Count > 0)
			{
				try
				{
					response = await SesionApi.DeleteAsync($"api/Operacion/EliminarDetalleRecoleccion?tProyecto={ConfigAPI.TipoProyecto}", Funciones.jsonContent(viewModel_ArticuloSalida.Items));
					if (!response.IsSuccessStatusCode)
					{
						await DisplayAlertAsync("Alerta", "Los Articulos eliminados no se puedieron eliminar correctamente.", "OK");
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
				}
			}
		}

		//TODO : Guardar la firma del responsable de la salida
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

		// Obtenemos solo las lineas del dibujo para generar la imagen, esto es para evitar que se genere una imagen con fondo transparente, ya que el control de firma genera la imagen
		//con fondo transparente y al convertirla a base64 se pierde la transparencia y se genera una imagen con fondo gris, con esta opcion se genera una imagen con fondo blanco y las
		//lineas de la firma, evitando el problema de la transparencia
		var Lines = new List<IDrawingLine>();
		Lines.AddRange(signatureSample.Lines);
		await using var stream = await DrawingViewService.GetImageStream(ImageLineOptions.JustLines(Lines, new Size(400, 280), Brush.White));

		byte[] imageBytes;
		using (BinaryReader reader = new BinaryReader(stream))
		{
			imageBytes = reader.ReadBytes((int)stream.Length);
		}

		try
		{
			response = await SesionApi.PostAsync($"api/Firma", Funciones.jsonContent(new FirmaEntity
			{
				Folio = sFolioSalida,
				Firma = Convert.ToBase64String(imageBytes),
				tProyecto = ConfigAPI.TipoProyecto,
				Metodo = "ws_fnSet_FirmaSalida"
			}));
			if (!response.IsSuccessStatusCode)
			{
				await DisplayAlertAsync("Alerta", "La firma no se pudo guardar correctamente.", "OK");
				return;
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		UserDialogs.Instance.HideHud();
		await DisplayAlertAsync("Guardado Correcto", "Salida " + sFolioSalida, "OK");
		Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
		Global.folio_requisicion_ = string.Empty;
		Global.folio_cotizacion_ = string.Empty;
		Global.folio_orden_ = string.Empty;
		Global.cidsql_ = 0;
		sFolioSalida = "";

		OrdenRecoleccion = false;
		btnGenerarSalida.IsEnabled = true;
		await Navigation.PopAsync();
	}

	private async void btnGenerarSalida2_Clicked(Object sender, EventArgs e)
	{
		if (Global.strArea == "" || Global.strCCnivel1 == "" || Global.strCCnivel2 == "")  //|| Global.strCCnivel3 == ""
		{
			await DisplayAlertAsync("Informacion", "Informacion de centros de costo incompleta", "OK");
			return;
		}

		if (viewModel_ArticuloSalida.Items.Count == 0)
		{
			await DisplayAlertAsync("Informacion", "No hay articulos relacionados a la salida", "OK");
			return;
		}

		if (cbResonsable.SelectedIndex == -1 || cbAutorizado.SelectedIndex == -1 || String.IsNullOrEmpty(cbResonsable.SelectedItem.ToString()))
		{
			await DisplayAlertAsync("Informacion", "Responsable y autorizado invalidos", "OK");
			return;
		}

		var answer = await DisplayAlertAsync("Informaciòn", "¿Desea generar la salida?", "Si", "No");
		if (answer == false)
		{
			return;
		}

		UserDialogs.Instance.ShowLoading("Guardando");
		btnGenerarSalida.IsEnabled = false;
		string strResponsable = "";
		string strnom_resp = "";
		string strAutorizado = "";
		string strnom_Aut = "";
		try
		{
			string[] stmp;
			stmp = cbResonsable.SelectedItem.ToString().Split('-');
			strResponsable = stmp[0].ToString().Trim();
			strnom_resp = stmp[1].ToString().Trim();
		}
		catch (Exception ex)
		{
			//return;
		}

		try
		{
			string[] stmp;
			stmp = cbAutorizado.SelectedItem.ToString().Split('-');
			strAutorizado = stmp[0].ToString().Trim();
			strnom_Aut = stmp[1].ToString().Trim();
		}
		catch (Exception ex)
		{
			//return;
		}

		string FechaFormateada;
		try
		{
			FechaFormateada = dtFechaSalida.Date.Value.Day.ToString().PadLeft(2, '0') + '/' +
				dtFechaSalida.Date.Value.Month.ToString().PadLeft(2, '0') + '/' +
				dtFechaSalida.Date.Value.Year.ToString();
		}
		catch (Exception ex)
		{
			FechaFormateada = DateTime.Now.ToShortDateString();
		}

		ObservableCollection<SalidaAlmacenEntity> salida = new ObservableCollection<SalidaAlmacenEntity>();
		foreach (Item_ArticuloSalida item in viewModel_ArticuloSalida.Items)
		{
			item.FechaConfirmado = FechaFormateada;
			item.Responsable = strResponsable;
			item.nom_responsable = strnom_resp;
			item.Autorizador = strAutorizado;
			item.nom_autorizador = strnom_Aut;
			item.Usuario = Global.clave_usuario;
			item.Nombre_Usuario = Global.nombre_usuario;
		}

		string sFolioSalida = "";
		HttpResponseMessage response;
		try
		{
			response = await SesionApi.PostAsync($"api/Operacion/GenerarSalida?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}&OrdenRecoleccion={OrdenRecoleccion}&Responsable={strResponsable}&Autorizado={strAutorizado}", Funciones.jsonContent(viewModel_ArticuloSalida.Items));
			if (response.IsSuccessStatusCode)
			{
				string jsonResponse = await response.Content.ReadAsStringAsync();
				DataTable? dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
				int i = 1;
				foreach (DataRow r in dt.Rows)
				{
					sFolioSalida = r[0].ToString().Trim().PadLeft(6, '0');
				}
			}
		}
		catch (Exception ex)
		{
			UserDialogs.Instance.HideHud();
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		//TODO : Guardar la firma del responsable de la salida
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

		// Obtenemos solo las lineas del dibujo para generar la imagen, esto es para evitar que se genere una imagen con fondo transparente, ya que el control de firma genera la imagen
		//con fondo transparente y al convertirla a base64 se pierde la transparencia y se genera una imagen con fondo gris, con esta opcion se genera una imagen con fondo blanco y las
		//lineas de la firma, evitando el problema de la transparencia
		var Lines = new List<IDrawingLine>();
		Lines.AddRange(signatureSample.Lines);
		await using var stream = await DrawingViewService.GetImageStream(ImageLineOptions.JustLines(Lines, new Size(400, 280), Brush.White));

		//byte[] imageBytes;
		//using (BinaryReader reader = new BinaryReader(stream))
		//{
		//	imageBytes = reader.ReadBytes((int)stream.Length);
		//}

		try
		{
			response = await SesionApi.PostAsync($"api/Firma?tProyecto={ConfigAPI.TipoProyecto}&usuario={Global.clave_usuario}", Funciones.jsonContent(new FirmaEntity
			{
				Folio = sFolioSalida,
				Firma = Funciones.ConvertToBase64String(stream), // Convert.ToBase64String(imageBytes),
				tProyecto = ConfigAPI.TipoProyecto,
				Metodo = "ws_fnSet_FirmaSalida"
			}));
			if (!response.IsSuccessStatusCode)
			{
				await DisplayAlertAsync("Alerta", "La firma no se pudo guardar correctamente.", "OK");
				UserDialogs.Instance.HideHud();
				return;
			}
		}
		catch (Exception ex)
		{
			UserDialogs.Instance.HideHud();
			Debug.WriteLine($"Login fallo ({SesionApi.BaseAddress}): {ex}");
		}

		UserDialogs.Instance.HideHud();
		await DisplayAlertAsync("Guardado Correcto", $"Salida {sFolioSalida}", "OK");
		Global.regArticulosSalida = new ObservableCollection<clsArticuloSalida>();
		Global.folio_requisicion_ = string.Empty;
		Global.folio_cotizacion_ = string.Empty;
		Global.folio_orden_ = string.Empty;
		Global.cidsql_ = 0;
		sFolioSalida = "";

		OrdenRecoleccion = false;
		btnGenerarSalida.IsEnabled = true;
		await Navigation.PopAsync();
	}

	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (Origen == "H")
			if (e.CurrentSelection.FirstOrDefault() != null) miCollectionView.SelectedItem = null;
	}

	private void OnClearButtonClicked(object sender, EventArgs e)
	{
		signatureSample.Lines.Clear();
	}

	private void cbArea_Focused(object sender, FocusEventArgs e)
	{
		cArea = true;
	}

	private void cbNivel1_Focused(object sender, FocusEventArgs e)
	{
		ccn1 = true;
	}

	private void cbNivel2_Focused(object sender, FocusEventArgs e)
	{
		ccn2 = true;
	}

	private void cbNivel3_Focused(object sender, FocusEventArgs e)
	{
		ccn3 = true;
	}

	private void cbNivel4_Focused(object sender, FocusEventArgs e)
	{
		ccn4 = true;
	}
}