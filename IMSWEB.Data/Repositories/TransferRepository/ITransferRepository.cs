using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public interface ITransferRepository
    {
        Tuple<bool, int> AddTransferUsingSP(DataTable dtTransferOrder, DataTable dtDetails, DataTable dtTransferFromStock, DataTable dtTransferToStock);

        bool ReturnTranserferUsingSP(int TransferID);


        //bool CheckTransProductStatusByTransDId(int id);
        int CheckTransProductStatusByTransDId(int id);
        int CheckTransProductHireSalesStatusByTransDId(int id);
    }
}
