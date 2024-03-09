using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace CertificateAuthClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "https://localhost:44359/api/hello";
            string certificatePath = ConfigurationManager.AppSettings["ClientCertificatePath"];
            var certificate = new X509Certificate2(certificatePath);
            string certBase64 = Convert.ToBase64String(certificate.GetRawCertData());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Certificate", certBase64);
            var response = client.GetAsync(url).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);
            Console.ReadKey();
        }
    }
}
