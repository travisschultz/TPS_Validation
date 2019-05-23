using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
	class ValidationTest : INotifyPropertyChanged
	{
		private ValidationCase _case;
		private DoseValue _oldDose;
		private DoseValue _newDose;
		private double _percentDifference;
		
		public string OldDoseText { get { return _oldDose.ToString(); } }
		public string NewDoseText { get { return _newDose.ToString(); } }
		public string PercentDifferenceText { get { return String.Format("{0:0.00}%", _percentDifference); } }

		public ValidationTest(ValidationCase valCase, Beam beam)
		{
			_case = valCase;

			foreach (ReferencePoint refPoint in _case.Plan.ReferencePoints)
			{

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
