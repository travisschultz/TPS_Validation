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
            //p.BeginModifications();

            foreach (Course c in p.Courses)
            {
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
                    if (ebps.Id[0] == 'T')
                    {
                        try
                        {
                            vm.UpdateStatus($"Calculating course: {ebps.Course.Id} plan: {ebps.Id}");
                            CalculationResult cr;
                            cr=ebps.CalculateDose();
                            // System.Windows.MessageBox.Show(cr.ToString());
                            
                        }
                        catch(Exception e)
                        {
                            System.Windows.MessageBox.Show($"For some reason the script couldn't calculate the plan: {ebps.Id}\n{e.Message}");
                        }
                    }
                }
            }

            System.Windows.MessageBox.Show("Completed Calculating the plans");

            //vm.App.SaveModifications(); //!!! modified line when changed to passing view model. Untested, could cause issues.
            
        }
    }
}
