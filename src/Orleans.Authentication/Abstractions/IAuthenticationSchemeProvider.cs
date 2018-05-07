using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    /// <summary>
    /// Responsible for managing what authenticationSchemes are supported.
    /// </summary>
    public interface IAuthenticationSchemeProvider
    {
        /// <summary>
        /// Registers a scheme for use by <see cref="IAuthenticationService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        void AddScheme(AuthenticationScheme scheme);

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        void RemoveScheme(string name);

        /// <summary>
        /// Returns all currently registered <see cref="AuthenticationScheme"/>s.
        /// </summary>
        /// <returns>All currently registered <see cref="AuthenticationScheme"/>s.</returns>
        Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync();

        /// <summary>
        /// Returns the <see cref="AuthenticationScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        Task<AuthenticationScheme> GetSchemeAsync(string name);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultAuthenticateScheme"/>.
        /// Otherwise, this will fallback to <see cref="AuthenticationOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.</returns>
        Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync();



    }
}
