using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering.Printing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

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
			ViewModel vm = DataContext as ViewModel;

			if (vm.SelectedMachine == "Select Machine")
			{
				MessageBox.Show("Please select a machine from the dropdown before running", "No Machine Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (!ValidatePhotonAlgorithmSelection())
				return;

			if (!ValidateAcurosAlgorithmSelection())
				return;

			//if (!ValidateElectronAlgorithmSelection())
			//	return;

			Globals.Instance.ClearLog();

			//make sure a selection is made
			if (vm.SelectedPhotonCalcModel != "")
			{
				foreach (String id in Xml.GetPatientIDs())
				{
					Patient patient = vm.App.OpenPatientById(id);

					//only run if we are on the selected machine or we want to run for all machines
					if (vm.SelectedMachine == "All Machines" || patient.FirstName == vm.SelectedMachine)
					{
						vm.UpdateStatus($"Updating photon algorithms on {patient.Name}...");
						UpdateCalculationAlgorithms.Update(vm.App, patient, vm.SelectedPhotonCalcModel, "", vm.SelectedAcurosCalcModel);
					}


					vm.App.ClosePatient();
				}
			}
			else
			{
				MessageBox.Show("Please select a photon calculation algorithm first", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
			}

			vm.UpdateStatus("");

			MessageBox.Show("Finished updating photon calculation algorithms.  Unfortunately electron algorithms are not available through scripting and must be changed by hand", "Update Finished", MessageBoxButton.OK, MessageBoxImage.Exclamation);

			DisplayLogWindow();
		}

		private void Button_Click_CalcBeams(object sender, RoutedEventArgs e)
		{
			ViewModel vm = DataContext as ViewModel;

			if (vm.SelectedMachine == "Select Machine")
			{
				MessageBox.Show("Please select a machine from the dropdown before running", "No Machine Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Globals.Instance.ClearLog();

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				//only run if we are on the selected machine or we want to run for all machines
				if (vm.SelectedMachine == "All Machines" || patient.FirstName == vm.SelectedMachine)
				{
					vm.UpdateStatus($"Calculating plans on {patient.Name}...");
					CalculateTestPlans.Calculate(vm, patient);
				}

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");

			System.Windows.MessageBox.Show("Completed Calculating the plans");

			DisplayLogWindow();
		}

		private void Button_Click_RunEvaluation(object sender, RoutedEventArgs e)
		{
			ViewModel vm = DataContext as ViewModel;

			if (vm.SelectedMachine == "Select Machine")
			{
				MessageBox.Show("Please select a machine from the dropdown before running", "No Machine Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (!ValidatePhotonTolerance())
				return;

			if (!ValidateElectronTolerance())
				return;

			Globals.Instance.ClearLog();

			vm.Machines.Clear();

            foreach (String id in Xml.GetPatientIDs())
            {
                Patient patient = vm.App.OpenPatientById(id);

				//only run if we are on the selected machine or we want to run for all machines
				if (vm.SelectedMachine == "All Machines" || patient.FirstName == vm.SelectedMachine)
				{
					vm.UpdateStatus($"Running Evaluation on {patient.Name}...");
					vm.Machines.Add(new Machine(patient));
				}

                vm.App.ClosePatient();
            }
            
            vm.UpdateStatus("");

			DisplayLogWindow();
		}

		private void Button_Click_RunAll(object sender, RoutedEventArgs e)
		{
			ViewModel vm = DataContext as ViewModel;

			if (vm.SelectedMachine == "Select Machine")
			{
				MessageBox.Show("Please select a machine from the dropdown before running", "No Machine Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			//need to validate that an algorithm is actually selected first

			if (!ValidatePhotonAlgorithmSelection())
				return;

			if (!ValidateAcurosAlgorithmSelection())
				return;

			//if (!ValidateElectronAlgorithmSelection())
			//	return;

			if (!ValidatePhotonTolerance())
				return;

			if (!ValidateElectronTolerance())
				return;

			Globals.Instance.ClearLog();

			vm.Machines.Clear();

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				//only run if we are on the selected machine or we want to run for all machines
				if (vm.SelectedMachine == "All Machines" || patient.FirstName == vm.SelectedMachine)
				{
					//update algorithm
					vm.UpdateStatus($"Updating photon algorithm on {patient.Name}...");
					UpdateCalculationAlgorithms.Update(vm.App, patient, vm.SelectedPhotonCalcModel, "", vm.SelectedAcurosCalcModel);

					//calc plans
					vm.UpdateStatus($"Calculating plans on {patient.Name}...");
					CalculateTestPlans.Calculate(vm, patient);

					//show results
					vm.UpdateStatus($"Running evaluation on {patient.Name}...");
					vm.Machines.Add(new Machine(patient));
				}

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");

			DisplayLogWindow();
		}

		//loop through photon plans in patients and display somewhere any plans that the algorithm isn't available for and will skip if you continue to use this selection
		private void ComboBox_ValidatePhotonSelection(object sender, RoutedEventArgs e)
		{
			//I'm not sure if this is really doing anything, it doesn't seem to know if an algorithm is approved or not

			ViewModel vm = DataContext as ViewModel;
			bool valid = true;

			vm.UpdateStatus("Checking photon algorithm selection...");

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				//loop through all of the nonelectron courses and plans
				foreach (Course course in patient.Courses.Where(c => c.Id != "Electron"))
				{
					foreach (ExternalPlanSetup plan in course.ExternalPlanSetups)
					{
						//see if selected alg is in the available models
						if (!plan.GetModelsForCalculationType(CalculationType.PhotonVolumeDose).Contains(vm.SelectedPhotonCalcModel))
						{
							valid = false;
							//add machine name to list
							vm.PhotonSelectionValidation += $"{patient.FirstName}, ";
						}
					}
				}

				vm.App.ClosePatient();
			}

			vm.PhotonSelectionValidation.TrimEnd(' ');
			vm.PhotonSelectionValidation.TrimEnd(',');

			//if everything passed hide the message
			if (valid)
				vm.PhotonSelectionValidation = "";
			else
				vm.PhotonSelectionValidation.Insert(0, "AAA algorithm selection is not approved for ");

			vm.UpdateStatus("");
		}

		private void DisplayLogWindow()
		{
			if (Globals.Instance.GetLog() != "")
				MessageBox.Show(Globals.Instance.GetLog(), "Error Log", MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		private void Button_Click_PrintToCSV(object sender, RoutedEventArgs e)
		{
			//validate that evaluation has been run

			// Displays a SaveFileDialog so the user can save the Image  
			// assigned to Button2.  
			System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog
			{
				Filter = "Text Document (*.txt)|*.txt",
				Title = "Save Comma Separated Values (CSV) File of TPS Validation",
				FileName = "TPS Validation",
				RestoreDirectory = true

			};
			saveFileDialog1.ShowDialog();

			// If the file name is not an empty string open it for saving.  
			if (saveFileDialog1.FileName != "")
			{
				ViewModel vm = DataContext as ViewModel;

				string text = "MachineID,CourseID,PlanID - Field Name,Reference Point,Baseline Dose,Validation Dose,Percent Difference,Result" + Environment.NewLine;

				foreach (Machine m in vm.Machines)
				{
					foreach (ValidationGroup vg in m.Groups)
					{
						foreach (ValidationCase vc in vg.Cases)
						{
							foreach (ValidationTest vt in vc.ValidationTests)
							{
								text += m.MachineID + "," + vg.Name + "," + vc.Name + "," + vt.TestName + "," + vt.OldDoseText + "," + vt.NewDoseText + "," + vt.PercentDifferenceText + "," + vt.Result.ToString() + Environment.NewLine;
							}
						}
					}
				}

				File.WriteAllText(saveFileDialog1.FileName, text);
			}
		}

		private void Button_Click_ErrorLog(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Globals.Instance.GetLog(), "Error Log", MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		void Button_Click_PrintToPDF(object sender, RoutedEventArgs e)
		{
			ViewModel vm = DataContext as ViewModel;

			if (vm.Machines.Count == 0)
			{
				MessageBox.Show("Please run evaluation first", "No Evaluation", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			
			var reportData = CreateReportData();

			System.Windows.Forms.PrintDialog printDlg = new System.Windows.Forms.PrintDialog();
			MigraDocPrintDocument printDoc = new MigraDocPrintDocument();
			printDoc.Renderer = new MigraDoc.Rendering.DocumentRenderer(CreateReportData());
			printDoc.Renderer.PrepareDocument();

			printDoc.DocumentName = Window.GetWindow(this).Title;
			//printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
			printDlg.Document = printDoc;
			printDlg.AllowSelection = true;
			printDlg.AllowSomePages = true;
			//Call ShowDialog
			if (printDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				printDoc.Print();
		}

		private Document CreateReportData()
		{
			ViewModel vm = DataContext as ViewModel;

			Document doc = new Document();
			Internal.CustomStyles.Define(doc);
			Section section = new Section();

			// Set up page
			section.PageSetup.PageFormat = PageFormat.Letter;

			section.PageSetup.LeftMargin = Internal.Size.LeftRightPageMargin;
			section.PageSetup.TopMargin = Internal.Size.TopBottomPageMargin;
			section.PageSetup.RightMargin = Internal.Size.LeftRightPageMargin;
			section.PageSetup.BottomMargin = Internal.Size.TopBottomPageMargin;

			section.PageSetup.HeaderDistance = Internal.Size.HeaderFooterMargin;
			section.PageSetup.FooterDistance = Internal.Size.HeaderFooterMargin;

			// Add heder and footer
			new Internal.HeaderAndFooter().Add(section, vm);

			// Add contents
			//new Internal.MachineInfo().Add(section);
			new Internal.ValidationTableContent().Add(section, vm);

			doc.Add(section);

			return doc;
		}

		private bool ValidatePhotonAlgorithmSelection()
		{
			ViewModel vm = DataContext as ViewModel;

			if (!String.IsNullOrEmpty(vm.SelectedPhotonCalcModel))
				return true;
			else
			{
				MessageBox.Show("Please select a photon algorithm from the dropdown", "No Photon Algorithm Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		//check to see if any Acuros plans will be used and if so if an algorithm is selected
		private bool ValidateAcurosAlgorithmSelection()
		{
			ViewModel vm = DataContext as ViewModel;

			if (!String.IsNullOrEmpty(vm.SelectedAcurosCalcModel))
				return true;

			vm.UpdateStatus("Checking for Acuros plans");

			bool AXBNeeded = false;

			//loop through all patients so see if there are any AXB plans that would require and algorithm to be selected
			foreach (String id in Xml.GetPatientIDs())
			{
				if (AXBNeeded)
					continue;

				Patient patient = vm.App.OpenPatientById(id);

				//only chcek if we are on the selected machine or we want to run for all machines
				if (vm.SelectedMachine == "All Machines" || patient.FirstName == vm.SelectedMachine)
				{
					foreach(Course course in patient.Courses)
					{
						if (course.PlanSetups.Where(x => x.Id.ToLower().Contains("axb")).Count() > 0)
						{
							AXBNeeded = true;
							continue;
						}
					}
				}

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");

			if (!(String.IsNullOrEmpty(vm.SelectedAcurosCalcModel) && AXBNeeded))
				return true;
			else
			{
				MessageBox.Show("Please select an Acuros algorithm from the dropdown", "No Acuros Algorithm Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		private bool ValidateElectronAlgorithmSelection()
		{
			ViewModel vm = DataContext as ViewModel;

			if (!String.IsNullOrEmpty(vm.SelectedElectronCalcModel))
				return true;
			else
			{
				MessageBox.Show("Please select an electron algorithm from the dropdown", "No Electron Algorithm Selected", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		private bool ValidatePhotonTolerance()
		{
			ViewModel vm = DataContext as ViewModel;

			if (!Double.IsNaN(vm.PhotonTolerance))
				return true;
			else
			{
				MessageBox.Show("Please enter a photon tolerance", "No Photon Tolerance Entered", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		private bool ValidateElectronTolerance()
		{
			ViewModel vm = DataContext as ViewModel;

			if (!Double.IsNaN(vm.ElectronTolerance))
				return true;
			else
			{
				MessageBox.Show("Please enter an electron tolerance", "No Electron Tolerance Entered", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
	}
}
