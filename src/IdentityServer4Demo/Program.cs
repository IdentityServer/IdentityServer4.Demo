using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace IdentityServer4Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer";

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseSerilog((ctx, config) =>
                    {
                        config.MinimumLevel.Debug()
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                            .Enrich.FromLogContext();

                        if (ctx.HostingEnvironment.IsDevelopment())
                        {
                            config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
                        }
                        else if (ctx.HostingEnvironment.IsProduction())
                        {
                            config.WriteTo.File(@"D:\home\LogFiles\Application\identityserver.txt",
                                fileSizeLimitBytes: 1_000_000,
                                rollOnFileSizeLimit: true,
                                shared: true,
                                flushToDiskInterval: TimeSpan.FromSeconds(1));
                        }

                        var key = ctx.Configuration["AI_KEY"];
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            config.WriteTo.ApplicationInsightsTraces(key);
                        }
                    })
                    .Build();
        }
    }
}
