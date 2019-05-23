using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	class ValidationCase : INotifyPropertyChanged
	{
		private List<ValidationTest> _validationTests;
		private ValidationGroup _group;
		private PlanSetup _plan;

		public List<ValidationTest> ValidationTests { get { return _validationTests; } set { _validationTests = value; OnPropertyChanged("ValidationTests"); } }
		public ValidationGroup Group { get { return _group; } set { _group = value; OnPropertyChanged("Group"); } }
		public PlanSetup Plan { get { return _plan; } set { _plan = value; } }

		public ValidationCase(ValidationGroup group, PlanSetup plan)
		{
			Group = group;
			Plan = plan;

			foreach (Beam beam in plan.Beams.Where(b => !b.IsSetupField))
			{
				AddValidationTest(beam);
			}
		}

		private void AddValidationTest(Beam beam)
		{
			ValidationTests.Add(new ValidationTest(this, beam));
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
