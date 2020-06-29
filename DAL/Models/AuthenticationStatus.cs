using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Models
{
    public enum AuthenticationStatus
    {
        UserNotFound=1,
        WrongPassword=2,
        Authenticated=3
    }
}