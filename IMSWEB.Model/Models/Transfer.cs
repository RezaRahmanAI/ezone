using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Transfer : AuditTrailModel
    {
        public Transfer()
        {
            TransferDetails = new HashSet<TransferDetail>();
        }
        [Key]
        public int TransferID { get; set; }
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public int ToConcernID { get; set; }
        public int FromConcernID { get; set; }
        public int Status { get; set; }
        public SisterConcern SisterConcern{ get; set; }
        public int ConcernID { get; set; }

        public ICollection<TransferDetail> TransferDetails{ get; set; }
    }
}