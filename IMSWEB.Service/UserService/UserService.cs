using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<ApplicationUser> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IBaseRepository<ApplicationUser> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUserAsync()
        {
            return await _userRepository.GetAllUserAsync();
        }

        public ApplicationUser GetUserById(int id)
        {
            return _userRepository.FindBy(x => x.Id == id).FirstOrDefault();
        }

        public int GetUserIDByEmployeeID(int EmployeeID)
        {
            int UserID = 0;
            var user = _userRepository.FindBy(x => x.EmployeeID == EmployeeID).FirstOrDefault();
            if (user != null)
                UserID = user.Id;
            return UserID;
        }

        public string GetUserNameById(int id)
        {
            string UserID = "";
            var user = _userRepository.FindBy(x => x.Id == id).FirstOrDefault();
            if (user != null)
                UserID = user.UserName;
            return UserID;
        }
    }
}
