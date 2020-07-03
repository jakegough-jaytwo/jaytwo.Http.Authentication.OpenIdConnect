using System;

namespace jaytwo.Http.Authentication.OpenIdConnect
{
    public class ClientCredentialsTokenConfig
    {
        public string TokenUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Resource { get; set; }
    }
}
