using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class EmpGradeSalaryAssignmentViewModel : AuditTrailModel
    {
        public EmpGradeSalaryAssignmentViewModel()
        {
            EmpGradeSalaryAssignmentViewModels = new List<EmpGradeSalaryAssignmentViewModel>();
        }

        public string EmpGradeSalaryID { get; set; }
        public string Type { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EmployeeID { get; set; }
        public DateTime EffectDate { get; set; }

        [Display(Name = "Date Range")]
        public string DateRange { get; set; }
        public int GradeSalaryID { get; set; }
        public DateTime? TillDate { get; set; }
        public int? ArrearInfo { get; set; }

        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Grade")]
        public string GradeID { get; set; }

        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        public int? SlabNo { get; set; }
        public int Status { get; set; }

        [Display(Name = "Change Type")]
        public string Gradesalarytypeid { get; set; }

        [Display(Name = "Change Type")]
        public string GradesalarytypeName { get; set; }
        public int ConcernID { get; set; }
        public List<EmpGradeSalaryAssignmentViewModel> EmpGradeSalaryAssignmentViewModels { get; set; }
    }
}