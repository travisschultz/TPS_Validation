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
        public static void Update(VMS.TPS.Common.Model.API.Application app, Patient p, String photonAlg, String electronAlg, String acurosAlg)
        {
			p.BeginModifications();

			//foreach (Course c in p.Courses)
			foreach (Course c in p.Courses.Where(c => !c.Id.ToLower().Contains("electron")))
			{
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
                    if (ebps.Id[0] == 'T')
                    {
                        try
						{
							// check if it's an Acuros or normal photon plan and update accordingly
							if (ebps.Id.Split('_')[1] == "AXB")
							{
								ebps.SetCalculationModel(CalculationType.PhotonVolumeDose, acurosAlg);
								ebps.SetCalculationOption(acurosAlg, "DoseReportingMode", "Dose to medium");
								ebps.SetCalculationOption(acurosAlg, "PlanDoseCalcuation", "OFF");
							}
							else
								ebps.SetCalculationModel(CalculationType.PhotonVolumeDose, photonAlg);

							//ebps.SetCalculationOption(electronAlg, "SmoothingMethod", "3-D_Gaussian");
							//ebps.SetCalculationOption(electronAlg, "SmoothingLevel", "Medium");
						}
						catch (Exception e)
                        {
                            MessageBox.Show($"For some reason the script couldn't update the algorithm:\n{e.Message}");
                        }
					}
				}
            }

			app.SaveModifications();
        }

    }
}
