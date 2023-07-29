using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace jaytwo.Http.Authentication.OpenIdConnect;

public class ClientCredentialsAccessTokenProvider : IAccessTokenProvider
{
    private readonly IHttpClient _httpClient;
    private readonly ClientCredentialsTokenConfig _config;

    public ClientCredentialsAccessTokenProvider(IHttpClient httpClient, ClientCredentialsTokenConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "grant_type", "client_credentials" },
            { "client_id", _config.ClientId },
            { "client_secret", _config.ClientSecret },
            { "resource", _config.Resource },
            { "scope", _config.Scope },
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenUrl);
        request.Content = content;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<AccessTokenResponse>(responseString);

        return result;
    }
}
