using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IRoleService
    {
        void AddRole(ApplicationRole role);
        void UpdateRole(ApplicationRole role);
        void SaveRole();
        IEnumerable<ApplicationRole> GetAllRole();
        ApplicationRole GetRoleById(int id);
        IEnumerable<ApplicationUserRole> GetUserRoleByUserId(int id);
        void DeleteRole(int id);
        int GetRoleByUserId(int id);
    }
}
