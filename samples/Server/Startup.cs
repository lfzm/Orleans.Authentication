using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Authentication;
using Orleans.Authentication.IdentityServer4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orleans.Configuration;

namespace Server
{
    public static class Startup
    {

        public static void ConfigureServices(HostBuilderContext builder, IServiceCollection services)
        {
        
        }
        public static void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.HostingEnvironment.EnvironmentName}.json", optional: true);
        }
    }
}
