using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;

namespace iAlmacen
{
    public class BaseViewModel_ArticuloEnResguardo : INotifyPropertyChanged
    {
        public IDataStore_Herramienta<Item_ArticuloEnResguardo> DataStore => DependencyService.Get<IDataStore_Herramienta<Item_ArticuloEnResguardo>>() ?? new MockDataStore_ArticuloEnResguardo();
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}