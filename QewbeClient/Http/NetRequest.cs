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
        private const int MAX_ATTEMPTS = 5;

        protected string Endpoint;
        private Action<object> callback;

        private int uploadAttempts;

        internal NetRequest(string endpoint, Action<object> callback, params string[] args)
        {
            Endpoint = string.Format(endpoint, args);
            this.callback = callback;
        }

        internal void Perform()
        {
            object result = null;
            Task.Run(delegate
            {
                bool failed = true;
                while (failed && ++uploadAttempts < MAX_ATTEMPTS)
                {
                    try
                    {
                        result = InternalPerform();
                        failed = false;
                    }
                    catch
                    {
                        Thread.Sleep(5000);
                    }
                }
            }).ContinueWith(delegate { callback(result); });
        }

        protected virtual object InternalPerform()
        {
            WebResponse resp = HttpWebRequest.CreateHttp(Endpoint).GetResponse();
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                return sr.ReadToEnd();
        }
    }
}
