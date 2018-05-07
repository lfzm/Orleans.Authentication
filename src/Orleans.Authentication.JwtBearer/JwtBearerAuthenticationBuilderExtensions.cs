using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication
{
   public static class JwtBearerAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder)
          => builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)
            => builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<JwtBearerOptions> configureOptions)
            => builder.AddJwtBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<JwtBearerOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerPostConfigureOptions>());
            return builder.AddScheme<JwtBearerOptions, JwtBearerHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
