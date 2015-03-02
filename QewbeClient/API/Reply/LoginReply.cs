using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API.Reply
{
    [DataContract]
    internal class LoginReply : ClientReply
    {
        [DataMember]
        internal string Token { get; set; }
    }
}
