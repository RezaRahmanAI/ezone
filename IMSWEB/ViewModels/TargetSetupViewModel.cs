using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class TargetSetupViewModel
    {
        public TargetSetupViewModel()
        {
            Products = new List<GetProductViewModel>();
            CatergoryWiseDetails = new List<CatergoryWiseDetailViewModel>();
            TargetSetupDetailList = new List<GetProductViewModel>();
        }
        public string TID { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        [Display(Name = "Designation")]
        public string DesignationName { get; set; }
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        [Display(Name = "Target Month")]
        public DateTime TargetMonth { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
        public int Status { get; set; }
        [Display(Name = "Category")]
        public string CategoryID { get; set; }
        public IList<GetProductViewModel> Products { get; set; }
        public List<CatergoryWiseDetailViewModel> CatergoryWiseDetails { get; set; }
        public List<GetProductViewModel> TargetSetupDetailList { get; set; }
    }


    public class CatergoryWiseDetailViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal Quantity { get; set; }
    }
}