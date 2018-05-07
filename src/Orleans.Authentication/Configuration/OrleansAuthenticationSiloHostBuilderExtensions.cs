using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans.Authentication;
using Orleans.Hosting;

namespace Orleans.Hosting
{
    public static class OrleansAuthenticationSiloHostBuilderExtensions
    {
        public static ISiloHostBuilder AddAuthentication(this ISiloHostBuilder builder, Action<HostBuilderContext, AuthenticationBuilder> authBuilder)
        {

            builder.ConfigureServices((context, services) =>
            {
                services.TryAddScoped<IAuthenticationService, AuthenticationService>();
                services.TryAddScoped<IAuthenticationHandlerProvider, AuthenticationHandlerProvider>();
                services.TryAddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
                services.TryAddSingleton<IClaimsTransformation, NoopClaimsTransformation>(); // Can be replaced with scoped ones that use DbContext
                var authb = new AuthenticationBuilder(services);
                authBuilder?.Invoke(context, authb);
            });
            return builder;

        }

        public static ISiloHostBuilder AddAuthentication(this ISiloHostBuilder builder, Action<HostBuilderContext, AuthenticationBuilder> authBuilder, string defaultScheme)
            => builder.AddAuthentication(authBuilder, o => o.DefaultScheme = defaultScheme);

        public static ISiloHostBuilder AddAuthentication(this ISiloHostBuilder services, Action<HostBuilderContext, AuthenticationBuilder> authBuilder, Action<AuthenticationOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            return services.AddAuthentication(authBuilder);
        }


    }
}
