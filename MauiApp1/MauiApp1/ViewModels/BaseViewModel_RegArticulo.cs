using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;

namespace iAlmacen
{
    public class BaseViewModel_RegArticulo : INotifyPropertyChanged
    {
        public IDataStore_Herramienta<Item_RegArticulo> DataStore => DependencyService.Get<IDataStore_Herramienta<Item_RegArticulo>>() ?? new MockDataStore_Herramienta();
        private bool _isBusy = false;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string _title = string.Empty;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

		private Item_ArticuloSalida _item_ArticuloSalida;
		public Item_ArticuloSalida item_ArticuloSalida
		{
			get 
            {
                return _item_ArticuloSalida;
            }
			set
            {
                _item_ArticuloSalida = value;
                OnPropertyChanged(nameof(item_ArticuloSalida));
            }
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}