using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("user")]
    public class UserController : CoreController
    {
        ApplicationSignInManager _signInManager;
        ApplicationUserManager _userManager;
        IUserService _userService;
        ISisterConcernService _sisterConcernService;
        IRoleService _roleService;
        IMapper _mapper;
        ISystemInformationService _sysInfoService;


        public UserController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager, IErrorService errorService,
            IUserService userService, ISisterConcernService sisterConcernService,
            IRoleService roleService, IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _sisterConcernService = sisterConcernService;
            _roleService = roleService;
            _mapper = mapper;
            _sysInfoService = sysInfoService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var usersAsync = _userService.GetAllUserAsync();
            var users = _mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<CreateUserViewModel>>(await usersAsync);
            return View(users);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var vm = new CreateUserViewModel
            {
                VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList(),
                VMRoles = _roleService.GetAllRole().Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList(),
                ConcernId = User.Identity.GetConcernId().ToString()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel vm, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
                    vm.EmployeeId = formCollection["EmployeesId"];

                var user = _mapper.Map<CreateUserViewModel, ApplicationUser>(vm);
                user.LockoutEnabled = false;
                var result = await UserManager.CreateAsync(user, "Test_123");

                if (result.Succeeded)
                {
                    string[] roles = vm.RoleName.Split(',');
                    for (int i = 0; i < roles.Length; i++)
                        UserManager.AddToRole(user.Id, roles[i]);

                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    AddToastMessage("", "User has saved successfully with default password: Test_123", ToastType.Success);
                    ModelState.Clear();

                    vm = new CreateUserViewModel
                    {
                        VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c
                       => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList(),
                        VMRoles = _roleService.GetAllRole().Select(r
                        => new SelectListItem { Text = r.Name, Value = r.Name }).ToList()
                        ,
                        ConcernId = User.Identity.GetConcernId().ToString()
                    };
                    return View(vm);
                }
                else
                {
                    foreach (var error in result.Errors)
                        AddToastMessage(string.Empty, error, ToastType.Error);

                    vm.VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c
                        => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList();
                    vm.VMRoles = _roleService.GetAllRole().Select(r
                        => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();

                    return View(vm);
                }
            }

            vm.VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c
               => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList();
            vm.VMRoles = _roleService.GetAllRole().Select(r
                => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
            vm.ConcernId = User.Identity.GetConcernId().ToString();

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var user = UserManager.FindById(id);
            var vmUser = _mapper.Map<ApplicationUser, CreateUserViewModel>(user);

            var roles = UserManager.GetRoles(user.Id);
            vmUser.RoleName = string.Join(",", roles);

            vmUser.VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c
                => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList();
            vmUser.VMRoles = _roleService.GetAllRole().Select(r
               => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
            vmUser.ConcernId = User.Identity.GetConcernId().ToString();

            return View("Create", vmUser);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreateUserViewModel vm, FormCollection formCollection)
        {
            if (!ModelState.IsValid)
            {
                vm.VMSisterConcerns = _sisterConcernService.GetAllSisterConcern().Select(c
                    => new SelectListItem { Text = c.Name, Value = c.ConcernID.ToString() }).ToList();
                vm.VMRoles = _roleService.GetAllRole().Select(r
                    => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();
                vm.ConcernId = User.Identity.GetConcernId().ToString();
                return View("Create", vm);
            }

            var user = UserManager.FindById(int.Parse(vm.Id));
            user.UserName = vm.UserName;
            user.Email = vm.Email;
            user.ConcernID = int.Parse(vm.ConcernId);
            user.PhoneNumber = vm.PhoneNumber;
            if (!string.IsNullOrEmpty(formCollection["EmployeesId"]))
                user.EmployeeID = int.Parse(GetDefaultIfNull(formCollection["EmployeesId"]));
            UserManager.Update(user);

            var rolesForUser = await UserManager.GetRolesAsync(user.Id);
            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                }
            }

            string[] roles = formCollection["RoleName"].Split(',');
            for (int i = 0; i < roles.Length; i++)
                UserManager.AddToRole(user.Id, roles[i]);

            AddToastMessage("", "User has saved successfully!", ToastType.Success);
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var logins = user.Logins;
            var rolesForUser = await UserManager.GetRolesAsync(id);

            foreach (var login in logins.ToList())
            {
                await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }

            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                }
            }

            await UserManager.DeleteAsync(user);
            AddToastMessage("", "User has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult ChangeStatus(int id, string status)
        {
            var user = UserManager.FindById(id);
            user.LockoutEnabled = status.Equals("0") ? true : false;
            UserManager.Update(user);
            AddToastMessage("", "User has been updated successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        #region SignInManager
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        #endregion

        #region UserManager
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion
    }
}