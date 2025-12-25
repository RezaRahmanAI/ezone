using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IMSWEB.Model;


namespace IMSWEB
{

    public class SalaryMonthlyViewModel
    {
      
    }
    public class GetSalaryMonthlyViewModel
    {
        public int SalaryMonthlyID { get; set; }
        public int DepartmentID { get; set; }
        public int LocationID { get; set; }
        public int GradeID { get; set; }
        public int CategoryID { get; set; }
        public decimal PrevMonthBasic { get; set; }
        public decimal ThisMonthBasic { get; set; }
        public int IsFinalized { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Remarks { get; set; }
        public string AccountNo { get; set; }
        public int BranchID { get; set; }
        public int PFMemberType { get; set; }
        public int Gender { get; set; }
        public int IsConfirmed { get; set; }
        public int IsEligibleOT { get; set; }
        public int ReligionID { get; set; }
        public int WithNoDetail { get; set; }
        public string Designation { get; set; }
        public DateTime SalaryMonth { get; set; }
        public decimal ThisMonthGross { get; set; }
        public int SalaryProcessID { get; set; }
        public int ConcernID { get; set; }
    }
    public class CreateSalaryMonthlyViewModel
    {
        public int SalaryMonthlyID { get; set; }
        public int DepartmentID { get; set; }
        public int LocationID { get; set; }
        public int GradeID { get; set; }
        public int CategoryID { get; set; }
        public decimal PrevMonthBasic { get; set; }
        public decimal ThisMonthBasic { get; set; }
        public int IsFinalized { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Remarks { get; set; }
        public string AccountNo { get; set; }
        public int BranchID { get; set; }
        public int PFMemberType { get; set; }
        public int Gender { get; set; }
        public int IsConfirmed { get; set; }
        public int IsEligibleOT { get; set; }
        public int ReligionID { get; set; }
        public int WithNoDetail { get; set; }
        public string Designation { get; set; }
        public DateTime SalaryMonth { get; set; }
        public decimal ThisMonthGross { get; set; }
        public int SalaryProcessID { get; set; }
        public int ConcernID { get; set; }                                            

    }


}