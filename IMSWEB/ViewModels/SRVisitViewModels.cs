using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class SRVisitViewModel
    {
        public CreateSRVisitDetailViewModel SRVDetail { get; set; }
        public ICollection<CreateSRVisitDetailViewModel> SRVDetails { get; set; }
        public CreateSRVisitViewModel SRVisit { get; set; }
    }

    public class CreateSRVisitDetailViewModel
    {
        public string SRVisitDID { get; set; }

        public string SRVisitID { get; set; }

        [Display(Name = "Product")]
        public string ProductID { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        [Display(Name = "Color")]
        public string ColorID { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get; set; }

        public EnumStatus Status { get; set; }

        public ICollection<SRVProductDetail> SRVProductDetails { get; set; }

    }

    public class CreateSRVisitViewModel
    {
        public string SRVisitID { get; set; }

        [Display(Name = "Challan")]
        public string ChallanNo { get; set; }

        [Display(Name = "Visit. Date")]
        public string VisitDate { get; set; }

        [Display(Name = "Sales Rep.")]
        public string EmployeeID { get; set; }
    }

    public class GetSRVisitViewModel
    {
        public string SRVisitID { get; set; }

        [Display(Name = "Visit Date")]
        public string VisitDate { get; set; }

        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }

        [Display(Name = "Sales Rep.")]
        public string EmpName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        public string Status { get; set; }

    }
}