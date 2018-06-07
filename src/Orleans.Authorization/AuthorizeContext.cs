using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Orleans.Authorization
{
 
    public static class AuthorizeContext
    {
        internal static readonly AsyncLocal<ClaimsPrincipal> CallContextData = new AsyncLocal<ClaimsPrincipal>();

        /// <summary>
        /// Auth User
        /// </summary>
        public static ClaimsPrincipal User
        {
            get
            {
                return CallContextData.Value;
            }
            internal set
            {
                CallContextData.Value = value;
            }
        }


    }
}
