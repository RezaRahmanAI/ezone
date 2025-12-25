using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class MenuExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string, string, string>>>
            GetAllMenuAsync(this IBaseRepository<MenuItem> menuRepository)
        {
            IEnumerable<MenuItem> menus = menuRepository.All;

            var items = await menuRepository.All.GroupJoin(menus,
                m => m.ParentId, pm => pm.Id, (m, pm) => new { Menu = m, ParentMenu = pm }).
                SelectMany(pm => pm.ParentMenu.DefaultIfEmpty(), (m, pm) => new { Menu = m.Menu, ParentMenu = pm }).
                Select(x => new
                {
                    MenuId = x.Menu.Id,
                    MenuTitle = x.Menu.Title,
                    MenuDescription = x.Menu.Description,
                    MenuUrl = x.Menu.Url,
                    Parent = x.ParentMenu.Title
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, string, string>
                (
                    x.MenuId,
                    x.MenuTitle,
                    x.MenuDescription,
                    x.MenuUrl,
                    x.Parent
                )).ToList();
        }

        public static IEnumerable<MenuItem> GetMenuByUserRole(this IBaseRepository<MenuItem> menuRepository,
            IBaseRepository<ApplicationRoleMenu> roleMenuRepository, ICollection<int> roleIds)
        {
            var menuQuery = menuRepository.All;
            var roleMenuQuery = roleMenuRepository.All;
            var menus = new List<MenuItem>();

            var roleMenus = roleMenuQuery.Where(r => roleIds.Contains(r.RoleId)).ToList();
            roleMenus.ForEach(x => { var menu = menuRepository.All.Where(m => m.Id == x.MenuId).FirstOrDefault(); menus.Add(menu); });

            return menus.GroupBy(x=>x.Id).Select(x=>x.FirstOrDefault()).ToList();
        }
    }
}
