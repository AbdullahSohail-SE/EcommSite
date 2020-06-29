using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALA
{
    public enum AuthenticationStatus
    {
        UserNotFound = 1,
        WrongPassword = 2,
        Authenticated = 3
    }
}
