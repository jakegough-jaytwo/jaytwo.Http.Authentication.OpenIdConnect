using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;

namespace jaytwo.Http.Authentication.OpenIdConnect.Tests;

public class TestServerContext : IDisposable
{
    private readonly string _url;
    private readonly IWebHost _webHost;

    private TestServerContext(string url, IWebHost webHost)
    {
        _url = url;
        _webHost = webHost;
    }

    public string Url => _url;

    public static TestServerContext Create(Func<string[], IWebHostBuilder> webHostBuilderFactory, int port = 0)
    {
        var portOrRandom = port > 0 ? port : GetRandomFreePort();
        var url = $"http://localhost:{portOrRandom}";

        var webHostBuilder = webHostBuilderFactory.Invoke(new string[] { });

        var webHost = webHostBuilder
            .UseUrls(url)
            .Build();

        try
        {
            webHost.Start();
            return new TestServerContext(url, webHost);
        }
        catch
        {
            webHost.Dispose();
            throw;
        }
    }

    public static TestServerContext Create(Func<IWebHostBuilder> webHostBuilderFactory) => Create(x => webHostBuilderFactory());

    public void Dispose()
    {
        _webHost.Dispose();
    }

    private static int GetRandomFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);
        tcpListener.Start();
        int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();
        return port;
    }
}
