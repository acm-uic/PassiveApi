using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace PassiveApi.Models
{
    public class Group : Resource
    {
        public Group()
        {

        }

        public static void AddMember(string userDn, string groupDn)
        {
            try
            {
                DirectoryEntry dirEntry = AD.GetObjectDirectoryEntry(groupDn);
                dirEntry.Properties["member"].Add(userDn);

                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }

        public static void RemoveMember(string userDn, string groupDn)
        {
            try
            {
                DirectoryEntry dirEntry = AD.GetObjectDirectoryEntry(groupDn);
                dirEntry.Properties["member"].Remove(userDn);

                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }
    }
}
