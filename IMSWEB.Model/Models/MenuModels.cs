using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ApplicationRoleMenu
    {
        public ApplicationRoleMenu()
        {
            Permissions = new HashSet<MenuPermission>();
        }

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        [Key, Column(Order = 2)]
        public int MenuId { get; set; }

        public ICollection<MenuPermission> Permissions { get; set; }
    }

    public class MenuItem
    {

        public MenuItem()
        {
            WithoutView = false;
        }

        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string Title { get; set; }

        [Column(Order = 3)]
        public string Description { get; set; }

        [Column(Order = 4)]
        public int ParentId { get; set; }

        [Column(Order = 5)]
        public string Url { get; set; }

        [Column(Order = 6)]
        public bool WithoutView { get; set; }
        public string Icon { get; set; }
        public int Sequence { get; set; }

    }

    public class MenuPermission
    {
        [Key, Column(Order = 1)]
        public int RoleMenuId { get; set; }

        [Key, Column(Order = 2)]
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual ApplicationRoleMenu RoleMenu { get; set; }
    }

    public class Permission
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }
    }
}
