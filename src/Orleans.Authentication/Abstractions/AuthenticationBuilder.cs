using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication
{
    /// <summary>
    /// Used to configure authentication
    /// </summary>
    public class AuthenticationBuilder
    {

        /// <summary>
        /// The services being configured.
        /// </summary>
        public virtual IServiceCollection Services { get; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        public AuthenticationBuilder(IServiceCollection services )
        {
            this.Services = services;
 
        }

        /// <summary>
        /// Adds a <see cref="AuthenticationScheme"/> which can be used by <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <typeparam name="TOptions">The <see cref="AuthenticationSchemeOptions"/> type to configure the handler."/>.</typeparam>
        /// <typeparam name="THandler">The <see cref="AuthenticationHandler{TOptions}"/> used to handle this scheme.</typeparam>
        /// <param name="authenticationScheme">The name of this scheme.</param>
        /// <param name="displayName">The display name of this scheme.</param>
        /// <param name="configureOptions">Used to configure the scheme options.</param>
        /// <returns>The builder.</returns>
        public virtual AuthenticationBuilder AddScheme<TOptions, THandler>(string authenticationScheme, string displayName, Action<TOptions> configureOptions)
            where TOptions : AuthenticationSchemeOptions, new()
            where THandler : AuthenticationHandler<TOptions>
            => AddSchemeHelper<TOptions, THandler>(authenticationScheme, displayName, configureOptions);

        /// <summary>
        /// Adds a <see cref="AuthenticationScheme"/> which can be used by <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <typeparam name="TOptions">The <see cref="AuthenticationSchemeOptions"/> type to configure the handler."/>.</typeparam>
        /// <typeparam name="THandler">The <see cref="AuthenticationHandler{TOptions}"/> used to handle this scheme.</typeparam>
        /// <param name="authenticationScheme">The name of this scheme.</param>
        /// <param name="configureOptions">Used to configure the scheme options.</param>
        /// <returns>The builder.</returns>
        public virtual AuthenticationBuilder AddScheme<TOptions, THandler>(string authenticationScheme, Action<TOptions> configureOptions)
            where TOptions : AuthenticationSchemeOptions, new()
            where THandler : AuthenticationHandler<TOptions>
            => AddScheme<TOptions, THandler>(authenticationScheme, displayName: null, configureOptions: configureOptions);


        private AuthenticationBuilder AddSchemeHelper<TOptions, THandler>(string authenticationScheme, string displayName, Action<TOptions> configureOptions)
        where TOptions : class, new()
        where THandler : class, IAuthenticationHandler
        {
            Services.Configure<AuthenticationOptions>(o =>
            {
                o.AddScheme(authenticationScheme, scheme => {
                    scheme.HandlerType = typeof(THandler);
                    scheme.DisplayName = displayName;
                });
            });
            if (configureOptions != null)
            {
                Services.Configure(authenticationScheme, configureOptions);
            }
            Services.AddTransient<THandler>();
            return this;
        }

    }
}
