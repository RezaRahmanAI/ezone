using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Customer()
        //{
        //    CashCollections = new HashSet<CashCollection>();
        //    CreditSales = new HashSet<CreditSale>();
        //    SOrders = new HashSet<SOrder>();
        //}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }


      
        [StringLength(50)]
        public string MemberID { get; set; }

        [Required]
        public string Name { get; set; }

        public string FName { get; set; }

        public string CompanyName { get; set; }

        [Required]
        [StringLength(150)]
        public string ContactNo { get; set; }

        [StringLength(250)]
        public string EmailID { get; set; }

        [StringLength(150)]
        public string NID { get; set; }

        public string Address { get; set; }

        [StringLength(150)]
        public string PhotoPath { get; set; }

        public decimal OpeningDue { get; set; }
        public decimal TotalDue { get; set; }
        public decimal CreditDue { get; set; }
        public string RefName { get; set; }

        [StringLength(150)]
        public string RefContact { get; set; }

        public string RefFName { get; set; }
        public string Profession { get; set; }

        public string RefAddress { get; set; }

        public EnumCustomerType CustomerType { get; set; }

        public int EmployeeID { get; set; }

        public decimal CusDueLimit { get; set; }

        public int ConcernID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? RemindDate { get; set; }
        public string Remarks { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CashCollection> CashCollections { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CreditSale> CreditSales { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<SOrder> SOrders { get; set; }
    }
}
