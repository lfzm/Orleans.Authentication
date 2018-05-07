// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Authentication.IdentityServer4;
using Orleans.Authentication.JwtBearer;
using Orleans.Authentication;

namespace Orleans.Authentication
{
    /// <summary>
    /// Extensions for registering the IdentityServer authentication handler
    /// </summary>
    public static class IdentityServerAuthenticationExtensions
    {
        /// <summary>
        /// Registers the IdentityServer authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder)
            => builder.AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// Registers the IdentityServer authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddIdentityServerAuthentication(authenticationScheme, configureOptions: null);

        /// <summary>
        /// Registers the IdentityServer authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder, Action<IdentityServerAuthenticationOptions> configureOptions) =>
            builder.AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Registers the IdentityServer authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<IdentityServerAuthenticationOptions> configureOptions)
        {
            builder.AddJwtBearer(authenticationScheme + IdentityServerAuthenticationDefaults.JwtAuthenticationScheme, configureOptions: null);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(services =>
            {
                var monitor = services.GetRequiredService<IOptionsMonitor<IdentityServerAuthenticationOptions>>();
                return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
            });
            
            return builder.AddScheme<IdentityServerAuthenticationOptions, IdentityServerAuthenticationHandler>(authenticationScheme, configureOptions);
        }

        /// <summary>
        /// Registers the IdentityServer authentication handler.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="jwtBearerOptions">The JWT bearer options.</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder, string authenticationScheme, 
            Action<JwtBearerOptions> jwtBearerOptions)
        {
            if (jwtBearerOptions != null)
            {
                builder.AddJwtBearer(authenticationScheme + IdentityServerAuthenticationDefaults.JwtAuthenticationScheme, jwtBearerOptions);
            }
            return builder.AddScheme<IdentityServerAuthenticationOptions, IdentityServerAuthenticationHandler>(authenticationScheme, (o) => { });
        }
    }
}