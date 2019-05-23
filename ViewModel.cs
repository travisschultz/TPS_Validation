using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TPS_Validation
{
	class ViewModel : INotifyPropertyChanged
	{
		private VMS.TPS.Common.Model.API.Application _app;
		private string _status;
		private ObservableCollection<RefPointResult> _validations;

		public string Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
		public ObservableCollection<RefPointResult> Validations { get { return _validations; } set { _validations = value; OnPropertyChanged("Validations"); } }

		public ViewModel()
		{
			//create application
			try
			{
				Status = "Logging in...";
				_app = VMS.TPS.Common.Model.API.Application.CreateApplication();
				Status = "";
			}
			catch (Exception exception)
			{
				Status = "Exception was thrown:" + exception.Message;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
