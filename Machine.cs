using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	class Machine : INotifyPropertyChanged
	{
		private List<ValidationGroup> _groups;
		private string _machineId;

		public List<ValidationGroup> Groups { get { return _groups; } set { _groups = value; OnPropertyChanged("Validations"); } }
		public string MachineID { get { return _machineId; } set { _machineId = value; OnPropertyChanged("MachineID"); } }

		public Machine(Patient patient)
		{
			MachineID = patient.FirstName;

			Groups = new List<ValidationGroup>();

			foreach (Course course in patient.Courses)
			{
				AddValidation(course);
			}
		}

		private void AddValidation(Course course)
		{
			Groups.Add(new ValidationGroup(this, course));
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
