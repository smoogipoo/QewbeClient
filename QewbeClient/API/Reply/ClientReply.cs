using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API.Reply
{
    [DataContract]
    internal class ClientReply
    {
        [DataMember]
        internal bool OK { get; set; }
        [DataMember]
        internal Response Response { get; set; }
        [DataMember]
        internal string Error { get; set; }
    }
}
