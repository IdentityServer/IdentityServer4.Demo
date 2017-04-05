using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer4Demo
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _env = env;

            if (_env.IsDevelopment())
            {
                var serilog = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                    .CreateLogger();

                loggerFactory.AddSerilog(serilog);
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var builder = services.AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(TestUsers.Users);

            // demo versions
            services.AddTransient<IRedirectUriValidator, DemoRedirectValidator>();
            services.AddTransient<ICorsPolicyService, DemoCorsPolicy>();

            if (_env.IsDevelopment())
            {
                builder.AddTemporarySigningCredential();
            }
            else
            {
                builder.AddSigningCredential("98D3ACF057299C3745044BE918986AD7ED0AD4A2", StoreLocation.CurrentUser, nameType: NameType.Thumbprint);
            }
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/api", apiApp =>
            {
                apiApp.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = "https://demo.identityserver.io",
                    AutomaticAuthenticate = true,

                    ApiName = "api"
                });

                apiApp.UseMvc();
            });

            app.UseIdentityServer();

            // middleware for google authentication
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
                ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}