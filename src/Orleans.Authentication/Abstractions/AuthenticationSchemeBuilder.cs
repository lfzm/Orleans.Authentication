using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication
{
    /// <summary>
    /// Used to build <see cref="AuthenticationScheme"/>s.
    /// </summary>
    public class AuthenticationSchemeBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the scheme being built.</param>
        public AuthenticationSchemeBuilder(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the scheme being built.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The display name for the scheme being built.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The <see cref="IAuthenticationHandler"/> type responsible for this scheme.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Builds the <see cref="AuthenticationScheme"/> instance.
        /// </summary>
        /// <returns></returns>
        public AuthenticationScheme Build() => new AuthenticationScheme(Name, DisplayName, HandlerType);
    }
}
