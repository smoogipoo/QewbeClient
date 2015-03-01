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
    internal class NetRequest
    {
        protected const int MAX_ATTEMPTS = 5;

        protected string Endpoint;
        protected Action<object> Callback;

        protected int UploadAttempts;

        internal NetRequest(string endpoint, Action<object> callback, params string[] args)
        {
            Endpoint = string.Format(endpoint, args);
            Callback = callback;
        }

        internal virtual void Perform()
        {
            ++UploadAttempts;

            HttpWebRequest req = HttpWebRequest.CreateHttp(Endpoint);
            req.GetResponseAsync().ContinueWith(delegate(Task<WebResponse> resp)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(resp.Result.GetResponseStream()))
                        Callback(sr.ReadToEnd());
                }
                catch
                {
                    if (UploadAttempts == MAX_ATTEMPTS)
                        return;

                    Thread.Sleep(5000);
                    Perform();
                }
            });
        }
    }
}
