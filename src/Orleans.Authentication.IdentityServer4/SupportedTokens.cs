// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Orleans.Authentication.IdentityServer4
{
    /// <summary>
    /// Supported token types
    /// </summary>
    public enum SupportedTokens
    {
        /// <summary>
        /// JWTs and reference tokens
        /// </summary>
        Both,

        /// <summary>
        /// JWTs only
        /// </summary>
        Jwt
    }
}