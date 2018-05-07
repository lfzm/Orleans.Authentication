using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    public interface IAuthenticationHandler
    {
    
        /// <summary>
        /// The handler should initialize anything it needs from the request and scheme here.
        /// </summary>
        /// <param name="scheme">The <see cref="AuthenticationScheme"/> scheme.</param>
        /// <returns></returns>
        Task InitializeAsync(AuthenticationScheme scheme, AuthenticateContext context);

        /// <summary>
        /// Authentication behavior.
        /// </summary>
        /// <returns>The <see cref="AuthenticateResult"/> result.</returns>
        Task<AuthenticateResult> AuthenticateAsync();

    }
}
