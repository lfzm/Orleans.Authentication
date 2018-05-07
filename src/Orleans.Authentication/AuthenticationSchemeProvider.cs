using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.Authentication
{
    public class AuthenticationSchemeProvider : IAuthenticationSchemeProvider
    {
        private readonly AuthenticationOptions _options;
        private readonly object _lock = new object();
        private IDictionary<string, AuthenticationScheme> _map = new Dictionary<string, AuthenticationScheme>(StringComparer.Ordinal);
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="AuthenticationOptions"/> options.</param>
        public AuthenticationSchemeProvider(IOptions<AuthenticationOptions> options)
        {
            _options = options.Value;

            foreach (var builder in _options.Schemes)
            {
                var scheme = builder.Build();
                AddScheme(scheme);
            }
        }

        private Task<AuthenticationScheme> GetDefaultSchemeAsync()
         => _options.DefaultScheme != null
         ? GetSchemeAsync(_options.DefaultScheme)
         : Task.FromResult<AuthenticationScheme>(null);

        /// <summary>
        /// Returns the scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.
        /// This is typically specified via <see cref="AuthenticationOptions.DefaultAuthenticateScheme"/>.
        /// Otherwise, this will fallback to <see cref="AuthenticationOptions.DefaultScheme"/>.
        /// </summary>
        /// <returns>The scheme that will be used by default for <see cref="IAuthenticationService.AuthenticateAsync(HttpContext, string)"/>.</returns>
        public virtual Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
            => _options.DefaultAuthenticateScheme != null
            ? GetSchemeAsync(_options.DefaultAuthenticateScheme)
            : GetDefaultSchemeAsync();


        /// <summary>
        /// Returns the <see cref="AuthenticationScheme"/> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        public virtual Task<AuthenticationScheme> GetSchemeAsync(string name)
            => Task.FromResult(_map.ContainsKey(name) ? _map[name] : null);
        public virtual Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
            => Task.FromResult<IEnumerable<AuthenticationScheme>>(_map.Values);

        /// <summary>
        /// Registers a scheme for use by <see cref="IAuthenticationService"/>. 
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public virtual void AddScheme(AuthenticationScheme scheme)
        {
            if (_map.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
            }
            lock (_lock)
            {
                if (_map.ContainsKey(scheme.Name))
                {
                    throw new InvalidOperationException("Scheme already exists: " + scheme.Name);
                }
                _map[scheme.Name] = scheme;
            }
        }

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IAuthenticationService"/>.
        /// </summary>
        /// <param name="name">The name of the authenticationScheme being removed.</param>
        public virtual void RemoveScheme(string name)
        {
            if (!_map.ContainsKey(name))
            {
                return;
            }
            lock (_lock)
            {
                if (_map.ContainsKey(name))
                {
                    var scheme = _map[name];
                    _map.Remove(name);
                }
            }
        }

    }
}
