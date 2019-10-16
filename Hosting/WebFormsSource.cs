using System;
using Microsoft.AspNetCore.Http;

namespace WebFormsHost.Hosting
{
    public class WebFormsSource : WebSource
    {
        public WebFormsSource()
        {
        }

        public override Worker CreateWorker(HttpContext context, WebFormsApplicationServer server)
        {
            return new WebFormsWorker(context, server);
        }

        public override Type GetApplicationHostType()
        {
            return typeof(WebFormsApplicationHost);
        }
    }
}
