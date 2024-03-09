using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CertificateAuthApi.Security
{
    public class CertificateAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var certHeader = actionContext.Request.Headers.GetValues("X-Certificate").FirstOrDefault();
            if (string.IsNullOrEmpty(certHeader))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Missing Certificate"
                };
                return false;
            }

            try
            {
                var certBytes = Convert.FromBase64String(certHeader);
                var clientCertificate = new X509Certificate2(certBytes);
                // Perform validation on clientCertificate
                bool isValid = ValidateCertificate(clientCertificate);
                if (!isValid)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        ReasonPhrase = "Invalid Certificate"
                    };
                    return false;
                }
            }
            catch
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Invalid Certificate Format"
                };
                return false;
            }

            return true;
        }

        private bool ValidateCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                return false;
            }

            var trustedCertificatePath = ConfigurationManager.AppSettings["TrustedCertificatePath"];
            if (string.IsNullOrEmpty(trustedCertificatePath))
            {
                return false;
            }

            var trustedCertificatePassword = ConfigurationManager.AppSettings["TrustedCertificatePassword"];
            if (string.IsNullOrEmpty(trustedCertificatePassword))
            {
                return false;
            }

            var trustedCertificate = new X509Certificate2(trustedCertificatePath, trustedCertificatePassword);

            if (trustedCertificate == null)
            {
                return false;
            }

            if (DateTime.Now < certificate.NotBefore || DateTime.Now > certificate.NotAfter)
            {
                return false;
            }

            X509Chain chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(trustedCertificate);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            bool isChainValid = chain.Build(certificate);
            if (!isChainValid)
            {
                return false;
            }

            var isValid = chain
                        .ChainElements
                        .Cast<X509ChainElement>().Any(x => 
                            x.Certificate.Thumbprint == trustedCertificate.Thumbprint);
            return isValid;
        }

    }
}