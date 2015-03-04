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
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Win32;

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

        internal void Add(Bitmap bitmap)
        {
            string name = Path.Combine(tempDir, Guid.NewGuid().ToString().Replace(@"-", string.Empty) + @".png");
            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == @"image/png");
            EncoderParameters encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bitmap.Save(name, codec, encParams);
            Add(name);
        }

        internal void Add(string file)
        {
            FileInfo info = new FileInfo(file);

            string checksum = Crypto.CalculateChecksum(info);

            HttpClient.SendRequest(new NetRequest(Endpoints.UPLOAD_FILE, delegate(object r)
            {
                UploadFileReply reply = Serializer.Deserialize<UploadFileReply>(r.ToString());
                reply.File.InternalName = info.FullName;
                if (!reply.OK)
                    return;
                lock (fileQueue)
                    fileQueue.Add(reply.File);
            }, User.Token, Path.GetExtension(info.FullName), checksum, getMimeType(info.Extension)));
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

        private static string getMimeType(string ext)
        {
            string mimeType = "application/unknown";
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(ext.ToLower());
            if (key != null && key.GetValue("Content Type") != null)
                mimeType = key.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
