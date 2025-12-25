using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO
{
    public class BankLoanCollectionTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Paid Date")]
        public DateTime CollectionDate { get; set; }
        [Display(Name = "Paid Amount")]
        public decimal CollectionAmount { get; set; }
        [Display(Name = "Loan Code")]
        public string LoanCode { get; set; }
        [Display(Name = "Bank")]
        public string BankName { get; set; }
        public string CollectionType { get; set; }
    }
}
