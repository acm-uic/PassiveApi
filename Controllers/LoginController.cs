using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PassiveApi.SSO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace PassiveApi.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration config, ILogger<LoginController> logger)
        {
            _logger = logger;
            _configuration = config;
        }

        
        /// <summary>
        /// API Route that redirects user to AD FS Login page
        /// </summary>
        //[Route("/[controller]")]
        [HttpGet("login")]
        public void LoginUser()
        {
            // TODO:
            // Checking for cookie should happen at root level
            // redirect to AD FS Login
            // ADFS_HOST url instead of hard code
            // /adfs/ls/idpinitiatedsignon
            string redirectURL = System.Environment.GetEnvironmentVariable("PASSIVE_ADFS_HOST");
            
            Response.Redirect(redirectURL);
        }

        /// <summary>
        /// Route that is hit when SSO 
        /// returns the post request with
        /// claims
        /// </summary>
        /// <returns></returns>
        [HttpPost("SignIn")]
        public IActionResult PostSignIn()
        {
            string samlResponse = ExtractSamlResponse();

            XElement saml = XElement.Parse(samlResponse);
            IEnumerable<string> adminGroups = SSOUtility.GetAdminGroups();
            bool isAdmin = SSOUtility.IsAdminUser(saml, adminGroups);

            // TODO: Give User Cookie or Token
            return isAdmin ? Redirect("./") : Redirect("./");
        }

        /// <summary>
        /// Function that parses the incoming
        /// SAML post request.
        /// </summary>
        /// <returns>string representing the decoded SAML response</returns>
        private string ExtractSamlResponse()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var samlResponse = reader.ReadToEnd();
                var samlDecoded = HttpUtility.UrlDecode(samlResponse);
                samlDecoded = samlDecoded.Replace("SAMLResponse=", "");
                var saml64Decoded = Convert.FromBase64String(samlDecoded);
                var deflated = Encoding.UTF8.GetString(saml64Decoded);
                return deflated;
            }
        }
    }
}
