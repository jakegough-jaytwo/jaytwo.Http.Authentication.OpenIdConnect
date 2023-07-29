using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using jaytwo.MimeHelper;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace jaytwo.Http.Authentication.OpenIdConnect.Tests;

public class ClientCredentialsTokenProviderTests
{
    [Fact]
    public async Task GetTokenAsync_works()
    {
        // arrange
        var mockAccessTokenResponse = new AccessTokenResponse()
        {
            expires_in = 1,
            access_token = "orange",
            token_type = "apple",
        };

        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAccessTokenResponse);

        var tokenProvider = new ClientCredentialsTokenProvider(mockAccessTokenProvider.Object);

        // act
        var actualToken = await tokenProvider.GetTokenAsync(default);

        // assert
        Assert.Equal(mockAccessTokenResponse.access_token, actualToken);
    }

    [Fact]
    public async Task GetTokenAsync_does_not_call_GetAccessToken_again_if_token_is_fresh()
    {
        // arrange
        var mockAccessTokenResponse = new AccessTokenResponse()
        {
            access_token = "orange",
            token_type = "apple",
            Created = new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
            expires_in = 10,
            NowFactory = () => new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
        };

        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAccessTokenResponse);

        var tokenProvider = new ClientCredentialsTokenProvider(mockAccessTokenProvider.Object);

        // act
        await tokenProvider.GetTokenAsync(default);
        await tokenProvider.GetTokenAsync(default);
        await tokenProvider.GetTokenAsync(default);

        // assert
        mockAccessTokenProvider.Verify(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenAsync_calls_GetAccessToken_again_if_token_is_not_fresh()
    {
        // arrange
        var mockAccessTokenResponse = new AccessTokenResponse()
        {
            access_token = "orange",
            token_type = "apple",
            Created = new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
            expires_in = 10,
            NowFactory = () => new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
        };

        var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
        mockAccessTokenProvider
            .Setup(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAccessTokenResponse);

        var tokenProvider = new ClientCredentialsTokenProvider(mockAccessTokenProvider.Object);

        // act
        await tokenProvider.GetTokenAsync(default);
        await tokenProvider.GetTokenAsync(default);
        mockAccessTokenResponse.NowFactory = () => new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 20));
        await tokenProvider.GetTokenAsync(default);

        // assert
        mockAccessTokenProvider.Verify(x => x.GetAccessTokenAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
