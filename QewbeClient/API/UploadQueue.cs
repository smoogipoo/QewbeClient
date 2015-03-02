using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using QewbeClient.Http;
using System.Threading;
using System.Net;
using QewbeClient.Helpers;
using QewbeClient.API.Reply;
using System.IO;

namespace QewbeClient.API
{
    internal class UploadQueue
    {
        private const int MAX_CONCURRENT_UPLOADS = 5;

        internal event UploadResult UploadSucceeded;
        internal event UploadResult UploadFailed;

        private List<UploadFile> fileQueue = new List<UploadFile>();
        private int processingCount;

        private string tempDir;

        internal UploadQueue()
        {
            tempDir = Path.GetTempPath();
        }

        internal void Add(string file, bool fromTemp = false)
        {
            FileInfo info = new FileInfo(file);

            if (fromTemp)
                info = info.CopyTo(tempDir);

            string checksum = Crypto.CalculateChecksum(info);

            HttpClient.SendRequest(new NetRequest(Endpoints.UPLOAD_FILE, delegate(object r)
            {
                UploadFileReply reply = Serializer.Deserialize<UploadFileReply>(r.ToString());
                reply.File.InternalName = info.FullName;
                if (!reply.OK)
                    return;
                lock (fileQueue)
                    fileQueue.Add(reply.File);
            }, User.Token, Path.GetExtension(info.FullName), checksum, @"object"));
        }

        internal void Remove(System.IO.FileInfo file)
        {
            string checksum = Crypto.CalculateChecksum(file);
            lock (fileQueue)
                fileQueue.RemoveAll(f => f.Checksum == checksum);
        }

        internal void Update()
        {
            if (fileQueue.Count == 0 || processingCount == MAX_CONCURRENT_UPLOADS)
                return;

            lock (fileQueue)
            {
                for (int i = 0; i < fileQueue.Count || processingCount == MAX_CONCURRENT_UPLOADS; i++)
                {
                    Interlocked.Increment(ref processingCount);
                    UploadFile currentFile = fileQueue[i];
                    fileQueue.RemoveAt(i);
                    currentFile.UploadSucceeded += uploadCompleted;
                    currentFile.UploadFailed += uploadFailed;
                    currentFile.BeginUpload();
                }
            }
        }

        private void uploadCompleted(UploadFile file)
        {
            Interlocked.Decrement(ref processingCount);
            if (UploadSucceeded != null)
                UploadSucceeded(file);
        }

        private void uploadFailed(UploadFile file)
        {
            Interlocked.Decrement(ref processingCount);
            if (UploadFailed != null)
                UploadFailed(file);
        }
    }
}
