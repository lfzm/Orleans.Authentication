// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.Authentication.IdentityServer4
{
    /// <summary>
    /// Authentication handler for validating both JWT and reference tokens
    /// </summary>
    public class IdentityServerAuthenticationHandler : AuthenticationHandler<IdentityServerAuthenticationOptions>
    {
        private readonly ILogger _logger;

        /// <inheritdoc />
        public IdentityServerAuthenticationHandler(
            IOptionsMonitor<IdentityServerAuthenticationOptions> options,
            ILoggerFactory logger)
            : base(options, logger)
        {
            _logger = logger.CreateLogger<IdentityServerAuthenticationHandler>();
        }

        /// <summary>
        /// Tries to validate a token on the current request
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _logger.LogTrace("HandleAuthenticateAsync called");
            var token = Options.TokenRetriever(Context);
            _logger.LogInformation("Authenticate Token " + token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogTrace("Token not found");
                return AuthenticateResult.Fail("Authorization token not found");
            }
            _logger.LogTrace("Token found: {token}", token);
            // seems to be a JWT
            if (token.Contains('.') && Options.SupportsJwt)
            {
                _logger.LogTrace("Token is a JWT and is supported.");
                return await Context.AuthenticateAsync(Scheme.Name + IdentityServerAuthenticationDefaults.JwtAuthenticationScheme);
            }
            else
            {
                _logger.LogTrace("Neither JWT nor reference tokens seem to be correctly configured for incoming token.");
            }
            return AuthenticateResult.NoResult();

        }
    }
}