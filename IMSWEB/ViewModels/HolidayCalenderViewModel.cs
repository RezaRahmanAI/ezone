using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class HolidayCalenderViewModel
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public EnumHolidayType Type { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ConcernID { get; set; }

        [Display(Name = "Select")]
        public bool IsSelect { get; set; }
        public bool IsAssigned { get; set; }
    }
}