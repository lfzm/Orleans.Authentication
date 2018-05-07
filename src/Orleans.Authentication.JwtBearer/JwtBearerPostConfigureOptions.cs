using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Orleans.Authentication.JwtBearer
{
    /// <summary>
    /// Used to setup defaults for all <see cref="JwtBearerOptions"/>.
    /// </summary>
    public class JwtBearerPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, JwtBearerOptions options)
        {
            if (string.IsNullOrEmpty(options.TokenValidationParameters.ValidAudience) && !string.IsNullOrEmpty(options.Audience))
            {
                options.TokenValidationParameters.ValidAudience = options.Audience;
            }

            if (options.ConfigurationManager == null)
            {
                if (options.Configuration != null)
                {
                    options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(options.Configuration);
                }
                else if (!(string.IsNullOrEmpty(options.MetadataAddress) && string.IsNullOrEmpty(options.Authority)))
                {
                    if (string.IsNullOrEmpty(options.MetadataAddress) && !string.IsNullOrEmpty(options.Authority))
                    {
                        options.MetadataAddress = options.Authority;
                        if (!options.MetadataAddress.EndsWith("/", StringComparison.Ordinal))
                        {
                            options.MetadataAddress += "/";
                        }

                        options.MetadataAddress += ".well-known/openid-configuration";
                    }

                    if (options.RequireHttpsMetadata && !options.MetadataAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("The MetadataAddress or Authority must use HTTPS unless disabled for development by setting RequireHttpsMetadata=false.");
                    }

                    var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                    httpClient.Timeout = options.BackchannelTimeout;
                    httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

                    options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.MetadataAddress, new OpenIdConnectConfigurationRetriever(),
                        new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });
                }
            }
        }
    }
}
