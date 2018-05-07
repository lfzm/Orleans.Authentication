using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Orleans.Authentication
{
    public class AuthenticateContext
    {
        public AuthenticateContext (IServiceProvider serviceProvider)
        {
            this.RequestServices = serviceProvider;
            this.CancellationTokenSource = new CancellationTokenSource();
        }
        public IServiceProvider RequestServices { get;  }
        public CancellationTokenSource CancellationTokenSource { get; }
        public string GetHeaders(string key)
        {
            var value= RequestContext.Get(key);
            if (value != null)
                return value.ToString();
            else
                return null;
        }
    }
}
