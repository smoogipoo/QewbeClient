using QewbeClient.Helpers;
using QewbeClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.API.Reply
{
    [DataContract]
    public class UploadFile
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
    }
}
