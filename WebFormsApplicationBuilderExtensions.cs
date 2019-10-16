using System;
using System.Diagnostics;
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
    public static class WebFormsApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebForms(this IApplicationBuilder app, IHostingEnvironment env) {
            return Create(app, env.ContentRootPath);
        }

        public static IApplicationBuilder UseWebForms(this IApplicationBuilder app)
        {
            return Create(app, System.IO.Directory.GetCurrentDirectory());
        }

        private static IApplicationBuilder Create(IApplicationBuilder app, string physicalRoot) {
            var appServer = app.ApplicationServices.GetService<WebFormsApplicationServer>();
            appServer.PhysicalRoot = physicalRoot;

            app.Run(appServer.OnRequestStarted);

            return app;
        }
    }
}
