// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Orleans.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Orleans.Authentication.IdentityServer4
{
    /// <summary>
    /// Options for IdentityServer authentication
    /// </summary>
    public class IdentityServerAuthenticationOptions : AuthenticationSchemeOptions
    {

        /// <summary>
        /// Base-address of the token issuer
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Specifies whether HTTPS is required for the discovery endpoint
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Specifies which token types are supported (JWT, reference or both)
        /// </summary>
        public SupportedTokens SupportedTokens { get; set; } = SupportedTokens.Both;

        /// <summary>
        /// Callback to retrieve token from incoming request
        /// </summary>
        public Func<AuthenticateContext, string> TokenRetriever { get; set; } = (context) =>
        {
            return context.GetHeaders("Authorization");
        };

        /// <summary>
        /// Name of the API resource used for authentication against introspection endpoint
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Secret used for authentication against introspection endpoint
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Enable if this API is being secured by IdentityServer3, and if you need to support both JWTs and reference tokens.
        /// If you enable this, you should add scope validation for incoming JWTs.
        /// </summary>
        public bool LegacyAudienceValidation { get; set; } = false;

        /// <summary>
        /// Claim type for name
        /// </summary>
        public string NameClaimType { get; set; } = "name";

        /// <summary>
        /// Claim type for role
        /// </summary>
        public string RoleClaimType { get; set; } = "role";

        /// <summary>
        /// Specifies inbound claim type map for JWT tokens (mainly used to disable the annoying default behavior of the MS JWT handler)
        /// </summary>
        public Dictionary<string, string> InboundJwtClaimTypeMap { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// specifies the allowed clock skew when validating JWT tokens
        /// </summary>
        public TimeSpan? JwtValidationClockSkew { get; set; }

        /// <summary>
        /// back-channel handler for JWT middleware
        /// </summary>
        public HttpMessageHandler JwtBackChannelHandler { get; set; }

      
        /// <summary>
        /// timeout for back-channel operations
        /// </summary>
        public TimeSpan BackChannelTimeouts { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Specifies how often the cached copy of the discovery document should be refreshed.
        /// If not set, it defaults to the default value of Microsoft's underlying configuration manager (which right now is 24h).
        /// If you need more fine grained control, provide your own configuration manager on the JWT options.
        /// </summary>
        public TimeSpan? DiscoveryDocumentRefreshInterval { get; set; }

        /// <summary>
        /// Gets a value indicating whether JWTs are supported.
        /// </summary>
        public bool SupportsJwt => SupportedTokens == SupportedTokens.Jwt || SupportedTokens == SupportedTokens.Both;


        internal void ConfigureJwtBearer(JwtBearerOptions jwtOptions)
        {
            jwtOptions.Authority = Authority;
            jwtOptions.RequireHttpsMetadata = RequireHttpsMetadata;
            jwtOptions.BackchannelTimeout = BackChannelTimeouts;
            jwtOptions.RefreshOnIssuerKeyNotFound = true;

            //文档刷新时间
            if (DiscoveryDocumentRefreshInterval.HasValue)
            {
                var parsedUrl = DiscoveryClient.ParseUrl(Authority);

                var httpClient = new HttpClient(JwtBackChannelHandler ?? new HttpClientHandler())
                {
                    Timeout = BackChannelTimeouts,
                    MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
                };

                var manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    parsedUrl.Url,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever(httpClient) { RequireHttps = RequireHttpsMetadata })
                {
                    AutomaticRefreshInterval = DiscoveryDocumentRefreshInterval.Value
                    
                };

                jwtOptions.ConfigurationManager = manager;
            }

            if (JwtBackChannelHandler != null)
            {
                jwtOptions.BackchannelHttpHandler = JwtBackChannelHandler;
            }

            // if API name is set, do a strict audience check for
            if (!string.IsNullOrWhiteSpace(ApiName) && !LegacyAudienceValidation)
            {
                jwtOptions.Audience = ApiName;
            }
            else
            {
                // no audience validation, rely on scope checks only
                jwtOptions.TokenValidationParameters.ValidateAudience = false;
            }
            jwtOptions.TokenValidationParameters.NameClaimType = NameClaimType;
            jwtOptions.TokenValidationParameters.RoleClaimType = RoleClaimType;

            if (JwtValidationClockSkew.HasValue)
            {
                jwtOptions.TokenValidationParameters.ClockSkew = JwtValidationClockSkew.Value;
            }

            if (InboundJwtClaimTypeMap != null)
            {
                var handler = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = InboundJwtClaimTypeMap
                };

                jwtOptions.SecurityTokenValidators.Clear();
                jwtOptions.SecurityTokenValidators.Add(handler);
            }
        }


    }
}