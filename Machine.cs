using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	public class Machine : INotifyPropertyChanged
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

            //MessageBox.Show(String.Join(", ", Groups.Select(x => x.Name)));
            //MessageBox.Show(dumpContentsTheLongWay());
        }

		private void AddValidation(Course course)
		{
			Groups.Add(new ValidationGroup(course));
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
        public string dumpContentsTheLongWay()
        {
            string output = "";
            foreach (ValidationGroup vg in Groups)
            {
                foreach (ValidationCase vc in vg.Cases)
                {
                    foreach (ValidationTest vt in vc.ValidationTests)
                    {
                        output+=vg.Name + "," + vc.Name + "," + vt.TestName + "," + vt.OldDoseText + "," + vt.NewDoseText + "," + vt.PercentDifferenceText + "," + vt.Result.ToString() + "\n";
                    }
                }
            }
            return output;
            
        }

    }
}
