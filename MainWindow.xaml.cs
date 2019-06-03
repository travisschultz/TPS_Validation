using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				vm.UpdateStatus($"Updating photon algorithms on {patient.Name}...");
				UpdateCalculationAlgorithms.Update(vm.App, patient, vm.SelectedPhotonCalcModel, "");

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");
		}

		private void Button_Click_CalcBeams(object sender, RoutedEventArgs e)
		{

			ViewModel vm = DataContext as ViewModel;

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				vm.UpdateStatus($"Calculating plans on {patient.Name}...");
				CalculateTestPlans.Calculate(vm, patient);

				vm.App.ClosePatient();
			}

			vm.UpdateStatus("");
		}

		private void Button_Click_RunEvaluation(object sender, RoutedEventArgs e)
		{
            ViewModel vm = DataContext as ViewModel;

            foreach (String id in Xml.GetPatientIDs())
            {
                Patient patient = vm.App.OpenPatientById(id);

				vm.UpdateStatus($"Running Evaluation on {patient.Name}...");
				vm.Machines.Add(new Machine(patient));

                vm.App.ClosePatient();
            }
            
            vm.UpdateStatus("");
		}

		private void Button_Click_RunAll(object sender, RoutedEventArgs e)
		{
			//need to validate that an algorithm is actually selected first

			ViewModel vm = DataContext as ViewModel;

			foreach (String id in Xml.GetPatientIDs())
			{
				Patient patient = vm.App.OpenPatientById(id);

				//update algorithm
				vm.UpdateStatus($"Updating photon algorithm on {patient.Name}...");
				UpdateCalculationAlgorithms.Update(vm.App, patient, vm.SelectedPhotonCalcModel, "");

				//calc plans
				vm.UpdateStatus($"Calculating plans on {patient.Name}...");
				CalculateTestPlans.Calculate(vm, patient);

				//show results
				vm.UpdateStatus($"Running evaluation on {patient.Name}...");
				vm.Machines.Add(new Machine(patient));

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

		void Button_Click_PrintToPDF(object sender, RoutedEventArgs e)
		{
			//this was all copy pasted from DVHAnalysis, still need to go through it
			MessageBox.Show("Not yet implemented");

		//	//validate that evaluation has been run

		//	var reportService = new ReportPdf();
		//	var reportData = CreateReportData();

		//	System.Windows.Forms.PrintDialog printDlg = new System.Windows.Forms.PrintDialog();
		//	MigraDocPrintDocument printDoc = new MigraDocPrintDocument();
		//	printDoc.Renderer = new MigraDoc.Rendering.DocumentRenderer(reportService.CreateReport(reportData));
		//	printDoc.Renderer.PrepareDocument();

		//	printDoc.DocumentName = Window.GetWindow(this).Title;
		//	//printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
		//	printDlg.Document = printDoc;
		//	printDlg.AllowSelection = true;
		//	printDlg.AllowSomePages = true;
		//	//Call ShowDialog
		//	if (printDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		//		printDoc.Print();
		//}

		//private ReportData CreateReportData()
		//{
		//	ReportData reportData = new ReportData();

		//	reportData.Patient = new SimplePdfReport.Reporting.Patient
		//	{
		//		Id = _vm.PatientID,
		//		Name = _vm.PatientName
		//	};

		//	reportData.User = new SimplePdfReport.Reporting.User
		//	{
		//		Username = _vm.CurrentUser
		//	};

		//	reportData.Plans = new SimplePdfReport.Reporting.Plans
		//	{

		//		Id = _vm.PlanID,
		//		Course = _vm.CourseID == "" ? "" : $" ({_vm.CourseID})",
		//		Protocol = ConstraintList.GetProtocolName(_vm.SelectedProtocol),
		//		PlanList = new List<Plan>()
		//	};

		//	foreach (PlanInformation plan in _vm.Plans)
		//	{
		//		SimplePdfReport.Reporting.Plan newPlan = new SimplePdfReport.Reporting.Plan
		//		{
		//			Id = plan.PlanID,
		//			TotalDose = plan.TotalPlannedDose,
		//			DosePerFx = plan.DosePerFraction,
		//			Fractions = plan.NumberOfFractions
		//		};

		//		reportData.Plans.PlanList.Add(newPlan);
		//	}

		//	reportData.DvhTable = new DVHTable
		//	{
		//		Title = "DVH Analysis Report"
		//	};

		//	foreach (DVHTableRow row in _vm.DVHTable)
		//	{
		//		SimplePdfReport.Reporting.DVHTableRow newRow = new SimplePdfReport.Reporting.DVHTableRow();

		//		newRow.StructureId = row.Structure ?? "";
		//		newRow.PlanStructureId = row.SelectedStructure != null ? row.SelectedStructure.Id : "";
		//		newRow.Constraint = row.ConstraintText ?? "";
		//		newRow.VariationConstraint = row.VariationConstraintText ?? "";
		//		newRow.Limit = row.LimitText ?? "";
		//		newRow.VariationLimit = row.VariationLimitText ?? "";
		//		newRow.PlanValue = row.PlanValueText ?? "";
		//		newRow.PlanResult = row.PlanResult ?? "";
		//		newRow.PlanResultColor = row.PlanResultColor;

		//		reportData.DvhTable.Rows.Add(newRow);
		//	}

		//	return reportData;
		}
	}
}
