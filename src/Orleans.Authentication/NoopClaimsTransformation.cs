using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    /// <summary>
    /// Default claims transformation is a no-op.
    /// </summary>
    public class NoopClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// Returns the principal unchanged.
        /// </summary>
        /// <param name="principal">The user.</param>
        /// <returns>The principal unchanged.</returns>
        public virtual Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            return Task.FromResult(principal);
        }
    }
}
