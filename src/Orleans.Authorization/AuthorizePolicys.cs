using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authorization
{
    /// <summary>
    /// 授权政策
    /// </summary>
    public static class AuthorizePolicys
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public const string USERID = JwtClaimTypes.Subject;
    }

    /// <summary>
    /// 授权角色
    /// </summary>
    public static class AuthorizeRoles
    {
        /// <summary>
        /// 用户
        /// </summary>
        public const string User = "USER";
        /// <summary>
        /// 管理员
        /// </summary>
        public const string Admin = "ADMIN";
    }
}
