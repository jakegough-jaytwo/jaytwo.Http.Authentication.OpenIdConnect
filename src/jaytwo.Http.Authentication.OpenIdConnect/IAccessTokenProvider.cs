using System;
using System.Threading.Tasks;

namespace jaytwo.Http.Authentication.OpenIdConnect
{
    public interface IAccessTokenProvider
    {
        Task<AccessTokenResponse> GetAccessTokenAsync();
    }
}
