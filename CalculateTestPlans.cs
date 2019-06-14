using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    public static class CalculateTestPlans
    {
        public static void Calculate(ViewModel vm, Patient p)
        {
            p.BeginModifications();

			foreach (Course c in p.Courses.Where(c => !c.Id.ToLower().Contains("electron")))
			//foreach (Course c in p.Courses)
			{
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
                    if (ebps.Id[0] == 'T')
                    {
						// Find reference plan and save the MU
						ExternalPlanSetup refPlan = c.ExternalPlanSetups.Where(x => x.Id.Substring(1) == ebps.Id.Substring(1)).First();
						List<KeyValuePair<string, MetersetValue>> MUList = new List<KeyValuePair<string, MetersetValue>>();

						foreach(Beam b in refPlan.Beams)
						{
							MUList.Add(new KeyValuePair<string, MetersetValue>(b.Id, b.Meterset));
						}


						try
                        {
                            vm.UpdateStatus($"Calculating Machine: {ebps.Beams.First().TreatmentUnit.Id} Course: {ebps.Course.Id} Plan: {ebps.Id}");
							CalculationResult cr;
							//cr=ebps.CalculateDose();
							cr = ebps.CalculateDoseWithPresetValues(MUList);                     // this might only work for the IMRT plan
                            // System.Windows.MessageBox.Show(cr.ToString());                            
                        }
                        catch(Exception e)
                        {
                            System.Windows.MessageBox.Show($"For some reason the script couldn't calculate the plan: {ebps.Id}\n\n{e.Message}");
                        }
                    }
                }
            }

            System.Windows.MessageBox.Show("Completed Calculating the plans");

            vm.App.SaveModifications(); //!!! modified line when changed to passing view model. Untested, could cause issues.
            
        }
    }
}
