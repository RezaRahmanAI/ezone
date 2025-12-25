using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class GradeViewModel
    {
        public int GradeID { get; set; }

        [Required(ErrorMessage = "Description is Required.")]
        [StringLength(500)]
        [Display(Name="Name")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Code is Required.")]
        [StringLength(250)]
        public string Code { get; set; }
        public int UserID { get; set; }
        public int PayrollTypeID { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
        [Display(Name="Gross Applicable")]
        public bool HasGrossConcept { get; set; }
        [Display(Name = "Basic % of Gross")]
        public decimal BasicPercentOfGross { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsSelected { get; set; }
    }
}