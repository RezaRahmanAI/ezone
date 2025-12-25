
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SalaryMonthlyDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SMDetailID { get; set; }
        public decimal ChangedAmount { get; set; }
        public int SalaryMonthlyID { get; set; }
        public int ItemCode { get; set; }
        public int ItemGroup { get; set; }
        public int ItemID { get; set; }
        public int SupportID { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }
        public decimal CalculatedAmount { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual SalaryMonthly SalaryMonthly { get; set; }
    }
}



