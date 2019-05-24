using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    class CalculateTestPlans
    {
        public CalculateTestPlans(Patient p)
        {
            foreach (Course c in p.Courses)
            {
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
                    if (ebps.Id.ToCharArray()[0].Equals("T"))
                    {
                        try
                        {
                            ebps.CalculateDose();
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("For some reason the script couldn't recalculate the dose");
                        }
                    }
                }
            }

        }
    }
}
