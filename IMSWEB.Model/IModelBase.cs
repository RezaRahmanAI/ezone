using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public interface IModelBase
    {
        int CreatedBy { get; set; }

        DateTime CreateDate { get; set; }

        int? ModifiedBy { get; set; }

        DateTime? ModifiedDate { get; set; }
    }

    public interface IBankPayment
    {
         int BankTransID { get; set; }
         decimal CardPaidAmount { get; set; }
         int CardTypeSetupID { get; set; }
         decimal DepositChargePercent { get; set; }
    }
}
