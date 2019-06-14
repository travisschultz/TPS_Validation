using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
	public class ValidationCase : INotifyPropertyChanged
	{
		private List<ValidationTest> _validationTests;
		private ValidationGroup _group;  // not sure we would need this if Validation Cases exist within groups
		private ExternalPlanSetup _referencePlan;
        private ExternalPlanSetup _testPlan;
        private Beam _referenceBeam;
        private Beam _testBeam;
        private string _name;
        private string _type; // "beam" tests or "plan" tests

        public List<ValidationTest> ValidationTests { get { return _validationTests; } set { _validationTests = value; OnPropertyChanged("ValidationTests"); } }
		public ValidationGroup Group { get { return _group; } set { _group = value; OnPropertyChanged("Group"); } }
		public ExternalPlanSetup ReferencePlan { get { return _referencePlan; } set { _referencePlan = value; } }
        public ExternalPlanSetup TestPlan { get { return _testPlan; } set { _testPlan = value; } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }


		public ValidationCase(ValidationGroup group, ExternalPlanSetup plan)
		{
			Group = group;
			ReferencePlan = plan;

			foreach (Beam beam in plan.Beams.Where(b => !b.IsSetupField))
			{
				AddValidationTest(beam);
			}
		}

        public ValidationCase (ValidationGroup group, ExternalPlanSetup refPlan, ExternalPlanSetup testPlan, string testCaseName)
		{
			Group = group;
			ValidationTests = new List<ValidationTest>();
            ReferencePlan = refPlan;
            TestPlan = testPlan;
            Name = testCaseName;
            RunPlanValidationTests();

        }

        public ValidationCase (ValidationGroup group, Beam refBeam, Beam testBeam, string testName)
		{
			Group = group;
			ValidationTests = new List<ValidationTest>();
            _referenceBeam = refBeam;
            _testBeam = testBeam;
            Name = testName;
            Type = "Field";
            RunFieldValidationTests();
        }

		private void AddValidationTest(Beam beam)
		{
			ValidationTests.Add(new ValidationTest(this, beam));
		}

        private void RunFieldValidationTests() 
        {
			if (Math.Round(_testBeam.Meterset.Value, 2) != Math.Round(_referenceBeam.Meterset.Value, 2))
				ValidationLog.Instance.CreateEntry($"MUs not equal - {Group.Machine.MachineID} - {Group.Name} - {Name} - Reference Beam: {_referenceBeam.Id}({_referenceBeam.Meterset.Value} MU) Test Beam: {_testBeam.Id}({_testBeam.Meterset.Value} MU)\n");
				//System.Windows.MessageBox.Show($"MUs not equal - {Group.Machine.MachineID} - {Group.Name} - {Name} - Reference Beam: {_referenceBeam.Id}({_referenceBeam.Meterset}) Test Beam: {_testBeam.Id}({_testBeam.Meterset})");

			foreach ( FieldReferencePoint rfrp in _referenceBeam.FieldReferencePoints.Where(x => !Double.IsNaN(x.RefPointLocation.x)))
            {
                // HARDCODING SOME THINGS HERE
                double toleranceValueHardCoded = 2;
                double analysisCutoffFraction = .5;

                // 
                //string testName = rfrp.Id.ToString();
                string testName = rfrp.ReferencePoint.Id.ToString();


                FieldReferencePoint tfrp = _testBeam.FieldReferencePoints.Where(x => x.ReferencePoint.Id == rfrp.ReferencePoint.Id && !Double.IsNaN(x.RefPointLocation.x)).First();

				// below if essentially makes sure the reference point is inside the field being evaluated
				//if (rfrp.FieldDose > analysisCutoffFraction*_referenceBeam.Dose.GetAbsoluteBeamDoseValue(_referenceBeam.Dose.DoseMax3D))
				if (rfrp.FieldDose > new DoseValue(50, DoseValue.DoseUnit.cGy))
				{
                    ValidationTests.Add(new ValidationTest(testName, rfrp.FieldDose, tfrp.FieldDose, toleranceValueHardCoded));
                }
            }

     //       for (int iRefPt=0; iRefPt < _referenceBeam.FieldReferencePoints.Count(); iRefPt++)
     //       {
     //           if (!Double.IsNaN(_referenceBeam.FieldReferencePoints.ElementAt(iRefPt).RefPointLocation.x))
     //           {
     //               // This would assume the reference points are in the same order for each field

     //               // HARDCODING SOMETHING HERE
     //               double toleranceValueHardCoded = 2;

     //               // Writing it long style to make it easier to see at first, may be too long to one line it anyhow
     //               DoseValue refDV = _referenceBeam.FieldReferencePoints.ElementAt(iRefPt).FieldDose;
     //               DoseValue testDV = _testBeam.FieldReferencePoints.ElementAt(iRefPt).FieldDose;
     //               string testName = _referenceBeam.FieldReferencePoints.ElementAt(iRefPt).ReferencePoint.Id.ToString();

					////System.Windows.MessageBox.Show($"refpt dose: {refDV}\nisAbsolute?{refDV.IsAbsoluteDoseValue}\nbeam dmax dose: {_referenceBeam.Dose.DoseMax3D}\nIsAbsolute?{_referenceBeam.Dose.GetAbsoluteBeamDoseValue(_referenceBeam.Dose.DoseMax3D)}");
					//if (refDV > 0.5 * _referenceBeam.Dose.GetAbsoluteBeamDoseValue(_referenceBeam.Dose.DoseMax3D))
     //               {
     //                   ValidationTests.Add(new ValidationTest(testName, refDV, testDV, toleranceValueHardCoded));
     //               }
                    
     //           }
     //       }
        }
        private void RunPlanValidationTests()
        {
            //
            foreach (Structure testStruct in TestPlan.StructureSet.Structures) // assumes the same structure set
            {
                if ( testStruct.DicomType.ToUpper() == "PTV")
                {
                    ///  Tests for the target type go here
                    ///   - dmax
                    ///   - dmin
                    ///   - mean dose
                    ///   - d95
                    ///


                    // GRAB THE REF STRUCT
                    try
                    {
                        var refStruct = ReferencePlan.StructureSet.Structures.Where(n => n.Id == testStruct.Id).FirstOrDefault();

                        // variables for creting the cases
                        var rsDVH = ReferencePlan.GetDVHCumulativeData(refStruct, DoseValuePresentation.Absolute, VolumePresentation.Relative, 1);
                        var tsDVH = TestPlan.GetDVHCumulativeData(refStruct, DoseValuePresentation.Absolute, VolumePresentation.Relative, 1);
                        var rsD95 = ReferencePlan.GetDoseAtVolume(refStruct, 95, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                        var tsD95 = TestPlan.GetDoseAtVolume(refStruct, 95, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Dmax", rsDVH.MaxDose, tsDVH.MaxDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Dmin", rsDVH.MinDose, tsDVH.MinDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Mean", rsDVH.MeanDose, tsDVH.MeanDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " D95", rsD95, tsD95, 2));

                    }
                    catch(Exception e)
                    {
                        System.Windows.MessageBox.Show($"Error in the target runplanvalidations in validationscase\n\n{e.Message}");
                    }

                }
                else if (testStruct.DicomType.ToLower()=="avoidance" || testStruct.DicomType.ToLower()=="organ")
                {
                    try
                    {
                        // run the avoidance tests
                        var refStruct = ReferencePlan.StructureSet.Structures.Where(n => n.Id == testStruct.Id).FirstOrDefault();

                        // variables for creting the cases
                        var rsDVH = ReferencePlan.GetDVHCumulativeData(refStruct, DoseValuePresentation.Absolute, VolumePresentation.Relative, 1);
                        var tsDVH = TestPlan.GetDVHCumulativeData(refStruct, DoseValuePresentation.Absolute, VolumePresentation.Relative, 1);
                        var rsD20 = ReferencePlan.GetDoseAtVolume(refStruct, 20, VolumePresentation.Relative, DoseValuePresentation.Absolute);
                        var tsD20 = TestPlan.GetDoseAtVolume(refStruct, 20, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Dmax", rsDVH.MaxDose, tsDVH.MaxDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Dmin", rsDVH.MinDose, tsDVH.MinDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " Mean", rsDVH.MeanDose, tsDVH.MeanDose, 2));
                        ValidationTests.Add(new ValidationTest(testStruct.Id + " D20", rsD20, tsD20, 2));
                    }
                    catch(Exception e)
                    {
                        System.Windows.MessageBox.Show($"Error in the target runplanvalidations in validationscase\n\n{e.Message}");
                    }
                    

                }

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
