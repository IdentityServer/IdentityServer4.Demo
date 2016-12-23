using IdentityModel;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Linq;

namespace IdentityServer4Demo
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var builder = services.AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(TestUsers.Users);

            services.AddTransient<IRedirectUriValidator, DemoRedirectValidator>();

            if (_env.IsDevelopment())
            {
                builder.AddTemporarySigningCredential();
            }
            else
            {
                var cert = X509.CurrentUser.My.Thumbprint.Find("98D3ACF057299C3745044BE918986AD7ED0AD4A2", validOnly: false).FirstOrDefault();
                builder.AddSigningCredential(cert);
            }
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var serilog = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            loggerFactory.AddSerilog(serilog);
            app.UseDeveloperExceptionPage();

            //app.Map("/api", apiApp =>
            //{
            //    app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            //    {
            //        Authority = "https://identityserver4demo.azurewebsites.net",

            //        ApiName = "api",
            //        ApiSecret = "secret"
            //    });

            //    app.UseMvc();
            //});

            app.UseIdentityServer();

            // cookie middleware for temporarily storing the outcome of the external authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            // middleware for google authentication
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
                ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}