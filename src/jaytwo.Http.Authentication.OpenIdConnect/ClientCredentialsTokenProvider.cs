using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jaytwo.Http.Authentication.OpenIdConnect;

public class ClientCredentialsTokenProvider : IBearerTokenProvider
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly IAccessTokenProvider _accessTokenProvider;

    private AccessTokenResponse _cachedAccessToken;

    public ClientCredentialsTokenProvider(IHttpClient httpClient, ClientCredentialsTokenConfig config)
         : this(new ClientCredentialsAccessTokenProvider(httpClient, config))
    {
    }

    public ClientCredentialsTokenProvider(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync();
        try
        {
            var refreshRequired = !_cachedAccessToken?.IsFresh() ?? true;
            if (refreshRequired)
            {
                _cachedAccessToken = await _accessTokenProvider.GetAccessTokenAsync(cancellationToken);
            }

            return _cachedAccessToken.access_token;
        }
        catch
        {
            _cachedAccessToken = null;
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
