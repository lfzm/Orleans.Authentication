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
        
            services.AddAdoNetGrainStorage("PubSubStore", opt =>
            {
                opt.Invariant = "MySql.Data.MySqlClient";
                opt.ConnectionString = "Database=orleans;Data Source=127.0.0.1;User Id=root;Password=sapass;pooling=false";
            });
        }
        public static void ConfigureAppConfiguration(HostBuilderContext builder, IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.HostingEnvironment.EnvironmentName}.json", optional: true);
        }
    }
}
