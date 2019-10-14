using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace PassiveApi.Models
{
    public class OU :  Resource
    {
        public OU(string name, string path)
        {
            try
            {
                DirectoryEntry ouEntry = new DirectoryEntry(AD.Host + "/" + name + "," + path, AD.User, AD.Password);
                var test = ouEntry.Guid;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException)
            {
                DirectoryEntry baseEntry = new DirectoryEntry(AD.Host + "/" + path, AD.User, AD.Password);
                baseEntry = baseEntry.Children.Add(name, "OrganizationalUnit");
                baseEntry.CommitChanges();
                baseEntry.Close();
            }
        }
    }
}
