using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using IMSWEB.Model;
using IMSWEB.Service;
using Autofac;
using IMSWEB.Data;
using Microsoft.AspNet.Identity;

namespace IMSWEB
{
    public static class HtmlNavigationMenuExtension
    {
        static List<MenuItem> _menuItems;
        public static string BuildMenuNavigation(this HtmlHelper helper)
        {
            if (helper.ViewContext.RouteData.Values["controller"].ToString() == "Account" &&
                helper.ViewContext.RouteData.Values["action"].ToString() == "Login")
            {
                return "<br/><br/>";
            }

            if (HttpContext.Current.Session["NavigationMenu"] == null)
                GetMenuByUserRole();

            _menuItems = (List<MenuItem>)HttpContext.Current.Session["NavigationMenu"];
            StringBuilder navigationMenu = new StringBuilder();
            List<MenuItem> rootParents = _menuItems.FindAll(x => x.ParentId == default(int)).OrderBy(i => i.Sequence).ToList();

            foreach (var item in rootParents)
            {
                navigationMenu.Append("<li>").
                    Append("<a id='id" + item.Id + "'  href='#" + item.Id + "' data-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>" + "<div>" + item.Icon + "</div>" + item.Title + "</a>");
                GenerateNavigationMenu(item, navigationMenu, true);
                navigationMenu.Append("</li>");
            }

            return navigationMenu.Append("</ul>").ToString();
        }

        private static void GetMenuByUserRole()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DbFactory>().As<IDbFactory>();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>));
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            IContainer container = builder.Build();

            RoleService roleService = new RoleService(container.Resolve<IBaseRepository<ApplicationRole>>(),
                container.Resolve<IBaseRepository<ApplicationUserRole>>(), container.Resolve<IUnitOfWork>());
            MenuService menuService = new MenuService(container.Resolve<IBaseRepository<MenuItem>>(),
                container.Resolve<IBaseRepository<ApplicationRoleMenu>>(), container.Resolve<IUnitOfWork>());

            int userId = HttpContext.Current.User.Identity.GetUserId<int>();
            var roles = roleService.GetUserRoleByUserId(userId);
            var roleIds = roles.Select(x => x.RoleId).ToList();
            var menus = menuService.GetMenuByUserRole(roleIds);
            HttpContext.Current.Session["NavigationMenu"] = menus;
        }

        private static void GenerateNavigationMenu(MenuItem menuItem, StringBuilder navigationMenu, bool rootParent)
        {
            if (!rootParent && HasSecondSubMenu(menuItem))
            {
                if (!menuItem.WithoutView)
                    navigationMenu.Append("<li>").
                    Append("<a id='id" + menuItem.Id + "'  href='#" + menuItem.Id + "' data-toggle='collapse' aria-expanded='false' class='dropdown-toggle'><i class='fa fa-angle-right'></i>" + menuItem.Title + "</a>");
                else
                    navigationMenu.Append("<li>").
                        Append("<a data-ajax='true' data-ajax-mode='replace' data-ajax-update='#report' href='" + menuItem.Url + "'" + menuItem.Description + ">" + menuItem.Title + "</a>");
            }
            else if (!rootParent)
            {
                if (!menuItem.WithoutView)
                    navigationMenu.Append("<li>").
                        Append("<a id='id" + menuItem.Id + "' href='" + menuItem.Url + "'" + menuItem.Description + ">" + "<div>" + menuItem.Icon + "</div>" + menuItem.Title + "</a>");
                else
                    navigationMenu.Append("<li>").
                        Append("<a data-ajax='true' data-ajax-mode='replace' data-ajax-update='#report' href='" + menuItem.Url + "'" + menuItem.Description + ">" + menuItem.Title + "</a>");
            }

            List<MenuItem> childMenus = _menuItems.FindAll(x => x.ParentId == menuItem.Id).OrderBy(i => i.Sequence).ToList();
            if (childMenus != null && childMenus.Count > 0)
            {
                navigationMenu.Append("<ul class='collapse list-unstyled' id='" + menuItem.Id + "'>");
                foreach (var item in childMenus)
                {
                    GenerateNavigationMenu(item, navigationMenu, false);
                    //navigationMenu.Append("<li class='divider'></li>");
                }
                navigationMenu.Append("</ul>");
            }

            navigationMenu.Append("</li>");
        }

        private static bool HasSecondSubMenu(MenuItem menuItem)
        {
            return _menuItems.Find(x => x.ParentId == menuItem.Id) != null;
        }

        private static bool IsInThirdORLastNode(MenuItem menuItem)
        {
            var child = _menuItems.Find(x => x.ParentId == menuItem.Id);
            return child == null || _menuItems.Find(x => x.ParentId == child.Id) == null;
        }
    }
}