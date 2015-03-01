using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using QewbeClient.Http;
using System.Threading;
using System.Net;
using QewbeClient.Helpers;

namespace QewbeClient.API
{
    internal class UploadQueue
    {
        private const int MAX_CONCURRENT_UPLOADS = 5;

        internal event UploadResult UploadSucceeded;
        internal event UploadResult UploadFailed;

        private List<File> fileQueue = new List<File>();
        private int processingCount;

        private string tempDir;

        internal UploadQueue()
        {
            tempDir = Path.GetTempPath();
        }

        internal void Add(FileInfo file, bool fromTemp = false)
        {
            if (fromTemp)
                file = file.CopyTo(tempDir);

            string checksum = calculateChecksum(file);

            HttpClient.SendRequest(new NetRequest(Endpoints.UPLOAD_FILE, delegate(object r)
            {
                UploadFileReply reply = Serializer.Deserialize<UploadFileReply>(r.ToString());
                reply.File.InternalName = file.FullName;
                if (!reply.OK)
                    return;
                lock (fileQueue)
                    fileQueue.Add(reply.File);
            }, User.Token, Path.GetExtension(file.FullName), checksum, @"object"));
        }

        internal void Remove(FileInfo file)
        {
            string checksum = calculateChecksum(file);
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
                    File currentFile = fileQueue[i];
                    fileQueue.RemoveAt(i);
                    currentFile.UploadSucceeded += uploadCompleted;
                    currentFile.UploadFailed += uploadFailed;
                    currentFile.BeginUpload();
                }
            }
        }

        private void uploadCompleted(File file)
        {
            Interlocked.Decrement(ref processingCount);
            if (UploadSucceeded != null)
                UploadSucceeded(file);
        }

        private void uploadFailed(File file)
        {
            Interlocked.Decrement(ref processingCount);
            if (UploadFailed != null)
                UploadFailed(file);
        }

        private string calculateChecksum(FileInfo file)
        {
            using (SHA256Managed sha = new SHA256Managed())
                return Encoding.ASCII.GetString(sha.ComputeHash(file.OpenRead()));
        }
    }
}
