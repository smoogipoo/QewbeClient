using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API
{
    internal enum Response
    {
        //Generic information errors
        E_NOSERVICE = 0,
        E_NOCREDENTIALS = 1,
        E_INVALIDCREDENTIALS = 2,
        E_INVALIDTOKEN = 3,
        E_USEREXISTS = 4,
        E_INVALIDEMAIL = 5,
        E_EMPTYCREDENTIALS = 6,
        E_NOTLOGGEDIN = 7,
        E_FILEDOESNTEXIST = 8,

        //System errors
        E_INTERNALERROR = -100,
        E_INVALIDMETHOD = -3,
        E_INVALIDREQUESTTYPE = -2,
        E_NORETURN = -1,

        //Success responses
        R_USERACCOUNTCREATED = 400,
        R_TOKENCALLBACK = 401,
        R_LOGOUTSUCCESS = 402,
        R_DATACALLBACK = 403,
        R_NODATA = 404,
        R_INVALIDDATA = 405
    }
}
