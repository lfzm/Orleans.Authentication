using Orleans.Authorization;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Authentication
{
    /// <summary>
    /// 添加授权过滤器
    /// </summary>
    public static class OrleansAuthorizationISiloHostBuilderExtensions
    {
        public static ISiloHostBuilder AddAuthorizationFilter(this ISiloHostBuilder builder)
        {
            builder.AddIncomingGrainCallFilter<AuthorizeGrainFiltered>();
            return builder;
        }

        public static ISiloHostBuilder AddAuthorizationFilter<TGrainCallFilter>(this ISiloHostBuilder builder) 
            where TGrainCallFilter: class,IIncomingGrainCallFilter
        {
            builder.AddIncomingGrainCallFilter<TGrainCallFilter>();
            return builder;
        }
    }
}
