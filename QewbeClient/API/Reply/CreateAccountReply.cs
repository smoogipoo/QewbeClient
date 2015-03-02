using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API.Reply
{
    [DataContract]
    internal class CreateAccountReply : ClientReply
    {
        [DataMember]
        internal string Username { get; set; }
    }
}
