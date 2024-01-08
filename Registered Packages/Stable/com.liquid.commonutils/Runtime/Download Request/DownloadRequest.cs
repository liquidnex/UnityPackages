using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// A resource file downloader.
    /// Use UWR to provide the download function of network content.
    /// </summary>
    public class DownloadRequest
    {
        public enum RequestResult
        {
            NONE,
            FAIL,
            ABORT,
            SUCCESS
        }

        private enum DownloadTypeEnum
        {
            NONE,
            FILE,
            ASSET_BUNDLE
        }

        private RequestResult result = RequestResult.NONE;
        private byte[] downloadedBytes;
        private string downloadedText;
        private AssetBundle downloadAssetBundle;

        private DownloadTypeEnum type;
        private UnityWebRequest uwr;
        private event OnFileDownloadFinished fileDownloadCallback;
        private event OnAssetBundleDownloadFinished assetBundleDownloadCallback;

        /// <summary>
        /// Get the downloaded assetbundle.
        /// </summary>
        public AssetBundle DownloadedAssetBundle => downloadAssetBundle;

        /// <summary>
        /// Get the downloaded bytes.
        /// </summary>
        public byte[] DownloadedBytes => downloadedBytes;

        /// <summary>
        /// Get the downloaded text.
        /// </summary>
        public string DownloadedText => downloadedText;

        /// <summary>
        /// Returns result of the request.
        /// </summary>
        public RequestResult Result => result;

        public bool IsFinished
        {
            get => result != RequestResult.NONE;
        }

        /// <summary>
        /// Requested download progress.
        /// </summary>
        public float Progress
        {
            get
            {
                if (result == RequestResult.ABORT)
                    return 0f;
                else if (result == RequestResult.NONE)
                {
                    if (uwr != null)
                        return uwr.downloadProgress;
                    return 0f;
                }
                return 1f;
            }
        }

        /// <summary>
        /// A delegate called when the download of a common network resource file is completed.
        /// </summary>
        /// <param name="result">Result of the download behavior.</param>
        /// <param name="bytes">Downloaded file bytes.</param>
        /// <param name="text">Downloaded file text.</param>
        public delegate void OnFileDownloadFinished(RequestResult result, byte[] bytes, string text);

        /// <summary>
        /// A delegate called when the download of a common type of network Assetbundle file is completed.
        /// </summary>
        /// <param name="result">Result of the download behavior.</param>
        /// <param name="ab">Downloaded assetbundle object.</param>
        public delegate void OnAssetBundleDownloadFinished(RequestResult result, AssetBundle ab);

        /// <summary>
        /// Create a file download request.
        /// </summary>
        /// <param name="uri">Requested URI.</param>
        /// <param name="respondCallback">Download complete callback.</param>
        public DownloadRequest(string uri, OnFileDownloadFinished respondCallback = null)
        {
            type = DownloadTypeEnum.FILE;
            fileDownloadCallback += respondCallback;
            SendFileDownloadRequest(uri);
        }

        /// <summary>
        /// Create a AssetBundle download request
        /// </summary>
        /// <param name="uri">Requested URI.</param>
        /// <param name="version">
        /// An integer version number, which will be compared to the cached version of the asset bundle to download.
        /// Increment this number to force Unity to redownload a cached asset bundle.
        /// If zero, the version assignment is ignored.
        /// </param>
        /// <param name="crc">
        /// If nonzero, this number will be compared to the checksum of the downloaded asset bundle data.
        /// If the CRCs do not match, an error will be logged and the asset bundle will not be loaded.
        /// If set to zero, CRC checking will be skipped.
        /// </param>
        /// <param name="respondCallback">Download complete callback.</param>
        public DownloadRequest(
            string uri,
            uint version,
            uint crc,
            OnAssetBundleDownloadFinished respondCallback = null)
        {
            type = DownloadTypeEnum.ASSET_BUNDLE;
            assetBundleDownloadCallback += respondCallback;
            SendAssetBundleDownloadRequest(uri, version, crc);
        }

        internal DownloadRequest(
            RequestResult r,
            byte[] bytes,
            string text)
        {
            type = DownloadTypeEnum.FILE;
            result = r;
            if (result == RequestResult.SUCCESS)
            {
                downloadedBytes = bytes;
                downloadedText = text;
            }
        }

        internal DownloadRequest(
            RequestResult r,
            AssetBundle ab,
            byte[] bytes)
        {
            type = DownloadTypeEnum.ASSET_BUNDLE;
            result = r;
            if (result == RequestResult.SUCCESS)
            {
                downloadAssetBundle = ab;
                downloadedBytes = bytes;
            }
        }

        /// <summary>
        /// Abort the ongoing download.
        /// </summary>
        public void Abort()
        {
            OnDownloadAbort();
        }

        private void SendFileDownloadRequest(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                OnDownloadFail();
                return;
            }

            try
            {
                if (uri.IsLegalURI())
                {
                    uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET, new DownloadHandlerBuffer(), null);
                    var rq = uwr.SendWebRequest();
                    rq.completed += OnRequestCompleted;
                }
                else
                    OnDownloadFail();
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to send download request for uri: {0}. Exception: {1}", uri, e.Message);
                Debug.LogWarning(message);
                OnDownloadFail();
            }
        }

        private void SendAssetBundleDownloadRequest(string uri, uint version, uint crc)
        {
            if (string.IsNullOrEmpty(uri))
            {
                OnDownloadFail();
                return;
            }

            try
            {
                if (uri.IsLegalHTTPURI())
                {
                    if (version == 0 && crc == 0)
                        uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    else if(version == 0 && crc != 0)
                        uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri, crc);
                    else
                        uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri, version, crc);

                    var rq = uwr.SendWebRequest();
                    rq.completed += OnRequestCompleted;
                }
                else
                    OnDownloadFail();
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to asset bundle download request for uri: {0}. Exception: {1}", uri, e.Message);
                Debug.LogWarning(message);
                OnDownloadFail();
            }
        }

        private void OnRequestCompleted(AsyncOperation op)
        {
            if (type == DownloadTypeEnum.FILE)
            {
                OnFileDownloadSuccess(
                    uwr.downloadHandler.data,
                    uwr.downloadHandler.text);
            }
            else if (type == DownloadTypeEnum.ASSET_BUNDLE)
            {
                OnAssetBundleDownloadSuccess(
                    DownloadHandlerAssetBundle.GetContent(uwr));
            }
        }

        private void OnDownloadAbort()
        {
            result = RequestResult.ABORT;
            ClearRequest();
            if (type == DownloadTypeEnum.FILE)
            {
                if (fileDownloadCallback != null)
                    fileDownloadCallback(result, null, null);
            }
            else if (type == DownloadTypeEnum.ASSET_BUNDLE)
            {
                if (assetBundleDownloadCallback != null)
                    assetBundleDownloadCallback(result, null);
            }
        }

        private void OnDownloadFail()
        {
            result = RequestResult.FAIL;
            ClearRequest();
            if (type == DownloadTypeEnum.FILE)
            {
                if (fileDownloadCallback != null)
                    fileDownloadCallback(result, null, null);
            }
            else if (type == DownloadTypeEnum.ASSET_BUNDLE)
            {
                if (assetBundleDownloadCallback != null)
                    assetBundleDownloadCallback(result, null);
            }
        }

        private void OnFileDownloadSuccess(byte[] bytes, string text)
        {
            result = RequestResult.SUCCESS;
            downloadedBytes = bytes;
            downloadedText = text;
            ClearRequest();
            if (fileDownloadCallback != null)
                fileDownloadCallback(result, downloadedBytes, downloadedText);
        }

        private void OnAssetBundleDownloadSuccess(AssetBundle ab)
        {
            result = RequestResult.SUCCESS;
            downloadAssetBundle = ab;
            ClearRequest();
            if (assetBundleDownloadCallback != null)
                assetBundleDownloadCallback(result, ab);
        }

        private void ClearRequest()
        {
            uwr.Abort();
            uwr.Dispose();
            uwr = null;
        }
    }
}