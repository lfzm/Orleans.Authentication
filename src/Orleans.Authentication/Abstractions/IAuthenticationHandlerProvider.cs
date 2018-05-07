using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    /// <summary>
    /// Provides the appropriate IAuthenticationHandler instance for the authenticationScheme and request.
    /// </summary>
    public interface IAuthenticationHandlerProvider
    {
        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        Task<IAuthenticationHandler> GetHandlerAsync(AuthenticateContext context,string authenticationScheme);
    }
}
