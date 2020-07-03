using System;
using Newtonsoft.Json;

namespace jaytwo.Http.Authentication.OpenIdConnect
{
    public class AccessTokenResponse
    {
        public AccessTokenResponse()
        {
            Created = NowDelegate();
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        internal DateTimeOffset Created { get; set; }

        internal TimeSpan Threshold { get; set; } = TimeSpan.FromSeconds(60);

        internal Func<DateTimeOffset> NowDelegate { get; set; } = () => DateTimeOffset.Now;

        public bool IsFresh()
        {
            var expiration = Created
                .Add(TimeSpan.FromSeconds(ExpiresIn))
                .Subtract(Threshold);

            var now = NowDelegate();

            return now <= expiration;
        }
    }
}
