using Orleans.Authorization;
using System;
using System.Threading.Tasks;

namespace IGrains
{
    /// <summary>
    /// Orleans grain communication interface IHello
    /// </summary>
    public interface IAuthorize : Orleans.IGrainWithIntegerKey
    {
        [Authorize]
        Task<User> SayHelloAsync(string greeting);

       
    }
}
