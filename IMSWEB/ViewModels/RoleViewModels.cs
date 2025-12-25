using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateRoleViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string MenuId { get; set; }

        public IEnumerable<MenuViewModel> Menus { get; set; }
    }
}