using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QewbeClient.Http
{
    internal static class HttpClient
    {
        private static Queue<NetRequest> requests = new Queue<NetRequest>();

        internal static void Update()
        {
#if !DEBUG
            try
            {
#endif
                lock (requests)
                {
                    while (requests.Count > 0)
                    {
                        NetRequest req = requests.Dequeue();
                        Task.Run(delegate { req.Perform(); });
                    }
                }
#if !DEBUG
            }
            catch { }
#endif
        }

        internal static void SendRequest(NetRequest request)
        {
            lock (requests)
                requests.Enqueue(request);
        }
    }
}
