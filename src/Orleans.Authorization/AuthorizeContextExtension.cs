using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;

namespace Orleans.Authorization
{
    /// <summary>
    /// ClaimsPrincipal 扩展类
    /// </summary>
    public static class AuthorizeContextExtension
    {
        /// <summary>
        /// 授权用户信息
        /// </summary>
        public static ClaimsPrincipal User(this Grain grain)
        {
            return AuthorizeContext.User;
        }

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
        /// 用户Id
        /// </summary>
        /// <returns></returns>
        public static int UserId(this Grain grain)
        {
            return grain.User().UserId();
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

        /// <summary>
        /// 昵称
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string NickName(this Grain grain)
        {
            return grain.User().NickName();
        }
    }
}
