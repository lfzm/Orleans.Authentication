using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    /// <summary>
    /// Implementation of <see cref="IAuthenticationHandlerProvider"/>.
    /// </summary>
    public class AuthenticationHandlerProvider : IAuthenticationHandlerProvider
    {
        /// <summary>
        /// The <see cref="IAuthenticationHandlerProvider"/>.
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; }
        // handler instance cache, need to initialize once per request
        private Dictionary<string, IAuthenticationHandler> _handlerMap = new Dictionary<string, IAuthenticationHandler>(StringComparer.Ordinal);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="schemes">The <see cref="IAuthenticationHandlerProvider"/>.</param>
        public AuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        }
        /// <summary>
        /// Returns the handler instance that will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="authenticationScheme">The name of the authentication scheme being handled.</param>
        /// <returns>The handler instance.</returns>
        public async Task<IAuthenticationHandler> GetHandlerAsync(AuthenticateContext context, string authenticationScheme)
        {
            if (_handlerMap.ContainsKey(authenticationScheme))
            {
                return _handlerMap[authenticationScheme];
            }

            var scheme = await Schemes.GetSchemeAsync(authenticationScheme);
            if (scheme == null)
            {
                return null;
            }
            var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
                ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
                as IAuthenticationHandler;
            if (handler != null)
            {
                await handler.InitializeAsync(scheme, context);
                _handlerMap[authenticationScheme] = handler;
            }
            return handler;
        }
    }
}
