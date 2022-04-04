using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using VMS.TPS.Common.Model.API;

[assembly: ESAPIScript(IsWriteable = true)]
//[assembly: AssemblyVersion("1.2.0.0")]

namespace TPS_Validation
{
	public class ViewModel : INotifyPropertyChanged
	{
		private string _status;
		private ObservableCollection<Machine> _machines;
		private List<String> _photonCalcModels;
		private List<String> _acurosCalcModels;
		private String _selectedPhotonCalcModel;
		private String _selectedAcurosCalcModel;
		private String _selectedElectronCalcModel;
		private String _photonSelectionValidation;
		private Double _photonTolerance = 2;
		private Double _electronTolerance = 2;
		private List<String> _machineList;
		private String _selectedMachine;

		public VMS.TPS.Common.Model.API.Application App { get; set; }
		public string Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
		public ObservableCollection<Machine> Machines { get { return _machines; } set { _machines = value; OnPropertyChanged("Machines"); } }
		public List<String> PhotonCalcModels { get { return _photonCalcModels; } set { _photonCalcModels = value; OnPropertyChanged("PhotonCalcModels"); } }
		public List<String> AcurosCalcModels { get { return _acurosCalcModels; } set { _acurosCalcModels = value; OnPropertyChanged("AcurosCalcModels"); } }
		public String SelectedPhotonCalcModel { get { return _selectedPhotonCalcModel; } set { _selectedPhotonCalcModel = value; OnPropertyChanged("SelectedPhotonCalcModel"); } }
		public String SelectedAcurosCalcModel { get { return _selectedAcurosCalcModel; } set { _selectedAcurosCalcModel = value; OnPropertyChanged("SelectedAcurosCalcModel"); } }
		public String SelectedElectronCalcModel { get { return _selectedElectronCalcModel; } set { _selectedElectronCalcModel = value; OnPropertyChanged("SelectedElectronCalcModel"); } }
		public String PhotonSelectionValidation { get { return _photonSelectionValidation; } set { _photonSelectionValidation = value; OnPropertyChanged("PhotonSelectionValidation"); } }
		public Double PhotonTolerance { get { return _photonTolerance; } set { _photonTolerance = value; OnPropertyChanged("PhotonTolerance"); } }
		public Double ElectronTolerance { get { return _electronTolerance; } set { _electronTolerance = value; OnPropertyChanged("ElectronTolerance"); } }
		public List<String> MachineList { get { return _machineList; } set { _machineList = value; OnPropertyChanged("MachineList"); } }
		public String SelectedMachine { get { return _selectedMachine; } set { _selectedMachine = value; OnPropertyChanged("SelectedMachine"); } }

		public ViewModel()
		{
			//create application for looping through patients
			#region Create Application
			try
			{
				Status = "Logging in...";
				App = VMS.TPS.Common.Model.API.Application.CreateApplication();
				Status = "";
			}
			catch (Exception exception)
			{
				Status = "Exception was thrown:" + exception.Message;
			}
			#endregion

			PhotonSelectionValidation = "";

			UpdateStatus("Gathering photon algorithms...");
			PhotonCalcModels = new List<string>(GetPhotonCalcModels());
			AcurosCalcModels = new List<string>(PhotonCalcModels.Where(x => x.ToLower().Contains("acurosxb")));
			PhotonCalcModels = new List<string>(PhotonCalcModels.Where(x => !x.ToLower().Contains("acurosxb")));
			UpdateStatus("");

			Machines = new ObservableCollection<Machine>();

			//set up dropdown of machines
			MachineList = new List<string>();
			//MachineList.Add("All Machines");
			MachineList.Add("Select Machine");
			SelectedMachine = "Select Machine";
			foreach(String id in Xml.GetPatientIDs())
			{
				Patient patient = App.OpenPatientById(id);
				MachineList.Add(patient.FirstName);
				App.ClosePatient();
			}
		}

		private List<String> GetPhotonCalcModels()
		{
			List<String> modelList = new List<string>();

			//loop through all of the patients and pull models which are available to them
			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = App.OpenPatientById(id);

				//loop through all of the nonelectron courses and plans
				foreach(Course course in patient.Courses.Where(c => c.Id != "Electron"))
				{
					foreach(ExternalPlanSetup plan in course.ExternalPlanSetups)
					{
						modelList = new List<String>(modelList.Union(plan.GetModelsForCalculationType(VMS.TPS.Common.Model.Types.CalculationType.PhotonVolumeDose)));
					}
				}

				App.ClosePatient();
			}

			return modelList;
		}

		public static void RefreshUI()
		{
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
			{
				frame.Continue = false;
				return null;
			}), null);

			Dispatcher.PushFrame(frame);
			//EDIT:
			System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
		}

		public void UpdateStatus(string status)
		{
			Status = status;

			RefreshUI();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}

			if (name == "PhotonTolerance")
			{
				Globals.Instance.SetPhotonTolerance(PhotonTolerance);
			}
			if (name == "ElectronTolerance")
				Globals.Instance.SetElectronTolerance(ElectronTolerance);
		}
	}
}
