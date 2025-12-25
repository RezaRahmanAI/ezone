using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("role")]
    public class RoleController : CoreController
    {
        IRoleService _roleService;
        IMenuService _menuService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;

        public RoleController(IErrorService errorService, IRoleService roleService,
            IMenuService menuService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _roleService = roleService;
            _menuService = menuService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var roles = _roleService.GetAllRole();
            var vm = _mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<CreateRoleViewModel>>(roles);
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public async Task<ActionResult> Create()
        {
            var menus = _menuService.GetAllMenuAsync();
            return View(new CreateRoleViewModel
            {
                Menus = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string>>, IEnumerable<MenuViewModel>>(await menus)
            });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreateRoleViewModel newRole, FormCollection formCollection, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var menus = _menuService.GetAllMenuAsync();
                newRole.Menus = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string>>, IEnumerable<MenuViewModel>>(await menus);
                return View(newRole);
            }

            if (newRole != null)
            {
                var role = _mapper.Map<CreateRoleViewModel, ApplicationRole>(newRole);
                _roleService.AddRole(role);
                _roleService.SaveRole();

                if (!string.IsNullOrEmpty(formCollection["MenuId[]"]))
                    SaveRoleMenu(formCollection["MenuId[]"], role.Id);

                AddToastMessage("", "Role has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Role data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var role = _roleService.GetRoleById(id);
            var menus = _menuService.GetAllMenuAsync();
            var vmodel = _mapper.Map<ApplicationRole, CreateRoleViewModel>(role);
            vmodel.Menus = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string>>, IEnumerable<MenuViewModel>>(await menus);
            GetRoleMenu(vmodel);

            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public async Task<ActionResult> Edit(CreateRoleViewModel newRole, FormCollection formCollection, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                var menus = _menuService.GetAllMenuAsync();
                newRole.Menus = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string>>, IEnumerable<MenuViewModel>>(await menus);
                GetRoleMenu(newRole);
                return View("Create", newRole);
            }

            if (newRole != null)
            {
                var role = _roleService.GetRoleById(int.Parse(newRole.Id));
                role.Name = newRole.Name;

                _roleService.UpdateRole(role);
                _roleService.SaveRole();

                DeleteRoleMenu(role.Id);
                if (!string.IsNullOrEmpty(formCollection["MenuId[]"]))
                    SaveRoleMenu(formCollection["MenuId[]"], role.Id);

                AddToastMessage("", "Role has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Role data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            DeleteRoleMenu(id);
            _roleService.DeleteRole(id);
            _roleService.SaveRole();
            AddToastMessage("", "Role has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void GetRoleMenu(CreateRoleViewModel vmodel)
        {
            var roleMenu = _menuService.GetRoleMenuByRoleId(int.Parse(vmodel.Id));
            vmodel.MenuId = string.Join(",", roleMenu.Select(x => x.MenuId));
        }

        private void DeleteRoleMenu(int id)
        {
            _menuService.DeleteRoleMenuByRoleId(id);
            _menuService.SaveMenu();
        }

        private void SaveRoleMenu(string menuId, int roleId)
        {
            ApplicationRoleMenu roleMenu = null;
            string[] menuIdArr = menuId.Split(',');

            for (int i = 0; i < menuIdArr.Length; i++)
            {
                roleMenu = new ApplicationRoleMenu();
                roleMenu.RoleId = roleId;
                roleMenu.MenuId = int.Parse(menuIdArr[i]);
                _menuService.AddRoleMenu(roleMenu);
            }
            _menuService.SaveMenu();
        }
    }
}