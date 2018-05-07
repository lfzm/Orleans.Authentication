using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    public static class AuthenticateContextExtensions
    {
        public static Task<AuthenticateResult> AuthenticateAsync(this AuthenticateContext context) =>
            context.AuthenticateAsync(scheme: null);

        public static Task<AuthenticateResult> AuthenticateAsync(this AuthenticateContext context, string scheme) =>
            context.RequestServices.GetRequiredService<IAuthenticationService>().AuthenticateAsync(context, scheme);
    }
}
