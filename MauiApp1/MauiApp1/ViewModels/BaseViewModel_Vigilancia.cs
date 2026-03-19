using iAlmacen.Models;
using iAlmacen.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    //protected bool SetProperty<T>(ref T backingStore, T value,
    //    [CallerMemberName] string propertyName = "",
    //    Action onChanged = null)
    //{
    //    if (EqualityComparer<T>.Default.Equals(backingStore, value))
    //        return false;

    //    backingStore = value;
    //    onChanged?.Invoke();
    //    OnPropertyChanged(propertyName);
    //    return true;
    //}

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion INotifyPropertyChanged
}