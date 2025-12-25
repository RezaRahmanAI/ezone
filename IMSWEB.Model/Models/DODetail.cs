using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class DODetail
    {
        [Key]
        public int DODID { get; set; }
        public Product Product { get; set; }
        public int ProductID { get; set; }
        public decimal DOQty { get; set; }
        public decimal GivenQty { get; set; }
        public decimal MRP { get; set; }
        public decimal TotalAmt { get; set; }

        public int ColorID { get; set; }
        public virtual DO DO { get; set; }
        public int DOID { get; set; }
        public decimal DDLiftingPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PPDisPercent { get; set; }
        public decimal PPDisAmt { get; set; }

    }
}

