using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class DO : AuditTrailModel
    {
        public DO()
        {
            DODetails = new HashSet<DODetail>();
        }

        [Key]
        public int DOID { get; set; }
        public DateTime Date { get; set; }
        public string DONo { get; set; }
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }
        public EnumDOStatus Status { get; set; }

        public string Remarks { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal PaidAmt { get; set; }
        public int ConcernID { get; set; }
        public virtual ICollection<DODetail> DODetails { get; set; }

        public decimal NetDiscount { get; set; }

        public decimal GrandTotal { get; set; }
        public decimal FlatDiscount { get; set; }
        public decimal FlatDiscountPer { get; set; }

    }
}
