using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Authentication;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orleans;
using Orleans.Authorization;

namespace IGrains
{
    /// <summary>
    /// Orleans grain implementation class HelloGrain.
    /// </summary>
    [Authorize]
    public class AuthorizeGrain : Orleans.Grain, IAuthorize
    {
        private readonly ILogger Logger;
        private readonly IAuthenticationSchemeProvider Schemes;
        private IAuthenticationHandlerProvider Handlers { get; set; }


        public AuthorizeGrain(ILogger<AuthorizeGrain> _logger, IAuthenticationSchemeProvider _schemes)
        {
            this.Logger = _logger;
            this.Schemes = _schemes;
        }
        public override Task OnActivateAsync()
        {
            this.Handlers = base.ServiceProvider.GetRequiredService<IAuthenticationHandlerProvider>();
            return base.OnActivateAsync();
        }


        public async Task<User> SayHelloAsync(string greeting)
        {
            IServiceProvider serviceProvider = base.ServiceProvider.GetService<IServiceProvider>();
            var user = this.User();
            var userId = this.UserId();
          await   base.GrainFactory.GetGrain<IAuthorize>(1).SayHelloAsync(greeting);
            //string access = RequestContext.Get("access_token")?.ToString();
            this.Logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return await Task.FromResult(new User());
        }
    }
}
