using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace QewbeClient.API
{
    internal static class Serializer
    {
        internal static T Deserialize<T>(string data)
        {
            using (StreamReader sw = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(data))))
            {
                DataContractJsonSerializer dcs = new DataContractJsonSerializer(typeof(T));
                return (T)dcs.ReadObject(sw.BaseStream);
            }
        }

        internal static string Serialize(object data)
        {
            DataContractJsonSerializer dcs = new DataContractJsonSerializer(data.GetType());
            using (MemoryStream ms = new MemoryStream())
            using (StreamReader sr = new StreamReader(ms))
            {
                dcs.WriteObject(ms, data);
                return sr.ReadToEnd();
            }
        }
    }
}
