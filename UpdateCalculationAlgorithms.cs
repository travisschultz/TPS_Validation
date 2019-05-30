using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    public static class UpdateCalculationAlgorithms
    {
        public static void Update(VMS.TPS.Common.Model.API.Application app, Patient p, String photonAlg, String electronAlg)
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
							ebps.SetCalculationModel(CalculationType.PhotonVolumeDose, photonAlg);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show($"For some reason the script couldn't update the algorithm:\n{e.Message}");
                        }
					}
				}
            }

			//app.SaveModifications();

			MessageBox.Show("Finished updating photon calculation algorithms.  Unfortunately electron algorithms are not available through scripting and must be changed by hand", "Update Finished", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

    }
}
