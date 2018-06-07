using IGrains;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Orleans.Authentication.IdentityServer4;
using Orleans.Authentication;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                RunMainAsync();
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
            Console.ReadKey();
        }

        private static async void RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var siloPort = 11111;
            int gatewayPort = 30000;
            var siloAddress = IPAddress.Loopback;

            var builder = new SiloHostBuilder()
                .UseEnvironment("Development")
                .UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(siloAddress, siloPort))
                .ConfigureAppConfiguration(Startup.ConfigureAppConfiguration)
                .ConfigureServices(Startup.ConfigureServices)
                .ConfigureEndpoints(siloAddress, siloPort, gatewayPort)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(AuthorizeGrain).Assembly).WithReferences())
                .ConfigureLogging((HostBuilderContext context, ILoggingBuilder logging) =>
               {
                   logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                   logging.AddConsole();
               })
                .AddAuthentication((HostBuilderContext context, AuthenticationBuilder authen) =>
                {
                    authen.AddIdentityServerAuthentication(opt =>
                    {
                        opt.RequireHttpsMetadata = true;
                        opt.Authority = "https://xxx.xxx.com/";
                        opt.ApiName = "OTC_API";
                    });
                }, IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddAuthorizationFilter();


            var host = builder.Build();
            await host.StartAsync();
            return host;


        }
    }
}
