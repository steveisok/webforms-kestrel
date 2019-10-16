using System;
using System.Web;

namespace WebFormsHost.Hosting
{
    public class WebFormsDefaultDomainManager : AppDomainManager
    {
        public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
        {
            base.InitializeNewDomain(appDomainInfo);

            appDomainInfo.PrivateBinPathProbe = "*";
            appDomainInfo.ShadowCopyFiles = "true";
            appDomainInfo.ConfigurationFile = "web.config";
            appDomainInfo.DisallowCodeDownload = true;
        }
    }
}
