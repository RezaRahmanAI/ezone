using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<ApplicationRole> _roleRepository;
        private readonly IBaseRepository<ApplicationUserRole> _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IBaseRepository<ApplicationRole> roleRepository,
            IBaseRepository<ApplicationUserRole> userRoleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ApplicationRole> GetAllRole()
        {
            return _roleRepository.All.ToList<ApplicationRole>();
        }

        public void AddRole(ApplicationRole role)
        {
            _roleRepository.Add(role);
        }

        public void UpdateRole(ApplicationRole role)
        {
            _roleRepository.Update(role);
        }

        public void SaveRole()
        {
            _unitOfWork.Commit();
        }

        public ApplicationRole GetRoleById(int id)
        {
            return _roleRepository.FindBy(x => x.Id == id).First();
        }

        public IEnumerable<ApplicationUserRole> GetUserRoleByUserId(int id)
        {
            return _userRoleRepository.FindBy(x => x.UserId == id);
        }

        public int GetRoleByUserId(int id)
        {
            var data = _userRoleRepository.All.Where(d => d.UserId == id).OrderByDescending(d => d.RoleId).FirstOrDefault();
            return data != null ? data.RoleId : 0;
        }

        public void DeleteRole(int id)
        {
            _roleRepository.Delete(x => x.Id == id);
        }

       

    }
}
