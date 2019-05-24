using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    class UpdateCalculationAlgorithms
    {
        Patient _patient;
        String _electronAlgorithm;
        String _photonAlgorithm;

        public UpdateCalculationAlgorithms(Patient p, String photonAlg, String electronAlg)
        {
            foreach (Course c in p.Courses)
            {
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
                    if (ebps.Id.ToCharArray()[0].Equals("T"))
                    {
                        try
                        {
                            ebps.SetCalculationModel(CalculationType.PhotonVolumeDose, photonAlg);
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("For some reason the script couldn't update the algorithm");
                        }
                    }
                }
            }
        }

    }
}
