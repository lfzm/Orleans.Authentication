using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication.JwtBearer
{
    /// <summary>
    /// Default values used by bearer authentication.
    /// </summary>
    public static class JwtBearerDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme property in the JwtBearerAuthenticationOptions
        /// </summary>
        public const string AuthenticationScheme = "Bearer";
    }
}
