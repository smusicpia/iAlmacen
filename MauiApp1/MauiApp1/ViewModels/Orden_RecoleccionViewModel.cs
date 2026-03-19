using iAlmacen.Clases;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iAlmacen.ViewModels;

public class Orden_RecoleccionViewModel : INotifyPropertyChanged
{
    public List<clsSucursal> Sucursales { get; set; }

    public Orden_RecoleccionViewModel()
    {
        Sucursales = new List<clsSucursal>();
        Sucursales.Add(new clsSucursal() { ID = 1, Clave = "M", Descripcion = "MÉRIDA" });
        Sucursales.Add(new clsSucursal() { ID = 2, Clave = "T", Descripcion = "TEBEC" });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged == null)
            return;

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public clsSucursal _sucursal;

    public clsSucursal SelectedSucursal
    {
        get { return _sucursal; }
        set
        {
            _sucursal = value;
            OnPropertyChanged();
        }
    }

    public clsArea _area;

    public clsArea SelectedArea
    {
        get { return _area; }
        set
        {
            _area = value;
            OnPropertyChanged();
        }
    }

    private string _totalArticulos;

    public string TotalArticulos { get => _totalArticulos; set => _totalArticulos = value; }
}