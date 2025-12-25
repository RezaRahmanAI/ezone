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
    public class MenuService : IMenuService
    {
        private readonly IBaseRepository<MenuItem> _menuItemRepository;
        private readonly IBaseRepository<ApplicationRoleMenu> _roleMenuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IBaseRepository<MenuItem> menuItemRepository,
            IBaseRepository<ApplicationRoleMenu> roleMenuRepository, IUnitOfWork unitOfWork)
        {
            _menuItemRepository = menuItemRepository;
            _roleMenuRepository = roleMenuRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddMenu(MenuItem menu)
        {
            _menuItemRepository.Add(menu);
        }

        public void AddRoleMenu(ApplicationRoleMenu menu)
        {
            _roleMenuRepository.Add(menu);
        }

        public IEnumerable<MenuItem> GetAllMenu()
        {
            return _menuItemRepository.All;
        }

        public IEnumerable<ApplicationRoleMenu> GetAllRoleMenu()
        {
            return _roleMenuRepository.All;
        }

        public IEnumerable<ApplicationRoleMenu> GetRoleMenuByRoleId(int roleId)
        {
            return _roleMenuRepository.FindBy(x=>x.RoleId == roleId);
        }

        public async Task<IEnumerable<Tuple<int, string, string, string, string>>> GetAllMenuAsync()
        {
            return await _menuItemRepository.GetAllMenuAsync();
        }

        public MenuItem GetMenuById(int id)
        {
            return _menuItemRepository.FindBy(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<MenuItem> GetMenuByUserRole(ICollection<int> roleIds)
        {
            return _menuItemRepository.GetMenuByUserRole(_roleMenuRepository, roleIds);
        }

        public void UpdateMenu(MenuItem menu)
        {
            _menuItemRepository.Update(menu);
        }

        public void DeleteRoleMenuByRoleId(int id)
        {
            _roleMenuRepository.Delete(x=>x.RoleId == id);
        }

        public void DeleteMenu(int id)
        {
            _menuItemRepository.Delete(x => x.Id == id);
        }

        public void SaveMenu()
        {
            _unitOfWork.Commit();
        }
    }
}
