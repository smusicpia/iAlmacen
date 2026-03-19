using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;

namespace iAlmacen.ViewModels;

public class BaseViewModel_Proterm : INotifyPropertyChanged
{
    public IDataStore_Proterm<Item_proterm> DataStore_Proterm => DependencyService.Get<IDataStore_Proterm<Item_proterm>>() ?? new MockDataStore_Proterm();

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