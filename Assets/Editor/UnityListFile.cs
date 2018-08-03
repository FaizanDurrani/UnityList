using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Editor
{
    public class UnityListFile
    {
        public bool Completed { get; private set; }
        public Exception Error { get; private set; }

        public float Progress
        {
            get { return _req.downloadProgress; }
        }

        public string Path { get; private set; }
        public bool Cancelled { get; private set; }

        public string FullPath
        {
            get { return Path + "/" + FileName; }
        }

        public string FileName { get; private set; }
        public string Url { get; private set; }
        
        public int FileSize
        {
            get { return _handler.TotalLength / 1024; }
        }
        
        public int Downloaded
        {
            get { return _handler.AlreadyWritten / 1024; }
        }

        private readonly UnityWebRequest _req;
        private readonly UnityListFileDownloadHandler _handler;

        public UnityListFile(string fileName, string url, string path)
        {
            Path = path;
            Url = url;
            FileName = fileName;
            
            _handler = new UnityListFileDownloadHandler(FullPath);
            _req = new UnityWebRequest(url, "GET", _handler, null);
            var asyncReq = _req.SendWebRequest();
            asyncReq.completed += OnFileDownloaded;
        }

        private void OnFileDownloaded(AsyncOperation obj)
        {
            Completed = true;
            Error = new Exception(_req.error);
        }

        public void Cancel()
        {
            Cancelled = true;
            _req.Abort();
            _handler.Abort();
        }
    }
}