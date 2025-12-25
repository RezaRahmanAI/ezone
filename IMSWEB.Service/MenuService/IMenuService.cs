using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IMenuService
    {
        void AddMenu(MenuItem menu);
        void AddRoleMenu(ApplicationRoleMenu menu);
        MenuItem GetMenuById(int id);
        IEnumerable<MenuItem> GetAllMenu();
        IEnumerable<ApplicationRoleMenu> GetAllRoleMenu();
        IEnumerable<ApplicationRoleMenu> GetRoleMenuByRoleId(int roleId);
        Task<IEnumerable<Tuple<int, string, string, string, string>>> GetAllMenuAsync();
        IEnumerable<MenuItem> GetMenuByUserRole(ICollection<int> roleIds);
        void UpdateMenu(MenuItem menu);
        void DeleteRoleMenuByRoleId(int id);
        void DeleteMenu(int id);
        void SaveMenu();
    }
}
