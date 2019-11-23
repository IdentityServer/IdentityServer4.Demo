using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Demo
{
    public class Config
    {
        private static string ClientSecret { get; } = "secret"; 
        
        private static readonly string RabbitResourceServerId = "rabbitmq";
        private static readonly string Rabbit_ManagementWebsite_Scope = $"{RabbitResourceServerId}.managementwebsite";
        private static readonly string Rabbit_Configure_All_Scope = $"{RabbitResourceServerId}.configure:*/*";
        private static readonly string Rabbit_Read_All_Scope = $"{RabbitResourceServerId}.read:*/*";
        private static readonly string Rabbit_Write_All_Scope = $"{RabbitResourceServerId}.write:*/*";

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),                
                new IdentityResource
                {
                    Name = "names",
                    UserClaims = {"name"},
                    DisplayName = "Access to user names"
                },
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = {"role"},
                    DisplayName = "Access to user roles"
                }
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "Demo API")
                {
                    ApiSecrets = { new Secret("secret".Sha256()) }
                },

                // PolicyServer demo
                new ApiResource("policyserver.runtime"),
                new ApiResource("policyserver.management"),


                new ApiResource("rabbitmq")
                {
                    UserClaims = new [] { "names", "roles", "openid"},
                    ApiSecrets = {
                        new Secret(ClientSecret.Sha256())
                    },
                    Scopes =
                    {
                        new Scope(Rabbit_ManagementWebsite_Scope),
                        new Scope(Rabbit_Configure_All_Scope),
                        new Scope(Rabbit_Read_All_Scope),
                        new Scope(Rabbit_Write_All_Scope),
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                #region RabbitMq_ManagementWebsite

                new Client
                {
                    ClientId = $"GitHub.RabbitMq.ManagementWebsite.Local",
                    ClientName = "Implicit Client",
                    ClientSecrets = new [] { new Secret(ClientSecret.Sha256()) },
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 900,
                    RedirectUris =
                        GetRedirectUris("localhost", 15672),
                    PostLogoutRedirectUris =
                        GetPostLogoutRedirectUris("localhost", 15672),
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "names",
                        "roles",
                        "offline_access",
                        Rabbit_ManagementWebsite_Scope },
                },

                #endregion

                #region RabbitMq_Maintanance

                new Client()
                {
                    ClientId = $"RabbitMq.ReadWriteAll",
                    ClientName = $"RabbitMq.Maintanance",
                    ClientSecrets = new [] { new Secret(ClientSecret.Sha256()) },
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 3600,
                    IdentityTokenLifetime = 3600,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 3600,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "names",
                        "roles",
                        "offline_access",
                        Rabbit_Configure_All_Scope,
                        Rabbit_Read_All_Scope,
                        Rabbit_Write_All_Scope,
                    }
                },

                #endregion

            };
        }
        private static string GetHostName(
            string environment, int servicePortNumber)
        {
            return $"{environment}:{servicePortNumber}";
        }

        private static ICollection<string> GetRedirectUris(
            string environment, int localPortNumber)
        {
            return new List<string>()
            {
                $"http://{GetHostName(environment, localPortNumber)}/postaccess.html",
                $"http://{GetHostName(environment, localPortNumber)}/callback.html",
                $"http://{GetHostName(environment, localPortNumber)}/silent.html",
            };
        }

        private static ICollection<string>  GetPostLogoutRedirectUris(
            string environment, int localPortNumber)
        {
            return new List<string>()
            {
                $"http://{GetHostName(environment, localPortNumber)}/index.html",
            };
        }
    }
}