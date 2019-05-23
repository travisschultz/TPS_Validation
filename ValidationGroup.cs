using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	class ValidationGroup : INotifyPropertyChanged
	{
		private Machine _machine;
		private List<ValidationCase> _cases;
		
		public List<ValidationCase> Cases { get { return _cases; } set { _cases = value; OnPropertyChanged("Cases"); } }

		public ValidationGroup(Machine machine, Course course)
		{
			_machine = machine;
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
