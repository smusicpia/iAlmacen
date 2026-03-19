using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;

namespace iAlmacen.ViewModels;

public class BaseViewModel_Vigilancia : INotifyPropertyChanged
{
    public IDataStore_Vigilancia<Item_entrada_vigilancia> DataStore_Vigilancia => DependencyService.Get<IDataStore_Vigilancia<Item_entrada_vigilancia>>() ?? new MockDataStore_Vigilancia();

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