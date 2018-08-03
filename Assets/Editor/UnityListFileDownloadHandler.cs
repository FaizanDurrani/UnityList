using System.IO;
using UnityEngine.Networking;
using UnityEngine;

namespace Editor
{
    public class UnityListFileDownloadHandler : DownloadHandlerScript
    {
        private readonly FileStream _fileStream;
        public int AlreadyWritten { get; private set; }
        public int TotalLength  { get; private set; }

        public UnityListFileDownloadHandler(string path) : base(new byte[16*1024])
        {
            _fileStream = File.Create(path);
        }

        protected override float GetProgress()
        {
            return (float) AlreadyWritten / TotalLength;
        }

        protected override void ReceiveContentLength(int contentLength)
        {
            TotalLength = contentLength;
            Debug.Log(contentLength);
        }

        protected override void CompleteContent()
        {
            Abort();
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            _fileStream.Write(data, 0, dataLength);
            AlreadyWritten += dataLength;
            return true;
        }

        public void Abort()
        {
            _fileStream.Close();
            _fileStream.Dispose();
        }
    }
}