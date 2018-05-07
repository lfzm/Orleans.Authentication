using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Authentication.JwtBearer
{
    public class JwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
    {
        private OpenIdConnectConfiguration _configuration;

        public JwtBearerHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger)
            : base(options, logger)
        { }


        /// <summary>
        /// Searches the 'Authorization' header for a 'Bearer' token. If the 'Bearer' token is found, it is validated using <see cref="TokenValidationParameters"/> set in the options.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = null;
            try
            {

                string authorization = base.Context.GetHeaders("Authorization")?.ToString();
                // 如果没有找到授权头，则不需要进一步处理
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.NoResult();
                }

                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                // 如果没有找到令牌，则无法继续工作
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Authorization token not found");
                }


                if (_configuration == null && Options.ConfigurationManager != null)
                {
                    _configuration = await Options.ConfigurationManager.GetConfigurationAsync(Context.CancellationTokenSource.Token);
                }

                var validationParameters = Options.TokenValidationParameters.Clone();
                if (_configuration != null)
                {
                    var issuers = new[] { _configuration.Issuer };
                    validationParameters.ValidIssuers = validationParameters.ValidIssuers?.Concat(issuers) ?? issuers;

                    validationParameters.IssuerSigningKeys = validationParameters.IssuerSigningKeys?.Concat(_configuration.SigningKeys)
                        ?? _configuration.SigningKeys;
                }

                List<Exception> validationFailures = null;
                SecurityToken validatedToken;

                // Options.SecurityTokenValidators 默认为： new List<ISecurityTokenValidator> { new JwtSecurityTokenHandler() }
                foreach (var validator in Options.SecurityTokenValidators)
                {
                    if (validator.CanReadToken(token))
                    {
                        ClaimsPrincipal principal;
                        try
                        {
                            principal = validator.ValidateToken(token, validationParameters, out validatedToken);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogInformation(ex, "Failed to validate the token.");

                           // RefreshOnIssuerKeyNotFound默认为True, 在SignatureKey未找到时，重新从OIDC服务器获取
                            if (Options.RefreshOnIssuerKeyNotFound && Options.ConfigurationManager != null
                                && ex is SecurityTokenSignatureKeyNotFoundException)
                            {
                                Options.ConfigurationManager.RequestRefresh();
                            }

                            if (validationFailures == null)
                            {
                                validationFailures = new List<Exception>(1);
                            }
                            validationFailures.Add(ex);
                            continue;
                        }

                        Logger.LogInformation("Successfully validated the token.");

                        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));

                    }
                }

                if (validationFailures != null)
                {
                    return AuthenticateResult.Fail((validationFailures.Count == 1) ? validationFailures[0] : new AggregateException(validationFailures));
                }

                return AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token ?? "[null]");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occurred while processing message.");
                return AuthenticateResult.Fail(ex);
                throw;
              
           
            }
        }

   
    }
}
