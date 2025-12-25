using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class TransferViewModel
    {
        public TransferViewModel()
        {
            IMEIList = new List<ProductDetailsModel>();
        }
        public GetTransferViewModel Transfer { get; set; }
        public List<TransferDetailViewModel> Details { get; set; }
        public TransferDetailViewModel Detail { get; set; }
        public List<ProductDetailsModel> IMEIList { get; set; }

    }
    public class GetTransferViewModel
    {
        public GetTransferViewModel()
        {
            SisterConcerns = new List<SisterConcern>();
        }
        public int TransferID { get; set; }

        [Display(Name = "Transfer No")]
        public string TransferNo { get; set; }

        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "To Concern")]
        public int ToConcernID { get; set; }

        [Display(Name = "To Concern")]
        public string ToConcernName { get; set; }
        public int FromConcernID { get; set; }
        public EnumTransferStatus Status { get; set; }
        public List<SisterConcern> SisterConcerns { get; set; }

        [Display(Name = "From Concern")]
        public string FromConcern { get; set; }
    }

    public class TransferDetailViewModel
    {
        public int TDetailID { get; set; }
        [Display(Name = "Product")]
        public int ProductID { get; set; }

        [Display(Name = "To Product")]
        public int ToProductID { get; set; }

        [Display(Name = "Godown")]
        public int ToGodownID { get; set; }

        [Display(Name = "Color")]
        public int ToColorID { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }
        public decimal PRate { get; set; }
        public decimal Quantity { get; set; }

        [Display(Name = "Total Amt")]
        public decimal UTAmount { get; set; }
        public int SDetailID { get; set; }
        public EnumStatus Status { get; set; }

        [Display(Name = "IMEI")]
        public string IMENo { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        public string ProductCode { get; set; }
    }
}