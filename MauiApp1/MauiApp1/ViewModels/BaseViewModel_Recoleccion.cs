using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;

namespace iAlmacen.ViewModels;

public class BaseViewModel_Recoleccion : INotifyPropertyChanged
{
    public iDataStore_Recoleccion<Item_Virtual_Recoleccion> DataStore_Recoleccion => DependencyService.Get<iDataStore_Recoleccion<Item_Virtual_Recoleccion>>() ?? new MockDataStore_Recoleccion();

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