using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            Customers = new HashSet<Customer>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(150)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string FName { get; set; }

        public string MName { get; set; }

        [StringLength(150)]
        public string ContactNo { get; set; }

        [StringLength(150)]
        public string EmailID { get; set; }

        [StringLength(150)]
        public string NID { get; set; }

        [StringLength(50)]
        public string BloodGroup { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string PresentAdd { get; set; }

        public string PermanentAdd { get; set; }

        public int DesignationID { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }

        [StringLength(250)]
        public string PhotoPath { get; set; }

        public DateTime? DOB { get; set; }

        public decimal SRDueLimit { get; set; }

        public int ConcernID { get; set; }
        public int? DepartmentID { get; set; }
        public int? LocationID { get; set; }
        public int? ReligionID { get; set; }
        public int? GradeID { get; set; }
        public int? PaymentMode { get; set; }
        public int? IsEligibleOT { get; set; }
        public DateTime? EndOfContractDate { get; set; }
        public DateTime? DateOfConfirmation { get; set; }
        public int MachineEMPID { get; set; }
        public EnumActiveInactive Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }

        public virtual Designation Designation { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
        public decimal AdvanceLoanAmt { get; set; }
    }
}
