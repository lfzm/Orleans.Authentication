// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using Orleans.Authentication.JwtBearer;

namespace Orleans.Authentication.IdentityServer4
{
    internal class ConfigureInternalOptions : 
        IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IdentityServerAuthenticationOptions _identityServerOptions;
        private string _scheme;

        public ConfigureInternalOptions(IdentityServerAuthenticationOptions identityServerOptions, string scheme)
        {
            _identityServerOptions = identityServerOptions;
            _scheme = scheme;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name == _scheme + IdentityServerAuthenticationDefaults.JwtAuthenticationScheme &&
                _identityServerOptions.SupportsJwt)
            {
                _identityServerOptions.ConfigureJwtBearer(options);
            }
        }

     

        public void Configure(JwtBearerOptions options)
        { }

       
    }
}