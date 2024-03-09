using System.Web.Http;

namespace CertificateAuthApi.Controllers
{
    [Security.CertificateAuthorize]
    public class HelloController : ApiController
    {
        [HttpGet]
        public string Get(string Name = "World")
        {
            Name = string.IsNullOrEmpty(Name) ? "World" : Name;
            return "Hello, " + Name + "!";
        }
    }
}
