using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using jaytwo.AspNetCore.DataProtection;

namespace SampleIdentityServer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddDataProtection()
            .UseStaticKeyFromSeed("abc");

        services.AddIdentityServer()
            .AddSigningCredential(GetCert())
            .AddInMemoryClients(Config.GetClients())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryApiScopes(Config.GetApiScopes());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseIdentityServer();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private X509Certificate2 GetCert()
    {
        var key = "SampleIdentityServer.EmbeddedResources.certificate.p12";

        using (var resourceStream = this.GetType().Assembly.GetManifestResourceStream(key))
        using (var memoryStream = new MemoryStream())
        {
            resourceStream.CopyTo(memoryStream);
            return new X509Certificate2(memoryStream.ToArray());
        }
    }
}
