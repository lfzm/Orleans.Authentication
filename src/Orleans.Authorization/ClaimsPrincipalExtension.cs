using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Orleans.Authorization
{

    /// <summary>
    /// ClaimsPrincipal 扩展类
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static int UserId(this ClaimsPrincipal principal)
        {
            var sub = principal.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (string.IsNullOrEmpty(sub))
                return 0;
            return int.Parse(sub);
        }
        /// <summary>
        /// 昵称
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string NickName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(JwtClaimTypes.Subject)?.Value;
        }
    }
}
