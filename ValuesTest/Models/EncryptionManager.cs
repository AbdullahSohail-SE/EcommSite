using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Security;

namespace Web.Models
{
    public class EncryptionManager
    {
        private const string Purpose = "Authentication Token";

        public static string Protect(string unprotectedText)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedText);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);
            var protectedText = Convert.ToBase64String(protectedBytes);
            return protectedText;
        }

        public static string Unprotect(string protectedText)
        {
            try
            {
                var protectedBytes = Convert.FromBase64String(protectedText);
                var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
                var unprotectedText = Encoding.UTF8.GetString(unprotectedBytes);
                return unprotectedText;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}