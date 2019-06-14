using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    public static class RunEvaluation
    {
        //private List<Machine> _machineValidations;
        //public Machine m;

        public static Machine RunEvaluationOnPatient(Patient p)
        {
            Machine m = new Machine(p);
            foreach (Course c in p.Courses)
            {

                // Validation group constructor to read (for ease)
                // Constructor will also figure out whether or not they are plan or field based verifications

                m.Groups.Add(new ValidationGroup(c, m));
               


                //foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                //{

                    
                //}
            }
            return m;

        }

    }
}
