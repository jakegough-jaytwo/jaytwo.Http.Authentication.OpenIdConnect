using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using jaytwo.MimeHelper;
using Moq;
using Xunit;

namespace jaytwo.Http.Authentication.OpenIdConnect.Tests;

public class ClientCredentialsAccessTokenProviderTests
{
    [Fact]
    public async Task GetAccessTokenAsync_works()
    {
        // arrange
        var config = new ClientCredentialsTokenConfig()
        {
            TokenUrl = "https://example.com/oidc/access_token",
        };

        var accessToken = "howdy";
        var expiresIn = 777;
        var tokenType = "banana";

        var response = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(
                $@"
                    {{
                        ""access_token"": ""{accessToken}"",
                        ""expires_in"": {expiresIn},
                        ""token_type"": ""{tokenType}""
                    }}",
                Encoding.UTF8,
                MediaType.application_json),
        };

        var mockClient = new Mock<IHttpClient>();
        mockClient
            .Setup(x => x.SendAsync(
                It.Is<HttpRequestMessage>(x => x.RequestUri.OriginalString == config.TokenUrl),
                It.IsAny<HttpCompletionOption?>(),
                It.IsAny<CancellationToken?>()))
            .ReturnsAsync(response);

        var tokenProvider = new ClientCredentialsAccessTokenProvider(mockClient.Object, config);

        // act
        var accessTokenResponse = await tokenProvider.GetAccessTokenAsync(default);

        // assert
        Assert.Equal(accessToken, accessTokenResponse.access_token);
        Assert.Equal(expiresIn, accessTokenResponse.expires_in);
        Assert.Equal(tokenType, accessTokenResponse.token_type);
    }
}
