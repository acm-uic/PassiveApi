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
        private string UserName { get; set; }
        private string UserPassword { get; set; }
        private string FirstName { get; set; }
        private string LastName { get; set; }
        private UInt32 UIN { get; set; }
        private string Title { get; set; }
        private string MembershipNumber { get; set; }
        private string MemberType { get; set; }
        private string Major { get; set; }
        private string College { get; set; }
        private MailAddress Email { get; set; }
        private string PhoneNumber { get; set; }
        private Dictionary<string, string> OtherData { get; set; }

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
                DirectoryEntry newUser = dirEntry.Children.Add("CN=" + user.UserName, "user");

                newUser.Properties["sAMAccountName"].Value = user.UserName;
                newUser.Properties["employeeID"].Value = user.UIN;
                newUser.Properties["department"].Value = user.Major;
                newUser.Properties["company"].Value = user.College;
                newUser.Properties["title"].Value = user.Title;
                newUser.Properties["mail"].Value = user.Email;
                newUser.Properties["telephoneNumber"].Value = user.PhoneNumber;
                newUser.Properties["givenName"].Value = user.FirstName;
                newUser.Properties["sn"].Value = user.LastName;
                newUser.Properties["employeeNumber"].Value = user.MembershipNumber;
                newUser.Properties["employeeType"].Value = user.MemberType;
                newUser.Properties["description"].Value = user.OtherData;
                newUser.CommitChanges();

                user.dn = newUser.Properties["distinguishedName"].Value.ToString();

                dirEntry.Close();
                dirEntry.Dispose();
                newUser.Close();
                newUser.Dispose();


                User.UpdatePasword(user.dn, user.UserPassword);
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
                user.Dispose();
            }
            catch (DirectoryServicesCOMException E)
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
                user.Dispose();
            }
            catch (DirectoryServicesCOMException E)
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
                user.Dispose();
            }
            catch (DirectoryServicesCOMException E)
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
                user.Dispose();
            }
            catch (DirectoryServicesCOMException E)
            {
                throw E;
            }
        }

        public User(string userName, string userPassword, string firstName, string lastName, string UIN, string title, string membershipNumber, string memberType, string major, string college, string email, string phoneNumber, string otherData)
        {
            this.UserName = userName;
            this.UserPassword = userPassword;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UIN = UInt32.Parse(UIN);
            this.Title = title;
            this.MembershipNumber = membershipNumber;
            this.MemberType = memberType;
            this.Major = major;
            this.College = college;
            this.Email = new MailAddress(email);
            this.PhoneNumber = phoneNumber;
            this.OtherData = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherData);

            if (GetDistinguishedName(this.UserName) == string.Empty)
            {
                this.dn = CreateUserInAD(this);
            }

        }

    }
}
