using QewbeClient.API.Reply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QewbeClient.Http
{
    internal class FileNetRequest : NetRequest
    {
        private UploadFile file;

        internal event UploadFileCompletedEventHandler Completed;
        internal event UploadProgressChangedEventHandler ProgressChanged;

        internal FileNetRequest(UploadFile file, Action<object> callback = null)
            : base(string.Format(@"{0}/{1}", file.Domain, file.Name), callback, string.Empty)
        {
            this.file = file;
        }

        internal override void Perform()
        {
            ++UploadAttempts;

            WebClient wc = new WebClient();
            wc.UploadFileCompleted += Completed;
            wc.UploadProgressChanged += ProgressChanged;

            System.IO.FileInfo info = new System.IO.FileInfo(file.InternalName);
            byte[] data = new byte[info.Length];
            info.OpenRead().ReadAsync(data, 0, data.Length).ContinueWith(delegate(Task<int> res)
            {
                try
                {
                    wc.UploadData(Endpoint, data);
                    Callback(true);
                }
                catch
                {
                    if (UploadAttempts == MAX_ATTEMPTS)
                    {
                        Callback(false);
                        return;
                    }

                    Thread.Sleep(5000);
                    Perform();
                }
            });
        }
    }
}
