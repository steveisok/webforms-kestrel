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
    class MainClass
    {
        public static void Main(string[] args)
        {
            // so, let's see if we can set up a new app domain 
            // and create an instance there that starts everything up.
            // The idea here is that we can shuffle kestrel starting AND webforms hosting environment setup 
            // in the other appdomain.  That gives us a *shot* at replicating some of the webforms behavior 
            // while briding kestrel.

            var contentRoot = System.IO.Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar));

            var dStartup = System.Web.Hosting.ApplicationHost.CreateApplicationHost(
                typeof(WebFormsStartup), "/", contentRoot, true) as WebFormsStartup;

            dStartup.Start(contentRoot);

            //var d = AppDomain.CreateDomain("WebFormsDomain");
            //var dStartup = Activator.CreateInstance(d, "WebFormsHost", "WebFormsHost.WebFormsStartup").Unwrap() as WebFormsStartup;

            //dStartup.Start();


            /*
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                    logging.AddFilter("System", LogLevel.Debug);
                    logging.AddFilter("Default", LogLevel.Debug);
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
            */
        }

    }

    /*
    public class Startup {
        private static readonly byte[] _helloWorldBytes = System.Text.Encoding.UTF8.GetBytes("Hello, World!");
        private ILogger _log;

        public Startup(ILoggerFactory factory) {
            _log = factory.CreateLogger("Steve");
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddWebForms();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            app.UseWebForms();

            var appLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            appLifetime.ApplicationStopping.Register(
                () => { OnShutdown(app.ApplicationServices.GetService<WebFormsApplicationServer>()); }
            );
        }

        private void OnShutdown(WebFormsApplicationServer server) {
            server.Stop();
        }
    }
    */
}
