using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace jaytwo.Http.Authentication.OpenIdConnect.Tests;

public class AccessTokenResponseTests
{
    [Fact]
    public void IsFresh_true_under_threshold()
    {
        // arrange
        var tokenResponse = new AccessTokenResponse()
        {
            Created = new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
            expires_in = 10,
            NowFactory = () => new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
        };

        // act
        var isFresh = tokenResponse.IsFresh();

        // assert
        Assert.True(isFresh);
    }

    [Fact]
    public void IsFresh_false_over_threshold()
    {
        // arrange
        var tokenResponse = new AccessTokenResponse()
        {
            Created = new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 00)),
            expires_in = 10,
            NowFactory = () => new DateTimeOffset(new DateTime(2020, 6, 9, 12, 00, 6)),
        };

        // act
        var isFresh = tokenResponse.IsFresh();

        // assert
        Assert.False(isFresh);
    }

    [Fact]
    public void IsFresh_true_when_created()
    {
        // arrange
        var tokenResponse = new AccessTokenResponse()
        {
            expires_in = 100, // anything greater than the default timespan of 60
        };

        // act
        var isFresh = tokenResponse.IsFresh();

        // assert
        Assert.True(isFresh);
    }
}
