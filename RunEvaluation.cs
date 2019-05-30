using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace TPS_Validation
{
    class RunEvaluation
    {
        private List<Machine> _machineValidations;
        public Machine m;

        public Machine RunEvaluationOnPatient(Patient p)
        {
            m.MachineID = p.FirstName;
            foreach (Course c in p.Courses)
            {
                m.Groups.Add(new ValidationGroup(c)); // Validation group constructor to read (for ease)


                //foreach (ExternalPlanSetup ebps in c.ExternalPlanSetups)
                //{

                    
                //}
            }
            return m;

        }

    }
}
