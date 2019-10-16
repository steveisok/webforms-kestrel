using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebFormsHost.Hosting
{
    public class WebFormsApplicationServer : MarshalByRefObject
    {
        private ILogger<WebFormsApplicationServer> _log;
        private WebSource _webSource;
        private WebFormsApplicationHost _host;

        public WebFormsApplicationServer(ILogger<WebFormsApplicationServer> logger, WebSource webSource) {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _log = logger;
            _webSource = webSource;

            _host = new WebFormsApplicationHost();
            _host.Server = this;
        }

        //private HttpContext _httpContext;
        private string _contentRoot;

        public Task OnRequestStarted(HttpContext context) {
            _log.LogCritical("I got here!");

            try
            {
                context.Request.EnableRewind();

                var worker = _webSource.CreateWorker(context, this);
                return worker.Run(null);
            }
            catch (Exception e)
            {
                // for now - need to figure out how to best translate errors here....
                // how much do we rely on WF versus aspnet core
                // I think it needs to be WF contained.
                return Task.FromException(e);
            }
        }

        public WebFormsApplicationHost CreateHost() {
            // probably going away... because the host is over top the whole thing :-)
            return _host;
        }

        public void Stop() {
            System.Web.HttpRuntime.UnloadAppDomain();
        }

        public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            _log.LogError("Handling exception type {0}", ex.GetType().Name);
            _log.LogError("Message is {0}", ex.Message);
            _log.LogError("IsTerminating is set to {0}", e.IsTerminating);

            if (e.IsTerminating)
                _log.LogError(ex, "Details");
        }

        public string PhysicalRoot { get; set; }
    }
}
