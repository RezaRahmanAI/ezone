using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class TransferHistoryViewModel
    {
        public List<CreateTransferHistory> TransferList { get; set; }
        public CreateTransferHistory TransferModel { get; set; }
    }
    public class CreateTransferHistory
    {
        public int TransferHID { get; set; }

        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }
        public int CreatedBy { get; set; }
        public string GodownID { get; set; }

        [Display(Name = "From Godown")]
        public string FromGodownName { get; set; }

        public string ToGodown { get; set; }

        [Display(Name = "To Godown")]
        public string ToGodownName { get; set; }

        [Display(Name="Product")]
        public string ProductId { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

       
        public decimal Quantity { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name="Stock")]
        public decimal PreviousStock { get; set; }
    
    
        public decimal MPRate { get; set; }
    
        public int StockID { get; set; }
        public string StockCode { get; set; }

    }

    public class GetTransferHistory
    {
        public int TransferHID { get; set; }

        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }
        public int CreatedBy { get; set; }
        public string GodownID { get; set; }

        [Display(Name = "From Godown")]
        public string FromGodownName { get; set; }

        public int ToGodown { get; set; }

        [Display(Name = "To Godown")]
        public string ToGodownName { get; set; }

        public string ProductId { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }
      
        public decimal Quantity { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Size")]
        public string ModelName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Stock")]
        public decimal PreviousStock { get; set; }

       

        //public decimal UnitPrice { get; set; }
     

     

    }
}