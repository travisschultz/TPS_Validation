using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
	class RefPointResult : INotifyPropertyChanged
	{
		private DoseValue _oldDose;
		private DoseValue _newDose;
		private double _percentDifference;

		public string PatientName { get; set; }
		public string PlanName { get; set; }
		public string FieldName { get; set; }
		public string RefPointName { get; set; }
		public string OldDoseText { get { return _oldDose.ToString(); } }
		public string NewDoseText { get { return _newDose.ToString(); } }
		public string PercentDifferenceText { get { return String.Format("{0:0.00}%", _percentDifference); } }

		public RefPointResult()
		{
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
