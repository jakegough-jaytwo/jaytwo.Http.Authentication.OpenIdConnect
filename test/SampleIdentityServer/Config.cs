using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleIdentityServer;

public class Config
{
    public static IEnumerable<Client> GetClients()
    {
        yield return new Client
        {
            ClientId = "myclientid",
            RequireClientSecret = true,
            ClientSecrets = { new Secret("secret".Sha256()) },
            ClientName = "client credentials demo",
            RequireConsent = false,
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "scope.api1" }, // scope can mean access to anothe system by referencing the id of a resource
        };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
        yield return new ApiResource("resource.api1", "My Demo API 1")
        {
            Scopes = { "scope.api1" }
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        yield return new ApiScope(name: "scope.api1", displayName: "for API1.");
    }
}
