using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PassiveApi.SSO
{
    public static class SSOUtility
    {
        /// <summary>
        /// Function that retrieves the admin groups
        /// from an environment variable named "PASSIVE_ADMIN_GROUPS"
        /// it assumes the value is a comma separated string
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAdminGroups()
        {
            string adminGroups = System.Environment.GetEnvironmentVariable("PASSIVE_ADMIN_GROUPS");
            return adminGroups.Split(',').ToList();
        }

        /// <summary>
        /// Function that checks user claims
        /// to determine if user is an admin
        /// </summary>
        /// <param name="samlResponse">Parsed xml object containing saml claims</param>
        /// <returns>True if user is an admin, else false</returns>
        public static bool IsAdminUser(XElement samlResponse, IEnumerable<string> adminGroups)
        {
            // TODO Extract nameId into Extract Claims Functions
            var nameId = samlResponse.Descendants().Where(n => n.Name.LocalName.Contains("NameID"));

            // TODO Check other ways of sending adfs response so this is "neater"
            var groupAttributes = samlResponse.Descendants().Where(c => c.FirstAttribute != null && c.FirstAttribute.Value.Contains("role")).Descendants().Select(d => d.Value).Intersect(adminGroups);
            return groupAttributes.Any() ? true : false;
        }

    }
}
