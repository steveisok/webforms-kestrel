using System;
using WebFormsHost.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebFormsServiceCollectionExtensions
    {
        public static void AddWebForms(this IServiceCollection services) {
            services.AddSingleton<WebSource>(new WebFormsSource());
            services.AddSingleton<WebFormsApplicationServer>();
        }
    }
}
