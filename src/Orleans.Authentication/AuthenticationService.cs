using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    /// <summary>
    /// Implements <see cref="IAuthenticationService"/>.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Used to lookup AuthenticationSchemes.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; }

        /// <summary>
        /// Used to resolve IAuthenticationHandler instances.
        /// </summary>
        public IAuthenticationHandlerProvider Handlers { get; }

        /// <summary>
        /// Used for claims transformation.
        /// </summary>
        public IClaimsTransformation Transform { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IAuthenticationSchemeProvider"/>.</param>
        /// <param name="handlers">The <see cref="IAuthenticationRequestHandler"/>.</param>
        /// <param name="transform">The <see cref="IClaimsTransformation"/>.</param>
        public AuthenticationService(IAuthenticationSchemeProvider schemes, IAuthenticationHandlerProvider handlers, IClaimsTransformation transform)
        {
            Schemes = schemes;
            Handlers = handlers;
            Transform = transform;
        }

        /// <summary>
        /// Authenticate for the specified authentication scheme.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scheme">The name of the authentication scheme.</param>
        /// <returns>The result.</returns>
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthenticateContext context, string scheme)
        {
            if (scheme == null)
            {
                var defaultScheme = await Schemes.GetDefaultAuthenticateSchemeAsync();
                scheme = defaultScheme?.Name;
                if (scheme == null)
                {
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found.");
                }
            }

            var handler = await Handlers.GetHandlerAsync(context, scheme);
            if (handler == null)
            {
                throw await CreateMissingHandlerException(scheme);
            }

            var result = await handler.AuthenticateAsync();
            if (result != null && result.Succeeded)
            {
                var transformed = await Transform.TransformAsync(result.Principal);
                return AuthenticateResult.Success(new AuthenticationTicket(transformed, result.Properties, result.Ticket.AuthenticationScheme));
            }
            return result;
        }
      
        private async Task<Exception> CreateMissingHandlerException(string scheme)
        {
            var schemes = string.Join(", ", (await Schemes.GetAllSchemesAsync()).Select(sch => sch.Name));

            var footer = $" Did you forget to call AddAuthentication().Add[SomeAuthHandler](\"{scheme}\",...)?";

            if (string.IsNullOrEmpty(schemes))
            {
                return new InvalidOperationException(
                    $"No authentication handlers are registered." + footer);
            }

            return new InvalidOperationException(
                $"No authentication handler is registered for the scheme '{scheme}'. The registered schemes are: {schemes}." + footer);
        }
    }
}
