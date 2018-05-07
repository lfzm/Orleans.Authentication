using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication
{
    /// <summary>
    /// Contains the options used by the <see cref="AuthenticationHandler{T}"/>.
    /// </summary>
    public class AuthenticationSchemeOptions
    {
        /// <summary>
        /// Check that the options are valid. Should throw an exception if things are not ok.
        /// </summary>
        public virtual void Validate() { }

        /// <summary>
        /// Checks that the options are valid for a specific scheme
        /// </summary>
        /// <param name="scheme">The scheme being validated.</param>
        public virtual void Validate(string scheme)
            => Validate();

        /// <summary>
        /// Gets or sets the issuer that should be used for any claims that are created
        /// </summary>
        public string ClaimsIssuer { get; set; }


        /// <summary>
        /// If set, this specifies a default scheme that authentication handlers should forward all authentication operations to
        /// by default. The default forwarding logic will check the most specific ForwardAuthenticate/Challenge/Forbid/SignIn/SignOut 
        /// setting first, followed by checking the ForwardDefaultSelector, followed by ForwardDefault. The first non null result
        /// will be used as the target scheme to forward to.
        /// </summary>
        public string ForwardDefault { get; set; }

        /// <summary>
        /// If set, this specifies the target scheme that this scheme should forward AuthenticateAsync calls to.
        /// For example Context.AuthenticateAsync("ThisScheme") => Context.AuthenticateAsync("ForwardAuthenticateValue");
        /// Set the target to the current scheme to disable forwarding and allow normal processing.
        /// </summary>
        public string ForwardAuthenticate { get; set; }

      

    }
}
