using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    public abstract class AuthenticationHandler<TOptions> : IAuthenticationHandler where TOptions : AuthenticationSchemeOptions, new()
    {
        public AuthenticationScheme Scheme { get; private set; }
        public TOptions Options { get; private set; }
        protected AuthenticateContext Context { get; private set; }
        protected ILogger Logger { get; }

        protected IOptionsMonitor<TOptions> OptionsMonitor { get; }

        protected virtual string ClaimsIssuer => Options.ClaimsIssuer ?? Scheme.Name;

        protected AuthenticationHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger)
        {
            Logger = logger.CreateLogger(this.GetType().FullName);
            OptionsMonitor = options;
        }

        /// <summary>
        /// Initialize the handler, resolve the options and validate them.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InitializeAsync(AuthenticationScheme scheme, AuthenticateContext context)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Scheme = scheme;
            Context = context;
            Options = OptionsMonitor.Get(Scheme.Name) ?? new TOptions();
            Options.Validate(Scheme.Name);

            await InitializeHandlerAsync();
        }

      
        /// <summary>
        /// Called after options/events have been initialized for the handler to finish initializing itself.
        /// </summary>
        /// <returns>A task</returns>
        protected virtual Task InitializeHandlerAsync() => Task.CompletedTask;


        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            // Calling Authenticate more than once should always return the original value.
            var result = await HandleAuthenticateOnceAsync();
            if (result?.Failure == null)
            {
                var ticket = result?.Ticket;
                if (ticket?.Principal != null)
                {
                    Logger.LogDebug("AuthenticationScheme: {AuthenticationScheme} was successfully authenticated.", Scheme.Name);
                }
                else
                {
                    Logger.LogDebug("AuthenticationScheme: {AuthenticationScheme} was not authenticated.", Scheme.Name);
                }
            }
            else
            {
                Logger.LogDebug("{AuthenticationScheme} was not authenticated. Failure message: {FailureMessage}",Scheme.Name, result.Failure.Message);
            }
            return result;
        }

        /// <summary>
        /// Used to ensure HandleAuthenticateAsync is only invoked once. The subsequent calls
        /// will return the same authenticate result.
        /// </summary>
        protected Task<AuthenticateResult> HandleAuthenticateOnceAsync()
        {
        
            return HandleAuthenticateAsync();
        }

        /// <summary>
        /// Used to ensure HandleAuthenticateAsync is only invoked once safely. The subsequent
        /// calls will return the same authentication result. Any exceptions will be converted
        /// into a failed authentication result containing the exception.
        /// </summary>
        protected async Task<AuthenticateResult> HandleAuthenticateOnceSafeAsync()
        {
            try
            {
                return await HandleAuthenticateOnceAsync();
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        protected abstract Task<AuthenticateResult> HandleAuthenticateAsync();

    }
}
