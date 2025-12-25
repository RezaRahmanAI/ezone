using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
   public interface ITargetSetupRepository
    {
       bool AddTargetSetupUsingSP(DataTable dtTargetSetup, DataTable dtTargetSetupDetails);
       bool UpdateTargetSetupUsingSP(int TID, DataTable dtTargetSetup, DataTable dtTargetSetupDetails);
       bool DeleteTargetSetupUsingSP(int TID);
    }
}
