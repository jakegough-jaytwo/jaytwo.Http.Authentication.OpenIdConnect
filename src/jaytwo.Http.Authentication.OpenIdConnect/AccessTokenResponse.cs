using System;

namespace jaytwo.Http.Authentication.OpenIdConnect;

public class AccessTokenResponse
{
    public AccessTokenResponse()
    {
        Created = NowFactory();
    }

    public string access_token { get; set; }

    public int expires_in { get; set; }

    public string token_type { get; set; }

    public string scope { get; set; }

    internal DateTimeOffset Created { get; set; }

    internal Func<DateTimeOffset> NowFactory { get; set; } = () => DateTimeOffset.Now;

    public bool IsFresh()
    {
        var now = NowFactory();

        // half of expiration time patterned off of how DHCP refreshes based on TTL
        var expiration = Created.Add(TimeSpan.FromSeconds(expires_in / 2));

        return now <= expiration;
    }
}
