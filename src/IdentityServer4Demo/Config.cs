using IdentityServer4.Models;
using IdentityServer4.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4Demo
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "implicit",
                    ClientName = "Conformance : Implicit",

                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://op.certification.openid.net:60784/authz_cb" },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = { "openid", "profile", "email", "address", "phone" },
                    AllowAccessTokensViaBrowser = true,
                },
                new Client
                {
                    ClientId = "code",
                    ClientName = "Code",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RedirectUris = { "https://op.certification.openid.net:60784/authz_cb" },

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = { "openid", "profile", "email", "address", "phone" }
                }
            };
        }

        public static List<InMemoryUser> GetUsers()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "1",
                    Username = "bob",
                    Password = "bob",

                    Claims =
                    {
                        new Claim("name", "Bob Smith")
                    }

                }
            };
        }
    }
}