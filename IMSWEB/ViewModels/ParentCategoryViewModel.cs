using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class ParentCategoryViewModel
    {
        public int? PCategoryID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}