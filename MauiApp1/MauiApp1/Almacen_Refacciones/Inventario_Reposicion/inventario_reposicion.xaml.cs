using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;

namespace iAlmacen.Almacen_Refacciones.Inventario_Reposicion
{
    public partial class inventario_reposicion : ContentPage
    {
        public ObservableCollection<Item> Items { get; set; }

        //public Command LoadItemsCommand { get; set; }
        private ItemsViewModel viewModel;

        public inventario_reposicion()
        {
            InitializeComponent();
            txt_filtro.Text = "";
            NavigationPage.SetBackButtonTitle(this, "Atras");

            indicador.IsVisible = true;
            indicador.IsRunning = true;

            //Title = "Lista Reposiciones";
            Items = new ObservableCollection<Item>();
            //LoadItemsCommand = new Command(async () => await cargar_reposiciones());
            BindingContext = viewModel = new ItemsViewModel();

            indicador.IsVisible = false;
            indicador.IsRunning = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            Item item_ = new Item();
            if (item_ == null)
                return;

            if (Global.item_return == false)
            {
                //ItemsListView.SelectedItem = null;
                return;
            }

            item_.cantidad_inventario = Global.item_select.cantidad_inventario;
            item_.texto_2 = "Cantidad Sistema: " + Global.item_select.cantidad + "   /   " + "Cantidad Inventario: " + Global.item_select.cantidad_inventario;
            item_.Color_ = Global.item_select.Color_;

            Global.item_return = false;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = value.ToString();
            switch (valueAsString)
            {
                case (""):
                    {
                        return Colors.Black;
                    }
                case ("Teal"):
                    {
                        return Colors.Teal;
                    }
                default:
                    {
                        return Color.FromHex(value.ToString());
                    }
            }
        }

        private void Handle_Clicked(object sender, EventArgs e)
        {
            Global.filtro_ = txt_filtro.Text.Trim();
            viewModel.LoadItemsCommand.Execute(null);
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection.FirstOrDefault() as Item;
            if (item == null)
                return;

            Global.item_select = item;

            await Navigation.PushAsync(new Item_inventario_reposicion());
        }
    }
}