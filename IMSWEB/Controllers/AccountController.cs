using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IMSWEB.Service;
using IMSWEB.Model;
using System.Collections.Generic;
using AutoMapper;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class AccountController : CoreController
    {
        protected ApplicationSignInManager _signInManager;
        protected ApplicationUserManager _userManager;

        private readonly ISystemInformationService _systemInformationService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ISisterConcernService _sisterConcernService;

        public AccountController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager, IErrorService errorService,
            ISystemInformationService systemInformationService,
            IRoleService roleService, IMapper mapper, ISisterConcernService sisterConcernService)
            : base(errorService, systemInformationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _systemInformationService = systemInformationService;
            _roleService = roleService;
            _mapper = mapper;
            _sisterConcernService = sisterConcernService;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            TempData.Clear();
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            SystemInformation sysInfo = null;
            var LoggedUser = UserManager.FindByName(model.UserName);
            if (LoggedUser == null)
            {
                AddToastMessage("", "This Account is not available.", ToastType.Error);
                return View(model);
            }
            TempData["UserName"] = UserManager.FindByName(model.UserName);
            int concernId = LoggedUser.ConcernID;
            if (LoggedUser != null)
            {
                sysInfo = _systemInformationService.GetSystemInformationByConcernId(LoggedUser.ConcernID);
     
                if (LoggedUser.LockoutEnabled)
                {
                    AddToastMessage("", "This Account currently is Inactive.", ToastType.Error);
                    return View(model);
                }
                var userrole = _roleService.GetUserRoleByUserId(LoggedUser.Id);
                bool IsAdmin = false;
                foreach (var item in userrole)
                {
                    var role = _roleService.GetRoleById(item.RoleId);
                    if (role.Name.Equals(EnumUserRoles.Admin.ToString()))
                    {
                        IsAdmin = true;
                        break;
                    }

                }
                if (!IsAdmin)
                {
                    if (sysInfo.ExpireDate < GetLocalDateTime())
                    {
                        model.ErrorMessage = sysInfo.ExpireMessage + Environment.NewLine + "Expire Date: " + sysInfo.ExpireDate.Value.ToString("dd MMM yyyy");
                        return RedirectToAction("Index", "Payment", new { paymentID = "", status = "", concernId = LoggedUser.ConcernID });
                        //return View(model);
                    }
                }

            }



            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        TempData.Remove("ConcernName");
                        Session["SystemInfo"] = _mapper.Map<SystemInformation, CreateSystemInformationViewModel>(sysInfo);
                        SisterConcern concern = _sisterConcernService.GetSisterConcernById(LoggedUser.ConcernID);
                        if (concern != null && string.IsNullOrEmpty(concern.SmsContactNo))
                        {
                            int concernIsd = concern.ConcernID;
                            AddToastMessage("", "Please update sms contact number for payment confirmation message.", ToastType.Info);
                            return RedirectToAction("Edit", "SisterConcern", new { id = concernId });
                        }
                    }
                    AddToastMessage("", "Welcome!", ToastType.Success);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    AddToastMessage("", "Account is locked out.", ToastType.Info);
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    AddToastMessage("", "Verification code is required.", ToastType.Info);
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    AddToastMessage("", "Invalid login attempt.", ToastType.Error);
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId == default(int) || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            TempData.Clear();
            return RedirectToAction("Index", "Home");
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

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Helpers Method
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                AddToastMessage(string.Empty, error, ToastType.Error);
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion 
    }
}