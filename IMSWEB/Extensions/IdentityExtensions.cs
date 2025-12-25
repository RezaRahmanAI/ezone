using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace IMSWEB
{

    public static class IdentityExtensions
    {
        public static int GetConcernId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ConcernID");
            return (claim != null) ? Convert.ToInt32(claim.Value) : default(int);
        }
    }
}