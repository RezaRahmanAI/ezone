using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TransferHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int TransferHID { get; set; }  
        public DateTime TransferDate { get; set; }
        public int CreatedBy { get; set; }
        public int FromGodown { get; set; }    
        public int ToGodown { get; set; }   
        public int ProductId { get; set; }
        public decimal Qty { get; set; }  
        public int FromSDetailID { get; set; }
        public int ToSDetailID { get; set; }
        public int POrderDetailID { get; set; }
        public int ConcernID { get; set; }
       
    }
}
