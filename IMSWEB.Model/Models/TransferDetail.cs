using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TransferDetail
    {
        [Key]
        public int TDetailID { get; set; }
        public int ProductID { get; set; }
        public int ToProductID { get; set; }
        public int ToGodownID { get; set; }
        public int ToColorID { get; set; }
        public decimal PRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UTAmount { get; set; }
        public Transfer Transfer { get; set; }
        public int TransferID { get; set; }
        public int SDetailID { get; set; }
        public string IMEI { get; set; }
        public decimal SRate { get; set; }
    }
}
