using iAlmacen.Almacen_Refacciones.Herramientas_v2;
using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using System.Collections.ObjectModel;

namespace iAlmacen.Almacen_Refacciones.InventarioH
{
    public partial class frmArticulosInventarioH : ContentPage
    {
        public ObservableCollection<Item_RegArticulo> Items { get; set; }
        public Command LoadItemsCommand_RegArticulo { get; set; }
        private ItemsViewModel_RegArticulo viewModel_RegArticulo;

        public frmArticulosInventarioH()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
            this.Title = "Articulos del Inventario: " + Global.FolioInventario;

            Items = new ObservableCollection<Item_RegArticulo>();
            LoadItemsCommand_RegArticulo = new Command(async () => await cargar());
            BindingContext = viewModel_RegArticulo = new ItemsViewModel_RegArticulo($"{Global.FolioInventario}");
        }

        private async Task cargar()
        { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel_RegArticulo.LoadItemsCommand_regArticulo.Execute($"{Global.FolioInventario}");
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item_RegArticulo;
            if (item == null)
                return;
            Global.ArticuloEnInventario = item;
            await Navigation.PushAsync(new frmCapturaInventarioH(false));
        }
    }
}