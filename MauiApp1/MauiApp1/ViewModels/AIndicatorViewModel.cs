using System.ComponentModel;
using System.Windows.Input;

namespace iAlmacen.ViewModels
{
	public class AIndicatorViewModel: INotifyPropertyChanged
	{
		public ICommand ActivarCommand { get; set; }

		private bool _activador;

		public bool Activador
		{
			get { return _activador; }
			set
			{
				if (value != _activador)
				{
					_activador = value;
					OnPropertyChanged(nameof(Activador));
				}
			}
		}

		private string _mensaje;

		public string Mensaje
		{
			get { return _mensaje; }
			set
			{
				if (value != _mensaje)
				{
					_mensaje = value;
					OnPropertyChanged(nameof(Mensaje));
				}
			}
		}

		public AIndicatorViewModel()
		{
			ActivarCommand = new Command(execute: async () =>
			{
				Mensaje = null;
				Activador = true;
				await Task.Delay(3000); // Simula una operación que tarda 3 segundos

				Mensaje = new string("Prueba"); // Simula un mensaje largo
			});
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion INotifyPropertyChanged
	}
}
