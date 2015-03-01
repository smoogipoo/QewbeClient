using QewbeClient.Helpers;
using QewbeClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QewbeClient.API
{
    [DataContract]
    public class File
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public Int64 Uploaded { get; set; }
        [DataMember(Name = "Hash")]
        public string Checksum { get; set; }

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

    [DataContract]
    public class Reply
    {
        [DataMember]
        public bool OK { get; set; }
        [DataMember]
        public Response Response { get; set; }
        [DataMember]
        public string Error { get; set; }
    }

    [DataContract]
    public class CreateAccountReply : Reply
    {
        [DataMember]
        public string Username { get; set; }
    }

    [DataContract]
    public class LoginReply : Reply
    {
        [DataMember]
        public string Token { get; set; }
    }

    [DataContract]
    public class UploadFileReply : Reply
    {
        [DataMember]
        public File File { get; set; }
    }

    [DataContract]
    public class GetFilesReply : Reply
    {
        [DataMember]
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
