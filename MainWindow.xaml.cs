using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ViewModel();
		}

		private void Button_Click_UpdateAlgorithms(object sender, RoutedEventArgs e)
		{
			//need to validate that an algorithm is actually selected first

			ViewModel vm = DataContext as ViewModel;

			vm.UpdateStatus("Updating photon algorithm selection...");

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				UpdateCalculationAlgorithms.Update(vm.App, patient, vm.SelectedPhotonCalcModel, "");

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");
		}

		private void Button_Click_CalcBeams(object sender, RoutedEventArgs e)
		{

			ViewModel vm = DataContext as ViewModel;

			vm.UpdateStatus("Calculating plans...");

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				CalculateTestPlans.Calculate(vm.App, patient);

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");

		}

		private void Button_Click_RunEvaluation(object sender, RoutedEventArgs e)
		{
            ViewModel vm = DataContext as ViewModel;

            vm.UpdateStatus("Running Evaluation on Patients...");

            foreach (String id in Xml.GetPatientIDs())
            {
                Patient patient = vm.App.OpenPatientById(id);

                vm.Machines.Add(RunEvaluation.RunEvaluationOnPatient(patient));

                vm.App.ClosePatient();
            }

            
            vm.UpdateStatus("");

        }

		private void ComboBox_ValidatePhotonSelection(object sender, RoutedEventArgs e)
		{
			//loop through photon plans in patients and display somewhere any plans that the algorithm isn't available for and will skip if you continue to use this selection

			ViewModel vm = DataContext as ViewModel;

			vm.UpdateStatus("Checking photon algorithm selection...");

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				//loop through all of the nonelectron courses and plans
				foreach (Course course in patient.Courses.Where(c => c.Id != "Electron"))
				{
					foreach (ExternalPlanSetup plan in course.ExternalPlanSetups)
					{
						//modelList = new List<String>(modelList.Union(plan.GetModelsForCalculationType(VMS.TPS.Common.Model.Types.CalculationType.PhotonVolumeDose)));
					}
				}

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");
		}
	}
}
