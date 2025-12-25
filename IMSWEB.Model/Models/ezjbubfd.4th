
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class BankTransaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BankTransaction()
        {
          
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BankTranID { get; set; }
        public DateTime? TranDate { get; set; }   
        public string TransactionNo { get; set; }  
        public int TransactionType { get; set; }
        public int BankID { get; set; }
        public int ConcernID { get; set; }
        public decimal Amount { get; set; }
       // public int ProductType { get; set; }
        public string Remarks { get; set; }

        public virtual Bank Bank { get; set; }

  
    }
}
