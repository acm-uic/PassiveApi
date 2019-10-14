using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Net.Mail;
using Newtonsoft.Json;

namespace PassiveApi.Models
{
    public class User : Resource
    {
        private string dn;

        // AD Attributes
        private string userName { get; set; }
        private string userPassword { get; set; }
        private string firstName { get; set; }
        private string lastName { get; set; }
        private UInt32 UIN { get; set; }
        private string title { get; set; }
        private string membershipNumber { get; set; }
        private string memberType { get; set; }
        private string major { get; set; }
        private string college { get; set; }
        private MailAddress email { get; set; }
        private string phoneNumber { get; set; }
        private Dictionary<string, string> otherData;

        private static string GetDistinguishedName(string userName)
        {
            return AD.GetObjectDistinguishedName(userName, ObjectClass.user, ReturnType.distinguishedName);
        }

        public static string CreateUserInAD(User user)
        {
            if (AD.singleton == null)
            {
                AD.singleton = new AD();
            }
            string userDn = string.Empty;
            try
            {
                string month = "OU=" + Enum.GetName(typeof(Month), DateTime.Now.Month);
                string year = "OU=" + DateTime.Now.Year.ToString();

                string connectionString = AD.Host + "/" + AD.UsersOU + "," + AD.BaseDN;
                string monthString = year + "," + AD.UsersOU + "," + AD.BaseDN;
                string yearString = AD.UsersOU + "," + AD.BaseDN;

                new OU(year, yearString);
                new OU(month, monthString);


                string userPath = AD.Host + "/" + month + "," + year + "," + AD.UsersOU + "," + AD.BaseDN;
                DirectoryEntry dirEntry = AD.GetObjectDirectoryEntry(userPath);
                DirectoryEntry newUser = dirEntry.Children.Add("CN=" + user.userName, "user");

                newUser.Properties["sAMAccountName"].Value = user.userName;
                newUser.Properties["employeeID"].Value = user.UIN;
                newUser.Properties["department"].Value = user.major;
                newUser.Properties["company"].Value = user.college;
                newUser.Properties["title"].Value = user.title;
                newUser.Properties["mail"].Value = user.email;
                newUser.Properties["telephoneNumber"].Value = user.phoneNumber;
                newUser.Properties["givenName"].Value = user.firstName;
                newUser.Properties["sn"].Value = user.lastName;
                newUser.Properties["employeeNumber"].Value = user.membershipNumber;
                newUser.Properties["employeeType"].Value = user.memberType;
                newUser.Properties["description"].Value = user.otherData;
                newUser.CommitChanges();

                user.dn = newUser.Properties["distinguishedName"].Value.ToString();

                dirEntry.Close();
                newUser.Close();

                User.UpdatePasword(user.dn, user.userPassword);
                User.Unlock(user.dn);
                User.Enable(user.dn);
                Group.AddMember(user.dn, AD.PaidGroup);

            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
            return userDn;
        }

        public static void UpdatePasword(string userDn, string password)
        {
            try
            {
                DirectoryEntry user = AD.GetObjectDirectoryEntry(userDn);
                user.Invoke("SetPassword", new object[] { password });
                user.CommitChanges();

                user.CommitChanges();
                user.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }

        public static void Enable(string userDn)
        {
            try
            {
                DirectoryEntry user = AD.GetObjectDirectoryEntry(userDn);
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val & ~0x2;

                user.CommitChanges();
                user.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }
        public static void Disable(string userDn)
        {
            try
            {
                DirectoryEntry user = AD.GetObjectDirectoryEntry(userDn);
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val | 0x2;

                user.CommitChanges();
                user.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }

        public static void Unlock(string userDn)
        {
            try
            {
                DirectoryEntry user = AD.GetObjectDirectoryEntry(userDn);
                user.Properties["LockOutTime"].Value = 0;

                user.CommitChanges();
                user.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                throw E;
            }
        }

        public User(string userName, string userPassword, string firstName, string lastName, string UIN, string title, string membershipNumber, string memberType, string major, string college, string email, string phoneNumber, string otherData)
        {
            this.userName = userName;
            this.userPassword = userPassword;
            this.firstName = firstName;
            this.lastName = lastName;
            this.UIN = UInt32.Parse(UIN);
            this.title = title;
            this.membershipNumber = membershipNumber;
            this.memberType = memberType;
            this.major = major;
            this.college = college;
            this.email = new MailAddress(email);
            this.phoneNumber = phoneNumber;
            this.otherData = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherData);

            if (GetDistinguishedName(this.userName) == string.Empty)
            {
                this.dn = CreateUserInAD(this);
            }

        }

    }
}
