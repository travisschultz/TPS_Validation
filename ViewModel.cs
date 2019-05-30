using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	class ViewModel : INotifyPropertyChanged
	{
		private VMS.TPS.Common.Model.API.Application _app;
		private string _status;
		private ObservableCollection<RefPointResult> _validations;
		private List<String> _photonCalcModels;
		private String _selectedPhotonCalcModel;

		public string Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
		public ObservableCollection<RefPointResult> Validations { get { return _validations; } set { _validations = value; OnPropertyChanged("Validations"); } }
		public List<String> PhotonCalcModels { get { return _photonCalcModels; } set { _photonCalcModels = value; OnPropertyChanged("PhotonCalcModels"); } }
		public String SelectedPhotonCalcModel { get { return _selectedPhotonCalcModel; } set { _selectedPhotonCalcModel = value; OnPropertyChanged("SelectedPhotonCalcModel"); } }

		public ViewModel()
		{
			//create application for looping through patients
			#region Create Application
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
			#endregion

			PhotonCalcModels = GetPhotonCalcModels();
		}

		private List<String> GetPhotonCalcModels()
		{
			List<String> modelList = new List<string>();

			//loop through all of the patients and pull models which are available to them
			foreach(String id in Xml.GetPatientIDs())
			{
				DateTime time = DateTime.Now;//can delete later, just to time how long these loops take

				Patient patient = _app.OpenPatientById(id);

				//loop through all of the nonelectron courses and plans
				foreach(Course course in patient.Courses.Where(c => c.Id != "Electron"))
				{
					foreach(ExternalPlanSetup plan in course.ExternalPlanSetups)
					{
						modelList.AddRange(plan.GetModelsForCalculationType(VMS.TPS.Common.Model.Types.CalculationType.PhotonVolumeDose));
					}
				}
				
				_app.ClosePatient();

				MessageBox.Show($"Checking all plans in {patient} took:\n{DateTime.Now - time}");
			}

			return modelList;
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
