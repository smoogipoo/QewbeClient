using Microsoft.Win32;
using QewbeClient.API.Reply;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QewbeClient.Http
{
    internal class FileNetRequest : NetRequest
    {
        private FileInfo file;

        internal FileNetRequest(string endpoint, FileInfo file, Action<object> callback = null, params string[] args)
            : base(endpoint, callback, args)
        {
            this.file = file;
        }

        protected override object Perform()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Endpoint);
            req.Method = @"POST";
            req.KeepAlive = true;

            using (Stream dataStream = new MemoryStream())
            {
                //HTTP header crap
                string boundary = @"----------------------------" + DateTime.Now.Ticks.ToString(@"x");

                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string fileTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

                //Initial boundary
                dataStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                //File header
                byte[] fileBytes = Encoding.ASCII.GetBytes(string.Format(fileTemplate, @"file", file.Name, getMimeType(file.Extension)));
                dataStream.Write(fileBytes, 0, fileBytes.Length);
                //File data
                fileBytes = new byte[file.Length];
                file.OpenRead().Read(fileBytes, 0, fileBytes.Length);
                dataStream.Write(fileBytes, 0, fileBytes.Length);
                //Closing boundary
                dataStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                req.ContentType = @"multipart/form-data; boundary=" + boundary;
                req.ContentLength = dataStream.Length;

                //Write header to request
                dataStream.Position = 0;
                using (Stream reqStream = req.GetRequestStream())
                {
                    byte[] data = new byte[dataStream.Length];
                    dataStream.Read(data, 0, data.Length);
                    reqStream.Write(data, 0, data.Length);
                }
            }

            using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream()))
                return sr.ReadToEnd();
        }

        private static string getMimeType(string ext)
        {
            string mimeType = @"application/unknown";
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(ext.ToLower());
            if (key != null && key.GetValue(@"Content Type") != null)
                mimeType = key.GetValue(@"Content Type").ToString();
            return mimeType;
        }
    }
}
