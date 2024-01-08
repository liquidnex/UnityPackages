using Liquid.CommonUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid.AssetBundleToolkit
{
    /// <summary>
    /// An asset bundle handler.
    /// Manage the loading, expanding, and unloading behavior of assets.
    /// </summary>
    public class AssetBundleHandler
    {
        /// <summary>
        /// Callback for asynchronous asset bundle load completion.
        /// </summary>
        /// <param name="success">The success value will be assigned to true when the load operation is successful, otherwise it will be assigned to false.</param>
        /// <param name="ab">Loaded assetbundle.</param>
        /// <param name="assetNames">Asset names of the loaded assetbundle.</param>
        public delegate void OnAssetBundleLoadFinished(bool success, AssetBundle ab, List<string> assetNames);

        /// <summary>
        /// Callback for asynchronous expand completion.
        /// </summary>
        /// <param name="success">The success value will be assigned to true when the expand operation is successful, otherwise it will be assigned to false.</param>
        /// <param name="assetObjects">Expanded asset objects.</param>
        public delegate void OnAssetBundleExpandFinished(bool success, Dictionary<string, Object> assetObjects);

        /// <summary>
        /// Callback for asynchronous unload completion.
        /// </summary>
        public delegate void OnAssetBundleUnloadFinished();

        /// <summary>
        /// Callback for asynchronous load completion.
        /// </summary>
        /// <typeparam name="T">Target object type of asset object.</typeparam>
        /// <param name="obj">Loaded asset object.</param>
        public delegate void OnAssetObjectLoadFinished<T>(T obj)
            where T : Object;

        /// <summary>
        /// State enumeration of the handler.
        /// </summary>
        public enum OperationState
        {
            NONE,
            WORKING,
            FINISHED
        }

        private enum OperationType
        {
            NONE,
            LOCAL_SYNC,
            LOCAL_ASYNC,
            REMOTE_ASYNC
        }

        /// <summary>
        /// Initialize handler.
        /// The initialization behavior of your derived types can be implemented by overriding.
        /// </summary>
        /// <param name="assetBundlePath">Remote or local path of asset bundle.</param>
        public AssetBundleHandler(string assetBundlePath)
        {
            if (assetBundlePath == null)
                this.assetBundlePath = string.Empty;
            else
                this.assetBundlePath = assetBundlePath;
        }

        /// <summary>
        /// Load asset synchronously.
        /// </summary>
        /// <param name="ab">Loaded assetbundle.</param>
        /// <param name="assetNames">Asset names of the loaded assetbundle.</param>
        /// <returns>Loaded asset bundle.</returns>
        public virtual bool Load(out AssetBundle ab, out List<string> assetNames)
        {
            InitDataBeforeLoad();

            if (IsLoadSuccessed)
            {
                assetNames = new List<string>{};
                ab = assetBundleContent;
                return true;
            }

            if (loadState == OperationState.WORKING)
            {
                if (loadType == OperationType.REMOTE_ASYNC)
                {
                    if (assetBundleDownloadRequest != null)
                    {
                        while (!assetBundleDownloadRequest.IsFinished) ;
                        ab = assetBundleDownloadRequest.DownloadedAssetBundle;
                        if (ab == null)
                        {
                            assetNames = new List<string>();
                            return false;
                        }
                        else
                        {
                            assetNames = new List<string>(ab.GetAllAssetNames());
                            return true;
                        }
                    }
                }
                else if (loadType == OperationType.LOCAL_ASYNC)
                {
                    if (assetBundleLoadRequest != null)
                    {
                        while (!assetBundleLoadRequest.isDone) ;
                        ab = assetBundleLoadRequest.assetBundle;
                        if (ab == null)
                        {
                            assetNames = new List<string>();
                            return false;
                        }
                        else
                        {
                            assetNames = new List<string>(ab.GetAllAssetNames());
                            return true;
                        }
                    }
                }
            }

            loadState = OperationState.WORKING;
            loadType = OperationType.LOCAL_SYNC;
            if (assetBundlePath.IsLegalHTTPURI())
            {
                DownloadRequest d = new DownloadRequest(assetBundlePath, 0, 0);
                while (d.IsFinished) ;
                assetBundleContent = d.DownloadedAssetBundle;
            }
            else
            {
                assetBundleContent = AssetBundle.LoadFromFile(assetBundlePath);
            }
            ab = assetBundleContent;
            this.assetNames = assetBundleContent.GetAllAssetNames();
            assetNames = AssetNames;

            loadState = OperationState.FINISHED;
            return IsLoadSuccessed;
        }

        /// <summary>
        /// Load asset asynchronously.
        /// </summary>
        /// <param name="callback">Callback for asynchronous load completion.</param>
        public virtual void LoadAsync(OnAssetBundleLoadFinished callback = null)
        {
            InitDataBeforeLoad();

            if (IsLoadSuccessed)
            {
                callback?.Invoke(true, assetBundleContent, AssetNames);
                return;
            }

            OnLoadFinished += callback;
            if (loadState == OperationState.WORKING)
                return;

            loadState = OperationState.WORKING;
            if (assetBundlePath.IsLegalHTTPURI())
            {
                loadType = OperationType.REMOTE_ASYNC;
                assetBundleDownloadRequest = new DownloadRequest(assetBundlePath, 0, 0);
                if (assetBundleDownloadRequest == null)
                {
                    Debug.LogError($"Remote AssetBundle {assetBundlePath} can't be found, please check it");
                    loadState = OperationState.FINISHED;
                }
            }
            else
            {
                loadType = OperationType.LOCAL_ASYNC;
                assetBundleLoadRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                if (assetBundleLoadRequest == null)
                {
                    Debug.LogError($"Local AssetBundle {assetBundlePath} can't be found, please check it");
                    loadState = OperationState.FINISHED;
                }
            }
        }

        /// <summary>
        /// Expand asset synchronously.
        /// For the streaming scene asset bundle type that cannot be expanded in Unity,
        /// this extension function will ignore it and directly mark it as expanded.
        /// </summary>
        /// <param name="assetObjects">Expanded assetbundle.</param>
        /// <returns>Returns true when the expand operation is successful, otherwise returns false.</returns>
        public virtual bool Expand(out Dictionary<string, Object> assetObjects)
        {
            InitDataBeforeExpand();

            if (assetBundleContent == null)
            {
                Debug.LogError("The expand operation needs to be performed after the assetbundle is loaded.");
                assetObjects = new Dictionary<string, Object>();
                expandState = OperationState.FINISHED;
                return false;
            }

            if (assetBundleContent.isStreamedSceneAssetBundle)
            {
                // Streamed scene asset bundle is no need to expand,
                // but it must be accessed through the UnityEngine.SceneManagement.SceneManager.
                expandState = OperationState.FINISHED;
                assetObjects = new Dictionary<string, Object>();
                return true;
            }

            if (IsExpandSuccessed)
            {
                assetObjects = AssetObjects;
                return true;
            }

            if (expandState == OperationState.WORKING)
            {
                while (expandedCount < assetNames.Length) ;
                assetObjects = AssetObjects;
                if (assetObjects.Count < assetNames.Length)
                    return false;
                else
                    return true;
            }

            expandState = OperationState.WORKING;
            expandedCount = 0;
            foreach (string name in assetNames)
            {
                bool hasAsset = this.assetObjects.TryGetValue(name, out Object asset);
                if (!hasAsset || asset == null)
                    LoadAssetObject(name);
            }
            expandedCount = assetNames.Length;
            assetObjects = AssetObjects;

            expandState = OperationState.FINISHED;
            return IsExpandSuccessed;
        }

        /// <summary>
        /// Expand asset asynchronously.
        /// For the streaming scene asset bundle type that cannot be expand in Unity,
        /// this expansion function will ignore it and directly mark it as expanded.
        /// </summary>
        public virtual void ExpandAsync(OnAssetBundleExpandFinished callback = null)
        {
            InitDataBeforeExpand();

            if (assetBundleContent == null)
            {
                Debug.LogError("The expand operation needs to be performed after the assetbundle is loaded.");
                expandState = OperationState.NONE;
                callback?.Invoke(false, new Dictionary<string, Object>());
                return;
            }

            if (assetBundleContent.isStreamedSceneAssetBundle)
            {
                // Streamed scene asset bundle is no need to expand,
                // but it must be accessed through the UnityEngine.SceneManagement.SceneManager.
                expandState = OperationState.FINISHED;
                callback?.Invoke(true, new Dictionary<string, Object>());
                return;
            }

            if (IsExpandSuccessed)
            {
                callback?.Invoke(true, AssetObjects);
                return;
            }

            OnExpandFinished += callback;
            if (expandState == OperationState.WORKING)
                return;

            expandState = OperationState.WORKING;
            expandedCount = 0;
            List<string> needExpandAssetObjectNames = new List<string>();
            foreach (string name in assetNames)
            {
                bool hasAsset = assetObjects.TryGetValue(name, out Object asset);
                if (!hasAsset || asset == null)
                    needExpandAssetObjectNames.Add(name);
                else
                    ++expandedCount;
            }
            foreach (string name in needExpandAssetObjectNames)
            {
                LoadAssetObjectAsync(name, OnOneExpandFinished);
            }
        }

        /// <summary>
        /// Unload asset synchronously.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all assets that the target depends on will also be unloaded,
        /// otherwise only the target asset will be unloaded.
        /// </param>
        public virtual void Unload(bool unloadAllLoadedObjects = true)
        {
            InitDataBeforeUnload();

            if (IsUnloadSuccessed)
                return;

            if (unloadState == OperationState.WORKING)
            {
                if (unloadType == OperationType.LOCAL_ASYNC)
                {
                    if (assetBundleUnloadRequest != null)
                    {
                        while (!assetBundleUnloadRequest.isDone) ;
                        return;
                    }
                }
            }

            unloadState = OperationState.WORKING;
            unloadType = OperationType.LOCAL_SYNC;
            assetBundleContent.Unload(unloadAllLoadedObjects);
            assetBundleContent = null;
            assetBundleUnloadRequest = null;
            onUnloadAssetBundleFinishedImpl = null;
            unloadState = OperationState.FINISHED;
        }

        /// <summary>
        /// Unload asset asynchronously.
        /// </summary>
        /// <param name="unloadAllRelated">
        /// If it is set to true, all assets that the target depends on will also be unloaded,
        /// otherwise only the target asset will be unloaded.
        /// </param>
        /// <param name="callback">Callback for asynchronous unload completion.</param>
        public virtual void UnloadAsync(bool unloadAllRelated = true, OnAssetBundleUnloadFinished callback = null)
        {
            InitDataBeforeUnload();

            if (IsUnloadSuccessed)
                callback?.Invoke();

            OnUnloadFinished += callback;
            if (unloadState == OperationState.WORKING)
                return;

            unloadState = OperationState.WORKING;
            unloadType = OperationType.LOCAL_ASYNC;
            assetBundleUnloadRequest = assetBundleContent.UnloadAsync(unloadAllRelated);
        }

        /// <summary>
        /// Try to get asset object.
        /// </summary>
        /// <param name="name">Name of asset.</param>
        /// <param name="obj">Asset object.</param>
        /// <returns>Whether the asset is gotten successful.</returns>
        public bool TryGetAsset(string name, out Object obj)
        {
            obj = null;
            if (name == null)
                return true;
            name = name.ToLower();

            if (assetObjects.ContainsKey(name))
            {
                obj = assetObjects[name];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to get asset object.
        /// </summary>
        /// <param name="name">Name of asset.</param>
        /// <param name="obj">Asset object.</param>
        /// <returns>Whether the asset is gotten successful.</returns>
        public bool TryGetAsset<T>(string name, out T obj)
            where T : Object
        {
            obj = null;

            if (name == null)
                return true;
            name = name.ToLower();

            if (!assetObjects.ContainsKey(name))
                return false;

            Object o = assetObjects[name];
            if (o == null)
                return false;

            if (o is T tObj)
            {
                obj = tObj;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Load an asset object by an asset object name.
        /// </summary>
        /// <typeparam name="T">Target object type of asset object.</typeparam>
        /// <param name="name">Name of asset object.</param>
        /// <returns>Asset object found.</returns>
        public T LoadAssetObject<T>(string name)
            where T : Object
        {
            if (name == null)
                return null;
            name = name.ToLower();

            if (assetBundleContent == null)
                return null;

            // Streamed scene asset bundle is no need to expand,
            // but it must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            T o = assetBundleContent.LoadAsset<T>(name);
            SetAssetObject(name, o);
            return o;
        }

        /// <summary>
        /// Load an asset object by an asset object name.
        /// </summary>
        /// <param name="name">Name of asset object.</param>
        /// <param name="type">Target object type of asset object.</param>
        /// <returns>Asset object found.</returns>
        public Object LoadAssetObject(string name, System.Type type)
        {
            if (name == null)
                return null;
            name = name.ToLower();

            if (assetBundleContent == null)
                return null;

            // Streamed scene asset bundle is no need to expand,
            // but it must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            Object o = assetBundleContent.LoadAsset(name, type);
            SetAssetObject(name, o);
            if (type.IsAssignableFrom(o.GetType()))
                return o;
            else
                return null;
        }

        /// <summary>
        /// Load an asset object by an asset object name.
        /// </summary>
        /// <param name="name">Name of asset object.</param>
        /// <returns>Asset object found.</returns>
        public Object LoadAssetObject(string name)
        {
            if (name == null)
                return null;
            name = name.ToLower();

            if (assetBundleContent == null)
                return null;

            // Streamed scene asset bundle is no need to expand,
            // It must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent.isStreamedSceneAssetBundle)
                return null;

            Object o = assetBundleContent.LoadAsset(name);
            SetAssetObject(name, o);
            return o;
        }

        /// <summary>
        /// Load asynchronously through an asset bundle path.
        /// </summary>
        /// <typeparam name="T">Target object type of asset object.</typeparam>
        /// <param name="name">Name of asset object.</param>
        /// <param name="callback">Callback for asynchronous load completion.</param>
        public void LoadAssetObjectAsync<T>(
            string name,
            OnAssetObjectLoadFinished<T> callback = null)
            where T : Object
        {
            if (name == null)
            {
                callback?.Invoke(null);
                return;
            }
            name = name.ToLower();

            // Streamed scene asset bundle is no need to expand,
            // It must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent == null ||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                callback?.Invoke(null);
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync<T>(name);
            rq.completed +=
                ao =>
                {
                    Object o = rq.asset;
                    SetAssetObject(name, o);
                    T t = o as T;
                    callback?.Invoke(t);
                };
        }

        /// <summary>
        /// Load asynchronously through an asset bundle path.
        /// </summary>
        /// <param name="name">Name of asset object.</param>
        /// <param name="type">Target object type of asset object.</param>
        /// <param name="callback">Callback for asynchronous load completion.</param>
        public void LoadAssetObjectAsync(
            string name,
            System.Type type,
            OnAssetObjectLoadFinished<Object> callback = null)
        {
            if (name == null)
            {
                callback?.Invoke(null);
                return;
            }
            name = name.ToLower();

            // Streamed scene asset bundle is no need to expand,
            // It must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent == null ||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                callback?.Invoke(null);
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync(name, type);
            rq.completed +=
                ao => {
                    Object o = rq.asset;
                    SetAssetObject(name, o);

                    if (type.IsAssignableFrom(o.GetType()))
                        callback?.Invoke(o);
                    else
                        callback?.Invoke(null);
                };
        }

        /// <summary>
        /// Load asynchronously through an asset bundle path.
        /// </summary>
        /// <param name="name">Name of asset object.</param>
        /// <param name="callback">Callback for asynchronous load completion.</param>
        public void LoadAssetObjectAsync(
            string name,
            OnAssetObjectLoadFinished<Object> callback = null)
        {
            if (name == null)
            {
                callback?.Invoke(null);
                return;
            }
            name = name.ToLower();

            // Streamed scene asset bundle is no need to expand,
            // but it must be accessed through the UnityEngine.SceneManagement.SceneManager.
            if (assetBundleContent == null ||
                assetBundleContent.isStreamedSceneAssetBundle)
            {
                callback?.Invoke(null);
                return;
            }

            AssetBundleRequest rq = assetBundleContent.LoadAssetAsync(name);
            rq.completed +=
                ao => {
                    Object o = rq.asset;
                    SetAssetObject(name, o);
                    callback?.Invoke(o);
                };
        }

        /// <summary>
        /// The update behavior of your derived types can be implemented by overriding.
        /// </summary>
        public virtual void OnUpdate()
        {
            if (loadState == OperationState.WORKING)
            {
                switch (loadType)
                {
                    case OperationType.LOCAL_ASYNC:
                        if (assetBundleLoadRequest != null)
                        {
                            if (assetBundleLoadRequest.isDone)
                            {
                                if (assetBundleLoadRequest.assetBundle == null)
                                {
                                    Debug.LogWarning($"Local AssetBundle {assetBundlePath} can't be loaded.");
                                    loadState = OperationState.FINISHED;
                                }
                                else
                                {
                                    assetBundleContent = assetBundleLoadRequest.assetBundle;
                                    assetNames = assetBundleContent.GetAllAssetNames();
                                    loadState = OperationState.FINISHED;
                                }
                            }
                        }
                        break;
                    case OperationType.REMOTE_ASYNC:
                        if (assetBundleDownloadRequest != null)
                        {
                            if (assetBundleDownloadRequest.IsFinished)
                            {
                                if (assetBundleDownloadRequest.DownloadedAssetBundle == null)
                                {
                                    Debug.LogWarning($"Remote asset bundle {assetBundlePath} can't be loaded, try reload");
                                    loadState = OperationState.FINISHED;
                                }
                                else
                                {
                                    assetBundleContent = assetBundleDownloadRequest.DownloadedAssetBundle;
                                    assetNames = assetBundleContent.GetAllAssetNames();
                                    loadState = OperationState.FINISHED;
                                }
                            }
                        }
                        break;
                }

                if (loadState == OperationState.FINISHED &&
                    onLoadAssetBundleFinishedImpl != null)
                {
                    onLoadAssetBundleFinishedImpl(IsLoadSuccessed, assetBundleContent, AssetNames);
                    onLoadAssetBundleFinishedImpl = null;
                }
            }

            if (unloadState == OperationState.WORKING)
            {
                switch (unloadType)
                {
                    case OperationType.LOCAL_ASYNC:
                        if (assetBundleUnloadRequest != null)
                        {
                            if (assetBundleUnloadRequest.isDone)
                                unloadState = OperationState.FINISHED;
                        }
                        break;
                }

                if (unloadState == OperationState.FINISHED &&
                    onUnloadAssetBundleFinishedImpl != null)
                {
                    onUnloadAssetBundleFinishedImpl();
                    assetBundleContent = null;
                    assetBundleUnloadRequest = null;
                    onUnloadAssetBundleFinishedImpl = null;
                }
            }
        }

        /// <summary>
        /// Clear handler content.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all assets that the target depends on will also be unloaded,
        /// otherwise only the target asset will be unloaded.
        /// </param>
        internal void Clear(bool unloadAllLoadedObjects = false)
        {
            Unload(unloadAllLoadedObjects);
            assetBundlePath = string.Empty;
        }

        /// <summary>
        /// Clear handler content async.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all assets that the target depends on will also be unloaded,
        /// otherwise only the target asset will be unloaded.
        /// </param>
        internal void ClearAsync(bool unloadAllLoadedObjects = false, System.Action callback = null)
        {
            UnloadAsync(unloadAllLoadedObjects, () => {
                assetBundlePath = string.Empty;
                callback?.Invoke();
            });
        }

        private void InitDataBeforeLoad()
        {
            assetObjects.Clear();

            unloadType = OperationType.NONE;

            expandState = OperationState.NONE;
            unloadState = OperationState.NONE;

            expandedCount = 0;

            assetBundleUnloadRequest = null;

            onExpandAssetBundleFinishedImpl = null;
            onUnloadAssetBundleFinishedImpl = null;
        }

        private void InitDataBeforeExpand()
        {
            List<string> needRemoveKeys = new List<string>();
            foreach (var kv in assetObjects)
            {
                string key = kv.Key;
                if (key == null)
                    needRemoveKeys.Add(key);
                else if (kv.Value == null)
                    needRemoveKeys.Add(key);
            }

            foreach (string key in needRemoveKeys)
            {
                assetObjects.Remove(key);
            }
        }

        private void InitDataBeforeUnload()
        {
            assetNames = new string[] {};
            assetObjects.Clear();

            loadType = OperationType.NONE;

            loadState = OperationState.NONE;
            expandState = OperationState.NONE;

            expandedCount = 0;

            assetBundleLoadRequest = null;
            assetBundleDownloadRequest = null;

            onLoadAssetBundleFinishedImpl = null;
            onExpandAssetBundleFinishedImpl = null;
        }

        private void OnOneExpandFinished(Object o)
        {
            ++expandedCount;
            if (expandedCount < assetNames.Length)
                return;

            expandState = OperationState.FINISHED;
            if (onExpandAssetBundleFinishedImpl != null)
            {
                onExpandAssetBundleFinishedImpl(IsExpandSuccessed, AssetObjects);
                onExpandAssetBundleFinishedImpl = null;
            }
        }

        private void SetAssetObject(string name, Object obj)
        {
            if (name == null ||
                obj == null)
                return;

            if (assetObjects.ContainsKey(name))
                assetObjects[name] = obj;
            else
                assetObjects.Add(name, obj);
        }

        /// <summary>
        /// Asset bundle path of this handler.
        /// </summary>
        public string AssetBundlePath
        {
            get => assetBundlePath;
        }

        /// <summary>
        /// Loaded asset bundle content.
        /// </summary>
        public AssetBundle AssetBundleContent
        {
            get => assetBundleContent;
        }

        public List<string> AssetNames
        {
            get
            {
                if (assetNames == null)
                    return null;

                return new List<string>(assetNames);
            }
        }

        public Dictionary<string, Object> AssetObjects
        {
            get
            {
                if (assetObjects == null)
                    return null;

                Dictionary<string, Object> copy = new Dictionary<string, Object>();
                foreach (var kv in assetObjects)
                    copy.Add(kv.Key, kv.Value);
                return copy;
            }
        }

        public bool IsLoadSuccessed
        {
            get
            {
                return IsLoadFinished && assetBundleContent != null;
            }
        }

        public bool IsExpandSuccessed
        {
            get
            {
                return IsExpandFinished && assetObjects.Count >= assetNames.Length;
            }
        }

        public bool IsUnloadSuccessed
        {
            get
            {
                return IsUnloadFinished && assetBundleContent == null;
            }
        }

        /// <summary>
        /// Determine whether the loading of asset bundle is completed.
        /// </summary>
        public bool IsLoadFinished
        {
            get => loadState == OperationState.FINISHED;
        }

        /// <summary>
        /// Determine whether the loading of asset objects is completed.
        /// </summary>
        public bool IsExpandFinished
        {
            get => expandState == OperationState.FINISHED;
        }

        /// <summary>
        /// Determine whether the unloading of asset bundle is completed.
        /// </summary>
        public bool IsUnloadFinished
        {
            get => unloadState == OperationState.FINISHED;
        }

        /// <summary>
        /// Load progress.
        /// Values range from 0 to 1.
        /// </summary>
        public float LoadProgress
        {
            get
            {
                switch (loadType)
                {
                    case OperationType.LOCAL_SYNC:
                        if (loadState == OperationState.FINISHED)
                            return 1f;
                        break;
                    case OperationType.LOCAL_ASYNC:
                        if (assetBundleLoadRequest != null)
                            return assetBundleLoadRequest.progress;
                        break;
                    case OperationType.REMOTE_ASYNC:
                        if (assetBundleDownloadRequest != null)
                            return assetBundleDownloadRequest.Progress;
                        break;
                }
                return 0f;
            }
        }

        /// <summary>
        /// Expand progress.
        /// Values range from 0 to 1.
        /// </summary>
        public float ExpandProgress
        {
            get
            {
                if (assetBundleContent != null &&
                    assetBundleContent.isStreamedSceneAssetBundle)
                    return 1f;

                if (assetNames == null ||
                    assetNames.Length == 0)
                    return 1f;

                return expandedCount / assetNames.Length;
            }
        }

        /// <summary>
        /// Unload progress.
        /// Values range from 0 to 1.
        /// </summary>
        public float UnloadProgress
        {
            get
            {
                switch (unloadType)
                {
                    case OperationType.LOCAL_SYNC:
                        if (unloadState == OperationState.FINISHED)
                            return 1f;
                        break;
                    case OperationType.LOCAL_ASYNC:
                        if (assetBundleUnloadRequest != null)
                            return assetBundleUnloadRequest.progress;
                        break;
                }
                return 0f;
            }
        }

        /// <summary>
        /// Determine whether the asset has call to unload.
        /// </summary>
        internal bool IsExpired
        {
            get => unloadState != OperationState.NONE;
        }

        private event OnAssetBundleLoadFinished OnLoadFinished
        {
            add
            {
                if (value == null)
                    return;

                if (IsLoadFinished)
                    value(IsLoadSuccessed, assetBundleContent, AssetNames);
                else
                    onLoadAssetBundleFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onLoadAssetBundleFinishedImpl -= value;
            }
        }

        private event OnAssetBundleExpandFinished OnExpandFinished
        {
            add
            {
                if (value == null)
                    return;

                if (IsExpandFinished)
                    value(IsExpandSuccessed, AssetObjects);
                else
                    onExpandAssetBundleFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onExpandAssetBundleFinishedImpl -= value;
            }
        }

        private event OnAssetBundleUnloadFinished OnUnloadFinished
        {
            add
            {
                if (value == null)
                    return;

                if (IsUnloadFinished)
                    value();
                else
                    onUnloadAssetBundleFinishedImpl += value;
            }
            remove
            {
                if (value != null)
                    onUnloadAssetBundleFinishedImpl -= value;
            }
        }

        private string assetBundlePath = string.Empty;
        private AssetBundle assetBundleContent;
        private string[] assetNames = new string[] {};
        private Dictionary<string, Object> assetObjects = new Dictionary<string, Object>();

        private OperationType loadType;
        private OperationType unloadType;

        private OperationState loadState = OperationState.NONE;
        private OperationState expandState = OperationState.NONE;
        private OperationState unloadState = OperationState.NONE;

        private int expandedCount = 0;

        private AssetBundleCreateRequest assetBundleLoadRequest;
        private DownloadRequest assetBundleDownloadRequest;
        private AsyncOperation assetBundleUnloadRequest;

        private event OnAssetBundleLoadFinished onLoadAssetBundleFinishedImpl;
        private event OnAssetBundleExpandFinished onExpandAssetBundleFinishedImpl;
        private event OnAssetBundleUnloadFinished onUnloadAssetBundleFinishedImpl;
    }
}