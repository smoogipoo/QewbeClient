using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.Helpers
{
    internal static class Crypto
    {
        internal static string CalculateChecksum(byte[] data)
        {
            using (SHA256Managed sha = new SHA256Managed())
            {
                byte[] result = sha.ComputeHash(data);

                string ret = string.Empty;
                for (int i = 0; i < result.Length; i++)
                    ret += result[i].ToString("x2");
                return ret;
            }
        }

        internal static string CalculateChecksum(string data)
        {
            return CalculateChecksum(Encoding.UTF8.GetBytes(data));
        }

        internal static string CalculateChecksum(FileInfo data)
        {
            byte[] bytes = new byte[data.Length];
            data.OpenRead().Read(bytes, 0, bytes.Length);
            return CalculateChecksum(bytes);
        }
    }
}
