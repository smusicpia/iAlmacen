using System.ComponentModel;

namespace iAlmacen.ViewModels
{
	public class BaseObservableObject : INotifyPropertyChanged
	{
		private bool _connectivity_IsVisible = false;
		private bool _connectivity_Green_IsVisible = false;
		private bool _connectivity_Red_IsVisible = false;
		private string _message_Connectivity = string.Empty;
		private Color textColor_Connectivity = Colors.Black;

		public bool Connectivity_IsVisible
		{
			get { return _connectivity_IsVisible; }
			set
			{
				_connectivity_IsVisible = value;
				OnPropertyChanged(nameof(Connectivity_IsVisible));
			}
		}

		public bool Connectivity_Green_IsVisible
		{
			get { return _connectivity_Green_IsVisible; }
			set
			{
				_connectivity_Green_IsVisible = value;
				OnPropertyChanged(nameof(Connectivity_Green_IsVisible));
			}
		}

		public bool Connectivity_Red_IsVisible
		{
			get { return _connectivity_Red_IsVisible; }
			set
			{
				_connectivity_Red_IsVisible = value;
				OnPropertyChanged(nameof(Connectivity_Red_IsVisible));
			}
		}

		public string Message_Connectivity
		{ 
			get { return _message_Connectivity; }
			set {
					_message_Connectivity = value;
					OnPropertyChanged(nameof(Message_Connectivity));
				}
		}
		public Color TextColor_Connectivity
		{
			get { return textColor_Connectivity; }
			set
			{
				textColor_Connectivity = value;
				OnPropertyChanged(nameof(TextColor_Connectivity));
			}
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
