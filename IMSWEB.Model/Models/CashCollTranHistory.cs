using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    [Table("CashCollTranHistories")]
    public partial class CashCollTranHistory 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CashCollTranHisID { get; set; }
        public int CashCollectionID { get; set; }
        public string ReceiptNo { get; set; }
        public string CreateOrEdit { get; set; }
        public string Value { get; set; }
        public int ConcernID { get; set; }
        public int CreateOrEditBy { get; set; }
        public DateTime HistoryDate { get; set; }
    }
}
