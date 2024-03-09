using System.Web.Configuration;
using System.Web.Http;

namespace CertificateAuthApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Change this to True for Release Deployments
            // Change this to False for Development
            bool protectSettings = true;

            var config = WebConfigurationManager.OpenWebConfiguration("~/");
            var section = config.GetSection("appSettings");
            if (section != null)
            {
                if (protectSettings && !section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.Save();
                }

                if (!protectSettings && section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.UnprotectSection();
                    config.Save();
                }
            }

        }
    }
}
