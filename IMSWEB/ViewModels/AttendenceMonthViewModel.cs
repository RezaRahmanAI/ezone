using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class AttendenceMonthViewModel
    {
        public AttendenceMonthViewModel()
        {
            this.Attendences = new List<AttendenceViewModel>();
        }

        public int AttenMonthID { get; set; }
        public System.DateTime AttendencMonth { get; set; }
        public int IsFinalize { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int ConcernID { get; set; }

        public string IP { get; set; }
        public string PortNo { get; set; }
        public string MachineNo { get; set; }

        public virtual List<AttendenceViewModel> Attendences { get; set; }
    }
}