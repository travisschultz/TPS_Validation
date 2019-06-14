using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;

namespace TPS_Validation
{
	public class ValidationGroup : INotifyPropertyChanged
	{
		private Machine _machine;
        private string _name;
		private List<ValidationCase> _cases;

		public Machine Machine { get { return _machine; } set { _machine = value; OnPropertyChanged("Machine"); } }
		public List<ValidationCase> Cases { get { return _cases; } set { _cases = value; OnPropertyChanged("Cases"); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

		public ValidationGroup(Course course, Machine machine)
		{
            List<ExternalPlanSetup> referencePlanSetups = new List<ExternalPlanSetup>();
            List<ExternalPlanSetup> testPlanSetups = new List<ExternalPlanSetup>();
            Cases = new List<ValidationCase>();
			Machine = machine;

            Name = course.Id;  
            // NEED TO DETERMINE WHEN CYCLING THROUGH PLANS IF IT IS A PLAN OR FIELD VERIFICATION
            foreach (ExternalPlanSetup eps in course.ExternalPlanSetups)
            {
                string caseTypeNumberString = (eps.Id.Split('_'))[0];
                string caseNumberString = caseTypeNumberString.Substring(1);
                int caseNumber = -1;
                Int32.TryParse(caseNumberString, out caseNumber);
                
                if (eps.Id[0]=='R')
                {
                    // It's a reference plan, put it in the index where it belong
                    //System.Windows.MessageBox.Show($"caseTypeNumberSting: {caseTypeNumberString} \n" +
                    //    $"caseNumber.ToString: {caseNumber}\n" +
                    //    $"referencePlanSetups Count: {referencePlanSetups.Count}\n\n" +
                    //    $"Course: {eps.Course}\n" +
                    //    $"Plan: {eps.Id}\n");
                    referencePlanSetups.Add(eps);
                }

                else if (eps.Id[0]=='T')
                {
                    // It's a test plan, put it in the index where it belongs
                    testPlanSetups.Add(eps);
                }
            }

			referencePlanSetups=new List<ExternalPlanSetup>(referencePlanSetups.OrderBy(x => x.Id.Split('_')[0].Substring(1)));
            testPlanSetups = new List<ExternalPlanSetup>(testPlanSetups.OrderBy(x => x.Id.Split('_')[0].Substring(1)));

			//System.Windows.MessageBox.Show($"RefPlanSetups: {String.Join(", ", referencePlanSetups.Select(x => x.Id))} \nTestPlanSetups: {String.Join(", ", testPlanSetups.Select(x => x.Id))}");

			foreach (ExternalPlanSetup eps in referencePlanSetups)
            {
                //!!!!!!!!!!!! CHECK COURSE NAME, IF IT IS Field VALIDATION RUN THIS, ELSE
                if (Name.ToLower().Contains("photon") || Name.ToLower().Contains("electron"))
                {
                    // Field Based
                    try
                    {
                        foreach( Beam refBeam in eps.Beams)
                        {
                            ExternalPlanSetup testEps = testPlanSetups.Where(x => x.Id.Split('_')[0].Substring(1) == eps.Id.Split('_')[0].Substring(1)).First();
                            Beam testBeam = testEps.Beams.Where(x => x.Id == refBeam.Id).First();
                            Cases.Add(new ValidationCase(this, refBeam, testBeam, testEps.Id.Split('_')[1] + " - " + refBeam.Id));
                        }
                    }
                    catch(Exception e)
                    {
                        System.Windows.MessageBox.Show($"Error in Course: {Name} Evaluation\n\n{e.Message}\n {e.StackTrace}");
                    }
                    

                    // ***** THINK THE BELOW CODE RAN INTO ISSUES WITH FIELD ORDER
                    //for (int iBeam = 0; iBeam < testPlanSetups.ElementAt(iCase).Beams.Count(); iBeam++)
                    //{
                    //    var refBeam = referencePlanSetups.ElementAt(iCase).Beams.ElementAt(iBeam);
                    //    var testBeam = testPlanSetups.ElementAt(iCase).Beams.ElementAt(iBeam);
                    //    var caseName = $"{testBeam.Plan.Id.Split('_')[1]} - {testBeam.Id}";
                    //    Cases.Add(new ValidationCase(refBeam, testBeam, caseName));
                    //}
                }
                else
                {
                    // Plan Validation Case

                    ExternalPlanSetup testEps = testPlanSetups.Where(x => x.Id.Split('_')[0].Substring(1) == eps.Id.Split('_')[0].Substring(1)).First();
                    Cases.Add(new ValidationCase(this, eps, testEps, eps.Id));
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
