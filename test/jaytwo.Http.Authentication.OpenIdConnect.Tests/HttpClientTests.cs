using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using jaytwo.FluentHttp;
using Xunit;
using Xunit.Abstractions;

namespace jaytwo.Http.Authentication.OpenIdConnect.Tests
{
    public class HttpClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TestServerContext _identityServerContext;
        private readonly TestServerContext _webApiServerContext;
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _clientCredentialsTokenProvider;

        public HttpClientTests(ITestOutputHelper output)
        {
            _output = output;
            _httpClient = new HttpClient();
            _identityServerContext = TestServerContext.Create(SampleIdentityServer.Program.CreateWebHostBuilder);
            _webApiServerContext = TestServerContext.Create(() => SampleWebApi.Program.CreateWebHostBuilder(new[] { $"--Authentication:IdentityServer:Authority", _identityServerContext.Url }));

            _clientCredentialsTokenProvider = new ClientCredentialsTokenProvider(_httpClient, new ClientCredentialsTokenConfig()
            {
                ClientId = "myclientid",
                ClientSecret = "secret",
                Resource = "api1",
                TokenUrl = $"{_identityServerContext.Url}/connect/token",
            });
        }

        [Fact]
        public async Task GetTokenAsync_works()
        {
            // arrange

            // act
            var token = await _clientCredentialsTokenProvider.GetTokenAsync();

            // assert
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task WithTokenAuthentication_works()
        {
            // arrange

            // act
            var response = await _httpClient.SendAsync(request =>
            {
                request
                    .WithMethod(HttpMethod.Get)
                    .WithBaseUri(_webApiServerContext.Url)
                    .WithUriPath("/api/echo/userInfo")
                    .WithTokenAuthentication(_clientCredentialsTokenProvider)
                    ;
            });

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Without_TokenAuthentication_returns_401()
        {
            // arrange

            // act
            var response = await _httpClient.SendAsync(request =>
            {
                request
                    .WithMethod(HttpMethod.Get)
                    .WithBaseUri(_webApiServerContext.Url)
                    .WithUriPath("/api/echo/userInfo")
                    ;
            });

            // assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _identityServerContext.Dispose();
            _webApiServerContext.Dispose();
        }
    }
}
