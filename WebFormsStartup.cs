using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebFormsHost.Hosting;

namespace WebFormsHost
{
    public class WebFormsStartup : MarshalByRefObject
    {
        public WebFormsStartup()
        {
        }

        public void Start(string contentRoot) {
            // need to know the VPath ahead of time and the physical path ahead of time for the WF AppDomain
            // need to figure out the hosting 
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(contentRoot)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                    logging.AddFilter("System", LogLevel.Debug);
                    logging.AddFilter("Default", LogLevel.Debug);
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public class Startup
    {
        private static readonly byte[] _helloWorldBytes = System.Text.Encoding.UTF8.GetBytes("Hello, World!");
        private ILogger _log;

        public Startup(ILoggerFactory factory)
        {
            _log = factory.CreateLogger("Steve");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebForms();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseWebForms(env);

            var appLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            appLifetime.ApplicationStopping.Register(
                () => { OnShutdown(app.ApplicationServices.GetService<WebFormsApplicationServer>()); }
            );
        }

        private void OnShutdown(WebFormsApplicationServer server)
        {
            server.Stop();
        }
    }
}
