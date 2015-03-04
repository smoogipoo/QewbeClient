using QewbeClient.API.Reply;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.Helpers
{
    internal delegate void UploadSucceededResult(UploadFile file);
    internal delegate void UploadFailedResult(FileInfo file);
}
