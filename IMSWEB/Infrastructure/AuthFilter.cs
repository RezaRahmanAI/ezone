using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace IMSWEB
{
    public class AuthFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        ISystemInformationService _systemInformationService = null;

        public AuthFilter(ISystemInformationService systemInformationService)
        {
            _systemInformationService = systemInformationService;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var concernId = filterContext.HttpContext.User.Identity.GetConcernId();
            SystemInformation.CurrentSystemInfo = _systemInformationService.GetSystemInformationById(concernId);
        }
    }
}