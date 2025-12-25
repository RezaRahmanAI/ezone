using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUserAsync();
        ApplicationUser GetUserById(int id);
        string GetUserNameById(int id);

        int GetUserIDByEmployeeID(int EmployeeID);
    }
}
