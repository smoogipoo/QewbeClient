using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API.Reply
{
    [DataContract]
    internal class UploadFileReply : ClientReply
    {
        [DataMember]
        internal UploadFile File { get; set; }
    }
}
