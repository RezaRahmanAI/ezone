using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }
        [Display(Name = "Department Code")]

        public string CODE { get; set; }
        [Display(Name = "Department Name")]
        public string DESCRIPTION { get; set; }
        public int SequenceNO { get; set; }
        public EnumActiveInactive Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}