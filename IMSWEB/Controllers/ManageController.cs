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
    [Authorize]
    [RoutePrefix("color")]
    public class ManageController : CoreController
    {
        ApplicationSignInManager _signInManager;
        ApplicationUserManager _userManager;
        IMapper _mapper;

        public ManageController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager, IErrorService errorService,
            IMapper mapper, ISystemInformationService sysInfoService)
            : base(errorService, sysInfoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var user = UserManager.FindById(id);

            var vmUser = new UpdateUserInfoViewModel();
            vmUser.Id = user.Id.ToString();
            vmUser.UserName = user.UserName;
            vmUser.Email = user.Email;
            vmUser.PhoneNumber = user.PhoneNumber;

            return View("Edit", vmUser);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateUserInfoViewModel vm)
        {
            if (!string.IsNullOrEmpty(vm.Password) && 
                !vm.Password.Equals(vm.RePassword))
                ModelState.AddModelError("RePassword", "Password must be same.");

            if (!ModelState.IsValid)
                return View("Edit", vm);

            var user = UserManager.FindById(int.Parse(vm.Id));
            user.UserName = vm.UserName;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;

            if (!string.IsNullOrEmpty(vm.Password))
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, token, vm.Password);

                if(result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                        AddToastMessage(string.Empty, error, ToastType.Error);

                    return View("Edit", vm);
                }
            }

            await UserManager.UpdateAsync(user);
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            AddToastMessage("", "Information has updated successfully!", ToastType.Success);
            ModelState.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                AddToastMessage("", "Password changed successfully.", ToastType.Success);
                return RedirectToAction("Index", "Home", new { Message = "Success" });
            }
            else
                AddToastMessage("", "Password changed failed.", ToastType.Error);

            //AddErrors(result);
            return View(model);
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