using QewbeClient.Helpers;
using QewbeClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QewbeClient.API
{
    [Serializable]
    public class File
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Type { get; set; }
        public Int64 Uploaded { get; set; }
        public string Checksum { get; set; }

        [NonSerialized]
        public string InternalName;

        internal event UploadResult UploadSucceeded;
        internal event UploadResult UploadFailed;

        public void BeginUpload()
        {
            HttpClient.SendRequest(new FileNetRequest(this, uploadCompleted));
        }

        private void uploadCompleted(object res)
        {
            if ((bool)res)
            {
                if (UploadSucceeded != null)
                    UploadSucceeded(this);
            }
            else
            {
                if (UploadFailed != null)
                    UploadFailed(this);
            }
        }
    }

    [Serializable]
    public class Reply
    {
        public bool OK { get; set; }
        public Response Response { get; set; }
    }

    [Serializable]
    public class CreateAccountReply : Reply
    {
        public string Username { get; set; }
    }

    [Serializable]
    public class LoginReply : Reply
    {
        public string Token { get; set; }
    }

    [Serializable]
    public class UploadFileReply : Reply
    {
        public File File { get; set; }
    }

    [Serializable]
    public class GetFilesReply : Reply
    {
        public File[] Files { get; set; }
    }

    public enum Response
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
