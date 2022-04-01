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

			//foreach (Course c in p.Courses.Where(c => !c.Id.ToLower().Contains("electron")))	//can't calc electrons in this version
            foreach (Course c in p.Courses.Where(c => c.Id.ToLower().Contains("photon") || c.Id.ToLower().Contains("dynamic")))
			//foreach (Course c in p.Courses)
			{
                foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                {
					// only calculate the test plans
                    if (ebps.Id[0] == 'T')
                    {
						// Find reference plan and save the MU
						ExternalPlanSetup refPlan = c.ExternalPlanSetups.Where(x => x.Id.Split('_')[0] == "R" + ebps.Id.Split('_')[0].Substring(1)).First();
						List<KeyValuePair<string, MetersetValue>> MUList = new List<KeyValuePair<string, MetersetValue>>();

						foreach(Beam b in refPlan.Beams)
						{
							MUList.Add(new KeyValuePair<string, MetersetValue>(b.Id, b.Meterset));
						}

						try
                        {
                            vm.UpdateStatus($"Calculating Machine: {ebps.Beams.First().TreatmentUnit.Id} Course: {ebps.Course.Id} Plan: {ebps.Id}");
							//cr=ebps.CalculateDose();
							ebps.CalculateDoseWithPresetValues(MUList);        // this might only work for the IMRT plan         
							
							//loop through each beam and adjust the MUs to match the reference plan
							foreach (Beam b in ebps.Beams.Where(x => !x.IsSetupField))
							{
								BeamParameters beamParams = b.GetEditableParameters();
								//beamParams.WeightFactor = b.WeightFactor * 100.0 / b.Meterset.Value;  //originally was this to make each beam 100.0 MU
								beamParams.WeightFactor = b.WeightFactor * MUList.Where(x => x.Key == b.Id).First().Value.Value / b.Meterset.Value;	//this should now make each beam whatever the previous value was
								b.ApplyParameters(beamParams);
							}
                        }
                        catch(Exception e)
                        {
                            System.Windows.MessageBox.Show($"For some reason the script couldn't calculate the plan: {ebps.Id}\n\n{e.Message}");
                        }
                    }
                }
            }

            vm.App.SaveModifications(); 
            
        }
    }
}
