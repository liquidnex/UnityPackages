using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// IO operating system tools for basic files.
    /// </summary>
    public class FileUtil
    {
        private static FileUtil instance;
        private static readonly object syslock = new object();

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static FileUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new FileUtil();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// A delegate called when the download of a common network resource file is completed.
        /// </summary>
        /// <param name="success">Result of the download behavior.</param>
        /// <param name="bytes">Downloaded file bytes.</param>
        /// <param name="text">Downloaded file text.</param>
        public delegate void OnLoadFinished(bool success, byte[] bytes, string text);

        /// <summary>
        /// Get local file text in synchronization.
        /// </summary>
        /// <param name="filePath">Relative path of the file.</param>
        /// <returns>Read file contents(in the form of string).</returns>
        public string GetFileText(string filePath)
        {
            GetFile(filePath, out byte[] bytes, out string fileText);
            return fileText;
        }

        /// <summary>
        /// Get local file bytes in synchronization.
        /// </summary>
        /// <param name="filePath">Relative path of the file.</param>
        /// <returns>Read file contents(in the form of string).</returns>
        public byte[] GetFileBytes(string filePath)
        {
            GetFile(filePath, out byte[] fileBytes, out string fileText);
            return fileBytes;
        }

        /// <summary>
        /// Get file content in synchronization.
        /// </summary>
        /// <param name="location">Path or URI of the file.</param>
        /// <param name="fileBytes">Read file bytes.</param>
        /// <param name="fileText">Read file contents(in the form of string).</param>
        public void GetFile(
            string location,
            out byte[] fileBytes,
            out string fileText)
        {
            fileBytes = null;
            fileText = null;
            if (string.IsNullOrEmpty(location))
                return;

            try
            {
                if (location.IsLegalURI())
                {
                    DownloadRequest downloader = new DownloadRequest(location);
                    while (downloader.IsFinished) ;
                    fileBytes = downloader.DownloadedBytes;
                    fileText = downloader.DownloadedText;
                }
                else if (File.Exists(location))
                {
                    fileBytes = File.ReadAllBytes(location);
                    fileText = File.ReadAllText(location);
                }
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to get content of file: {0}. Exception: {1}", location, e.Message);
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Get remote file bytes asynchronously.
        /// </summary>
        /// <param name="location">Path or URI of the file.</param>
        /// <param name="respondCallback">Callback for downloading results.</param>
        /// <returns>Handle of download.</returns>
        public DownloadRequest GetFileAsync(
            string location,
            OnLoadFinished respondCallback = null)
        {
            if (string.IsNullOrEmpty(location))
            {
                if (respondCallback == null)
                    respondCallback(false, null, null);
                return null;
            }

            if (location.IsLegalURI())
            {
                DownloadRequest downloader = new DownloadRequest(
                    location, (result, bytes, text) => {
                        if (respondCallback != null)
                            respondCallback(result == DownloadRequest.RequestResult.SUCCESS, bytes, text);
                    }
                );
                return downloader;
            }
            else
            {
                GetFile(location, out byte[] bytes, out string fileText);

                if (bytes != null &&
                    fileText != null)
                {
                    if (respondCallback != null)
                        respondCallback(true, bytes, fileText);
                    return new DownloadRequest(DownloadRequest.RequestResult.SUCCESS, bytes, fileText);
                }
                else
                {
                    if (respondCallback != null)
                        respondCallback(false, bytes, fileText);
                    return null;
                }
            }
        }

        public string GetMD5(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return string.Empty;

                StringBuilder sb = new StringBuilder();
                using (MD5 md5 = MD5.Create())
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] md5Bytes = md5.ComputeHash(fs);
                    fs.Close();

                    foreach (byte b in md5Bytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }

        private FileUtil() {}
    }
}