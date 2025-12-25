using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("menu")]
    public class MenuController : CoreController
    {
        IMenuService _menuService;
        IMapper _mapper;
        public MenuController(IErrorService errorService, IMenuService menuService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var menus = _menuService.GetAllMenuAsync();
            var vmMenus = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string>>,
                IEnumerable<MenuViewModel>>(await menus);
            return View(vmMenus);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new MenuViewModel
            {
                Menus = _menuService.GetAllMenu().Select(m => new SelectListItem { Text = m.Title, Value = m.Id.ToString() }).ToList()
            });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(MenuViewModel newMenu, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                newMenu.Menus = _menuService.GetAllMenu().Select(m => new SelectListItem { Text = m.Title, Value = m.Id.ToString() }).ToList();
                return View(newMenu);
            }

            if (newMenu != null)
            {
                var menu = _mapper.Map<MenuViewModel, MenuItem>(newMenu);
                _menuService.AddMenu(menu);
                _menuService.SaveMenu();

                AddToastMessage("", "Menu has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Menu data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var menu = _menuService.GetMenuById(id);
            var vmodel = _mapper.Map<MenuItem, MenuViewModel>(menu);
            vmodel.Menus = _menuService.GetAllMenu().Select(m => new SelectListItem { Text = m.Title, Value = m.Id.ToString() }).ToList();

            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(MenuViewModel newMenu, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                newMenu.Menus = _menuService.GetAllMenu().Select(m => new SelectListItem { Text = m.Title, Value = m.Id.ToString() }).ToList();
                return View(newMenu);
            }

            if (newMenu != null)
            {
                var existingMenu = _menuService.GetMenuById(int.Parse(newMenu.Id));

                existingMenu.Title = newMenu.Title;
                existingMenu.Description = newMenu.Description;
                existingMenu.ParentId = int.Parse(GetDefaultIfNull(newMenu.ParentId));
                existingMenu.Url = newMenu.Url;
                existingMenu.WithoutView = newMenu.WithoutView;
                existingMenu.Icon = newMenu.Icon;
                existingMenu.Sequence = newMenu.Sequence;
                _menuService.UpdateMenu(existingMenu);
                _menuService.SaveMenu();

                AddToastMessage("", "Menu has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Menu data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _menuService.DeleteMenu(id);
            _menuService.SaveMenu();
            AddToastMessage("", "Menu has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}