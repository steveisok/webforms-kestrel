using System;
using Microsoft.AspNetCore.Http;

namespace WebFormsHost.Hosting
{
    public abstract class WebSource : IDisposable
    {
        public abstract Worker CreateWorker(HttpContext context, WebFormsApplicationServer server);

        public abstract Type GetApplicationHostType();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
