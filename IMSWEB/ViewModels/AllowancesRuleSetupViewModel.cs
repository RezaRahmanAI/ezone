using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateAllowancesRuleSetupViewModel
    {
        public ADParameterBasicCreate ADParameterBasicCreate { get; set; }
        public ADParameterEmployeeCreate ADParameterEmployeeCreate { get; set; }
        public ADParameterGradeCreate ADParameterGradeCreate { get; set; }
        public List<ADParameterGradeCreate> ADParameterGradeCreateList { get; set; }
        public List<GradeViewModel> Grades { get; set; }
    }

    public class ADParameterGradeCreate
    {
        public int ADParameterID { get; set; }
        public int GradeID { get; set; }
    }
    public class ADParameterBasicCreate
    {
        public int ADParameterID { get; set; }
        public string AllowDeductID { get; set; }

        [Display(Name = "Allowances/Deductions")]
        public string AllowDeductName { get; set; }
        public int IsConfirmOnly { get; set; }

        [Display(Name = "Flat Amount")]
        public decimal FlatAmount { get; set; }

        [Display(Name = "% of Basic")]
        public decimal PercentOfBasic { get; set; }
        public EnumPeriodicity Periodicity { get; set; }

        [Display(Name = "Periodicity")]
        public string PeriodicityName { get; set; }
        public bool IsIndividual { get; set; }
        public int EntitleType { get; set; }

        [Display(Name = "Type")]
        public string EntitleTypeName { get; set; }

        [Display(Name = "Grades")]
        public string GradeName { get; set; }
        public int IsCurrentlyActive { get; set; }
        public System.DateTime LastActivationDate { get; set; }
        public Nullable<System.DateTime> LastDeactivationDate { get; set; }
        public int AllowOrDeduct { get; set; }
        public int IsTaxable { get; set; }
        public string IsTaxableName { get; set; }
        public decimal TaxablePercentage { get; set; }
        public int IsDependsOnAttendance { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> TaxProjection { get; set; }
        public int Gender { get; set; }
        public int PayrollTypeID { get; set; }
        public Nullable<int> PeriodicValue { get; set; }
        public int IsDependsOnSpecialHour { get; set; }
        public int IsWithException { get; set; }
        public Nullable<int> IsEditable { get; set; }

        [Display(Name="% of Gross")]
        public decimal PercentOfGross { get; set; }
        public int Status { get; set; }
        public decimal ISTaxProjection { get; set; }
        public int IsDependsOnOTHour { get; set; }
        public decimal PercentOfEarnedBasic { get; set; }
        public int IsFractionateApplicable { get; set; }
        public int SequenceNO { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int ConcernID { get; set; }
        public List<string> GradeList { get; set; }

    }


    public class ADParameterEmployeeCreate
    {

        public string ADParameterEmpID { get; set; }
        public System.DateTime EntryDate { get; set; }
        public Nullable<int> UserID { get; set; }
        public int ADParameterID { get; set; }
        public int AllowDeductID { get; set; }
        public int Periodicity { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> TillDate { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DisbursedAmount { get; set; }
        public int ArrearInfo { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ADEmpType { get; set; }
        public Nullable<int> ValueType { get; set; }

    }
}