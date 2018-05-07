using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{

    /// <summary>
    /// Used to provide authentication.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="AuthenticateContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        Task<AuthenticateResult> AuthenticateAsync(AuthenticateContext context, string scheme);

      
    }
}
