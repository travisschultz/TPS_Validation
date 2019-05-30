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

		
		public List<ValidationCase> Cases { get { return _cases; } set { _cases = value; OnPropertyChanged("Cases"); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }

		public ValidationGroup(Course course)
		{
            List<ExternalPlanSetup> referencePlanSetups = new List<ExternalPlanSetup>();
            List<ExternalPlanSetup> testPlanSetups = new List<ExternalPlanSetup>();
            Cases = new List<ValidationCase>();

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
                    referencePlanSetups.Insert(caseNumber, eps);
                }

                else if (eps.Id[0]=='T')
                {
                    // It's a test plan, put it in the index where it belongs
                    testPlanSetups.Insert(caseNumber, eps);
                }
            }

            for (int iCase = 0; iCase < referencePlanSetups.Count; iCase++) // Risk of size of each not matching
            {
                //!!!!!!!!!!!! CHECK COURSE NAME, IF IT IS Field VALIDATION RUN THIS, ELSE
                if (Name=="Photons" || Name=="Electrons")
                {
                    // Field Based
                    for (int iBeam=0;iBeam<testPlanSetups.ElementAt(iCase).Beams.Count();iBeam++)
                    {
                        var refBeam = referencePlanSetups.ElementAt(iCase).Beams.ElementAt(iBeam);
                        var testBeam = testPlanSetups.ElementAt(iCase).Beams.ElementAt(iBeam);
                        var caseName = testBeam.Id;
                        Cases.Add(new ValidationCase(refBeam, testBeam, caseName));
                    }
                }
                else
                {
                    // Plan Validation Case

                    Cases.Add(new ValidationCase(referencePlanSetups.ElementAt(iCase), testPlanSetups.ElementAt(iCase), referencePlanSetups.ElementAt(iCase).Id));
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
