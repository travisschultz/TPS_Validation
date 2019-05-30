using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
	class ValidationCase : INotifyPropertyChanged
	{
		private List<ValidationTest> _validationTests;
		//private ValidationGroup _group;  // not sure we would need this if Validation Cases exist within groups
		private ExternalPlanSetup _referencePlan;
        private ExternalPlanSetup _testPlan;
        private Beam _referenceBeam;
        private Beam _testBeam;
        private string _name;
        private string _type; // "beam" tests or "plan" tests

        public List<ValidationTest> ValidationTests { get { return _validationTests; } set { _validationTests = value; OnPropertyChanged("ValidationTests"); } }
		//public ValidationGroup Group { get { return _group; } set { _group = value; OnPropertyChanged("Group"); } }
		public ExternalPlanSetup ReferencePlan { get { return _referencePlan; } set { _referencePlan = value; } }
        public ExternalPlanSetup TestPlan { get { return _testPlan; } set { _testPlan = value; } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }


		public ValidationCase(ValidationGroup group, ExternalPlanSetup plan)
		{
			//Group = group;
			ReferencePlan = plan;

			foreach (Beam beam in plan.Beams.Where(b => !b.IsSetupField))
			{
				AddValidationTest(beam);
			}
		}

        public ValidationCase (ExternalPlanSetup refPlan, ExternalPlanSetup testPlan, string testCaseName)
        {
            ReferencePlan = refPlan;
            TestPlan = testPlan;
            Name = testCaseName;
            RunPlanValidationTests();

        }

        public ValidationCase (Beam refBeam, Beam testBeam, string testName)
        {
            _referenceBeam = refBeam;
            _testBeam = testBeam;
            Type = "Field";
            RunFieldValidationTests();
        }

		private void AddValidationTest(Beam beam)
		{
			ValidationTests.Add(new ValidationTest(this, beam));
		}

        private void RunFieldValidationTests() 
        {
            for (int iRefPt=0; iRefPt < _referenceBeam.FieldReferencePoints.Count(); iRefPt++)
            {
                // This would assume the reference points are in the same order for each field

                // HARDCODING SOMETHING HERE
                double toleranceValueHardCoded = 2;

                // Writing it long style to make it easier to see at first, may be too long to one line it anyhow
                DoseValue refDV = _referenceBeam.FieldReferencePoints.ElementAt(iRefPt).FieldDose;
                DoseValue testDV = _testBeam.FieldReferencePoints.ElementAt(iRefPt).FieldDose;
                string testName = _referenceBeam.FieldReferencePoints.ElementAt(iRefPt).Id.ToString();
                ValidationTests.Add(new ValidationTest(testName,refDV,testDV, toleranceValueHardCoded));

            }
        }
        private void RunPlanValidationTests()
        {
            //
            foreach (Structure testStruct in TestPlan.StructureSet.Structures) // assumes the same structure set
            {
                if ( testStruct.DicomType == "PTV")
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
                        var refStruct = ReferencePlan.StructureSet.Structures.Where(n => n.Id == testStruct.Id);
                        //ValidationTests.Add( new ValidationTest("Dmax", refStruct.Do))
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Error in the target runplanvalidations in validationscase");
                    }
                    

                   

           

                }
                else if (s.DicomType=="Avoidance")
                {
                    // run the avoidance tests
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
