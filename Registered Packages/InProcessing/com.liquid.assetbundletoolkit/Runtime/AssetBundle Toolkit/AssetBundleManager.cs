using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid.AssetBundleToolkit
{
    /// <summary>
    /// Provide asset bundle management tools - including loading, expansion and unloading.
    /// </summary>
    public class AssetBundleManager : MonoBehaviour
    {
        /// <summary>
        /// Load synchronously through a asset bundle path.
        /// If the asset is loaded repeatedly, it will be provided directly from the resource pool.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <param name="preloadAssets">
        /// If it is set to true, all asset objects will be preloaded,
        /// otherwise no asset object will be preloaded.
        /// </param>
        /// <param name="ab">Loaded assetbundle.</param>
        /// <param name="assetNames">Asset names for loaded assetbundle.</param>
        /// <param name="assetObjects">Loaded asset objects dictionary.</param>
        ///  <returns>It will returns true when the load operation is successful, otherwise it will returns false.</returns>
        public bool Load(string assetBundleName, bool preloadAssets,
            out AssetBundle ab,
            out List<string> assetNames,
            out Dictionary<string, UnityEngine.Object> assetObjects)
        {
            bool result = false;
            ab = null;
            assetNames = new List<string>();
            assetObjects = new Dictionary<string, UnityEngine.Object>();
            if (assetBundleName == null)
                return false;

            List<string> needLoadAssetBundleNames = GetDependentAssetBundleNames(assetBundleName);
            needLoadAssetBundleNames.Add(assetBundleName);

            foreach (string name in needLoadAssetBundleNames)
            {
                AssetBundleHandler handler;
                if (assetBundleHandlers.ContainsKey(name))
                {
                    handler = assetBundleHandlers[name];
                }
                else
                {
                    string assetBundlePath = GetAssetBundlePath(name);
                    handler = new AssetBundleHandler(assetBundlePath);
                    assetBundleHandlers.Add(name, handler);
                }

                handler.Load(out AssetBundle hAB, out List<string> hAN);
                if (preloadAssets)
                    handler.Expand(out Dictionary<string, UnityEngine.Object> hAO);
            }

            AssetBundleHandler targetHandler = GetAssetBundleHandler(assetBundleName);
            if (targetHandler != null)
            {
                result = targetHandler.IsLoadSuccessed;
                if (preloadAssets)
                    result = result && targetHandler.IsExpandSuccessed;

                ab = targetHandler.AssetBundleContent;
                assetNames = targetHandler.AssetNames;
                if (preloadAssets)
                    assetObjects = targetHandler.AssetObjects;
            }
            return result;
        }

        /// <summary>
        /// Load asynchronously through a asset bundle path.
        /// If the asset is loaded repeatedly, it will be provided directly from the resource pool.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <param name="preloadAssets">
        /// If it is set to true, all asset objects will be preloaded,
        /// otherwise no asset object will be preloaded.
        /// </param>
        /// <param name="loadCallback">Callback for asynchronous load completion.</param>
        public void LoadAsync(string assetBundleName, bool preloadAssets = true,
            Action<bool,
                AssetBundle,
                List<string>,
                Dictionary<string, UnityEngine.Object>> loadCallback = null)
        {
            if (assetBundleName == null)
            {
                loadCallback?.Invoke(false, null, new List<string>(), new Dictionary<string, UnityEngine.Object>());
                return;
            }

            List<string> needLoadAssetBundleNames = GetDependentAssetBundleNames(assetBundleName);
            needLoadAssetBundleNames.Add(assetBundleName);

            int loadedCount = 0;
            foreach (string name in needLoadAssetBundleNames)
            {
                AssetBundleHandler handler;
                if (assetBundleHandlers.ContainsKey(name))
                {
                    handler = assetBundleHandlers[name];
                }
                else
                {
                    string assetBundlePath = GetAssetBundlePath(name);
                    handler = new AssetBundleHandler(assetBundlePath);
                    assetBundleHandlers.Add(name, handler);
                }

                handler.LoadAsync(
                    (loadSuccess, ab, assetNames) =>
                    {
                        ++loadedCount;
                        if (loadedCount == needLoadAssetBundleNames.Count)
                        {
                            if (preloadAssets)
                            {
                                handler.ExpandAsync((expandSuccess, assetObjects) => {
                                    AssetBundleHandler targetHandler = GetAssetBundleHandler(assetBundleName);
                                    if (targetHandler != null)
                                    {
                                        loadCallback?.Invoke(
                                            targetHandler.IsLoadSuccessed && targetHandler.IsExpandSuccessed,
                                            targetHandler.AssetBundleContent,
                                            targetHandler.AssetNames,
                                            targetHandler.AssetObjects);
                                    }
                                    else
                                    {
                                        loadCallback?.Invoke(false, null, new List<string>(), new Dictionary<string, UnityEngine.Object>());
                                    }
                                });
                            }
                            else
                            {
                                AssetBundleHandler targetHandler = GetAssetBundleHandler(assetBundleName);
                                if (targetHandler != null)
                                {
                                    loadCallback?.Invoke(
                                        targetHandler.IsLoadSuccessed,
                                        targetHandler.AssetBundleContent,
                                        targetHandler.AssetNames,
                                        new Dictionary<string, UnityEngine.Object>());
                                }
                                else
                                {
                                    loadCallback?.Invoke(false, null, new List<string>(), new Dictionary<string, UnityEngine.Object>());
                                }
                            }
                        }
                    }
                );
            }
        }

        /// <summary>
        /// Unload synchronously through a asset bundle path.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        public void Unload(
            string assetBundleName,
            bool unloadAllLoadedObjects = true)
        {
            if (assetBundleName == null)
                return;

            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);

            foreach (AssetBundleHandler handler in handlers)
            {
                handler.Unload(unloadAllLoadedObjects);
            }

            foreach (AssetBundleHandler handler in handlers)
            {
                string needRemovedKey = null;
                foreach (var kv in assetBundleHandlers)
                {
                    if (kv.Value != null &&
                        kv.Value == handler)
                    {
                        needRemovedKey = kv.Key;
                        break;
                    }
                }

                if (needRemovedKey != null)
                    assetBundleHandlers.Remove(needRemovedKey);
            }
        }

        /// <summary>
        /// Unload synchronously through an exist asset bundle loader.
        /// </summary>
        /// <param name="handler">Asset bundle handler for unload.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        public void Unload(
            AssetBundleHandler handler,
            bool unloadAllLoadedObjects = true)
        {
            if (handler == null)
                return;

            if (!assetBundleHandlers.ContainsValue(handler))
                return;

            string key = null;
            foreach (var kv in assetBundleHandlers)
            {
                if (kv.Value == handler)
                {
                    key = kv.Key;
                    break;
                }
            }

            if (key == null)
                return;
            Unload(key, unloadAllLoadedObjects);
        }

        /// <summary>
        /// Unload synchronously through a loaded asset bundle.
        /// </summary>
        /// <param name="ab">A loaded asset bundle.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        public void Unload(
            AssetBundle ab,
            bool unloadAllLoadedObjects = false)
        {
            if (ab == null)
                return;

            string key = null;
            foreach (var kv in assetBundleHandlers)
            {
                if (kv.Value != null &&
                    kv.Value.AssetBundleContent == ab)
                {
                    key = kv.Key;
                    break;
                }
            }

            if (key == null)
                return;
            Unload(key, unloadAllLoadedObjects);
        }

        /// <summary>
        /// Unload asynchronously through a asset bundle path.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        /// <param name="callback">Callback for asynchronous unload completion.</param>
        public void UnloadAsync(
            string assetBundleName,
            bool unloadAllLoadedObjects = false,
            Action callback = null)
        {
            if (assetBundleName == null)
            {
                callback?.Invoke();
                return;
            }

            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler targetHandler = GetAssetBundleHandler(assetBundleName);
            if (targetHandler != null)
                handlers.Add(targetHandler);

            int unloadedCount = 0;
            foreach (AssetBundleHandler handler in handlers)
            {
                handler.UnloadAsync(
                    unloadAllLoadedObjects,
                    () => {
                        ++unloadedCount;
                        if (unloadedCount == handlers.Count)
                            callback?.Invoke();
                    }
                );
            }
        }

        /// <summary>
        /// Unload asynchronously through an exist asset bundle loader.
        /// </summary>
        /// <param name="handler">Asset bundle handler for unload.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        /// <param name="callback">Callback for asynchronous unload completion.</param>
        public void UnloadAsync(
            AssetBundleHandler handler,
            bool unloadAllLoadedObjects = false,
            Action callback = null)
        {
            if (handler == null)
            {
                callback?.Invoke();
                return;
            }

            if (!assetBundleHandlers.ContainsValue(handler))
            {
                callback?.Invoke();
                return;
            }

            string key = null;
            foreach (var kv in assetBundleHandlers)
            {
                if (kv.Value == handler)
                {
                    key = kv.Key;
                    break;
                }
            }

            if (key == null)
                callback?.Invoke();
            UnloadAsync(key, unloadAllLoadedObjects, callback);
        }

        /// <summary>
        /// Unload asynchronously through a loaded asset bundle.
        /// </summary>
        /// <param name="ab">A loaded asset bundle.</param>
        /// <param name="unloadAllLoadedObjects">
        /// If it is set to true, all loaded assets of the target will be unloaded,
        /// otherwise only the assetbundle will be unloaded.
        /// </param>
        /// <param name="callback">Callback for asynchronous unload completion.</param>
        public void UnloadAsync(
            AssetBundle ab,
            bool unloadAllLoadedObjects = false,
            Action callback = null)
        {
            if (ab == null)
                return;

            string key = null;
            foreach (var kv in assetBundleHandlers)
            {
                if (kv.Value.AssetBundleContent == ab)
                {
                    key = kv.Key;
                    break;
                }
            }

            if (key == null)
                callback?.Invoke();
            UnloadAsync(key, unloadAllLoadedObjects, callback);
        }

        /// <summary>
        /// Get a asset bundle loader through a asset bundle path.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>Asset bundle loader found.</returns>
        public AssetBundleHandler GetAssetBundleHandler(string assetBundleName)
        {
            if (assetBundleHandlers.TryGetValue(assetBundleName, out AssetBundleHandler loader))
                return loader;

            return null;
        }

        /// <summary>
        /// Get a loaded asset bundle through a asset bundle path.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>Asset bundle found.</returns>
        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            if (IsLoadSuccessed(assetBundleName))
            {
                if (assetBundleHandlers.TryGetValue(assetBundleName, out AssetBundleHandler loader))
                    return loader.AssetBundleContent;
            }

            return null;
        }

        /// <summary>
        /// Get the loading progress of the loader with an asset bundle path.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>Loading progress.</returns>
        public float GetLoadProgress(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return 0f;

            float allProgress = 0f;
            foreach (AssetBundleHandler handler in handlers)
            {
                allProgress += handler.LoadProgress;
            }

            float average = allProgress / handlers.Count;
            return average;
        }

        /// <summary>
        /// Get the expanding progress of the loader with an asset bundle path.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>Loading progress.</returns>
        public float GetExpandProgress(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return 0f;

            float allProgress = 0f;
            foreach (AssetBundleHandler loader in handlers)
            {
                allProgress += loader.ExpandProgress;
            }

            float average = allProgress / handlers.Count;
            return average;
        }

        /// <summary>
        /// Get the unloading progress of the loader with an asset bundle path.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>Unloading progress.</returns>
        public float GetUnloadProgress(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            int cnt = handlers.Count;
            if (cnt == 0)
                return 1f;

            float allProgress = 0f;
            bool hasUnloadHandler = false;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (handler.IsExpired)
                {
                    hasUnloadHandler = true;
                    allProgress += handler.UnloadProgress;
                }
            }

            if (hasUnloadHandler)
            {
                float average = allProgress / cnt;
                return average;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// Get the loading progress of all loaders.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <returns>Loading progress.</returns>
        public float GetLoadProgress()
        {
            float allProgress = 0f;
            int cnt = assetBundleHandlers.Values.Count;
            if (cnt == 0)
                return 0f;

            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                allProgress += handler.LoadProgress;
            }

            float average = allProgress / cnt;
            return average;
        }

        /// <summary>
        /// Get the expanding progress of all loaders.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <returns>Loading progress.</returns>
        public float GetExpandProgress()
        {
            float allProgress = 0f;
            int cnt = assetBundleHandlers.Values.Count;
            if (cnt == 0)
                return 0f;

            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                allProgress += handler.ExpandProgress;
            }

            float average = allProgress / cnt;
            return average;
        }

        /// <summary>
        /// Get the unloading progress of all loaders.
        /// Normal values range from 0 to 1.
        /// </summary>
        /// <returns>Unloading progress.</returns>
        public float GetUnloadProgress()
        {
            int cnt = assetBundleHandlers.Values.Count;
            if (cnt == 0)
                return 1f;

            float allProgress = 0f;
            bool hasUnloadHandler = false;
            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                if (handler.IsExpired)
                {
                    hasUnloadHandler = true;
                    allProgress += handler.UnloadProgress;
                }
            }

            if (hasUnloadHandler)
            {
                float average = allProgress / cnt;
                return average;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// Query whether the specified asset bundle has been loaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified loading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsLoadFinished(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return false;

            bool allLoadFinished = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsLoadFinished)
                {
                    allLoadFinished = false;
                    break;
                }
            }

            return allLoadFinished;
        }

        /// <summary>
        /// Query whether the specified asset objects has been loaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified loading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsExpandFinished(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return false;

            bool allExpandFinished = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsExpandFinished)
                {
                    allExpandFinished = false;
                    break;
                }
            }

            return allExpandFinished;
        }

        /// <summary>
        /// Query whether the specified asset bundle has been unloaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified unloading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsUnloadFinished(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return true;

            bool allUnloadFinished = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsUnloadFinished)
                {
                    allUnloadFinished = false;
                    break;
                }
            }

            return allUnloadFinished;
        }

        /// <summary>
        /// Query whether the specified asset bundle has been successful loaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified loading is complete successful,
        /// otherwise returns false.
        /// </returns>
        public bool IsLoadSuccessed(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return false;

            bool allLoadSuccessed = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsLoadSuccessed)
                {
                    allLoadSuccessed = false;
                    break;
                }
            }

            return allLoadSuccessed;
        }

        /// <summary>
        /// Query whether the specified asset objects has been successful loaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified loading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsExpandSuccessed(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return false;

            bool allExpandSuccessed = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsExpandSuccessed)
                {
                    allExpandSuccessed = false;
                    break;
                }
            }

            return allExpandSuccessed;
        }

        /// <summary>
        /// Query whether the specified asset bundle has been unloaded.
        /// </summary>
        /// <param name="assetBundleName">Name of asset bundle.</param>
        /// <returns>
        /// Returns true if the specified unloading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsUnloadSuccessed(string assetBundleName)
        {
            List<AssetBundleHandler> handlers = GetDependentHandlers(assetBundleName);
            AssetBundleHandler mainHandler = GetAssetBundleHandler(assetBundleName);
            if (mainHandler != null)
                handlers.Add(mainHandler);
            if (handlers.Count == 0)
                return true;

            bool allUnloadSuccessed = true;
            foreach (AssetBundleHandler handler in handlers)
            {
                if (!handler.IsUnloadSuccessed)
                {
                    allUnloadSuccessed = false;
                    break;
                }
            }

            return allUnloadSuccessed;
        }

        /// <summary>
        /// Query whether all asset bundles have been loaded.
        /// </summary>
        /// <returns>
        /// Returns true if all loading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsLoadFinished()
        {
            bool allLoadFinished = true;
            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                if (!handler.IsLoadFinished)
                {
                    allLoadFinished = false;
                    break;
                }
            }

            return allLoadFinished;
        }

        /// <summary>
        /// Query whether all asset objects have been loaded.
        /// </summary>
        /// <returns>
        /// Returns true if all loading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsExpandFinished()
        {
            bool allExpandFinished = true;
            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                if (!handler.IsExpandFinished)
                {
                    allExpandFinished = false;
                    break;
                }
            }

            return allExpandFinished;
        }

        /// <summary>
        /// Query whether all asset bundles have been unloaded.
        /// </summary>
        /// <returns>
        /// Returns true if all unloading is complete,
        /// otherwise returns false.
        /// </returns>
        public bool IsUnloadFinished()
        {
            bool allUnloadFinished = true;
            foreach (AssetBundleHandler handler in assetBundleHandlers.Values)
            {
                if (handler.IsUnloadFinished)
                {
                    allUnloadFinished = false;
                    break;
                }
            }

            return allUnloadFinished;
        }

        public void RegisterCatalog(AssetBundleCatalog abc)
        {
            if (!abc.IsLegal)
                return;

            string assetBundleName = abc.Name;
            if (assetBundleName == string.Empty)
                return;

            if (assetBundleCatalogs.ContainsKey(assetBundleName))
                assetBundleCatalogs[assetBundleName] = abc;
            else
                assetBundleCatalogs.Add(assetBundleName, abc);
        }

        public void RegisterCatalogs(List<AssetBundleCatalog> abcs)
        {
            if (abcs == null)
                return;

            foreach (AssetBundleCatalog abc in abcs)
            {
                RegisterCatalog(abc);
            }
        }

        public void ClearCatalog(string assetBundleName)
        {
            if (assetBundleName == null)
                return;

            if (assetBundleCatalogs.ContainsKey(assetBundleName))
                assetBundleCatalogs.Remove(assetBundleName);
        }

        public void ClearCatalogs(List<string> assetBundleNames)
        {
            if (assetBundleNames == null)
                return;

            foreach (string abName in assetBundleNames)
            {
                ClearCatalog(abName);
            }
        }

        public void ClearCatalogs()
        {
            if (assetBundleCatalogs == null)
                assetBundleCatalogs = new Dictionary<string, AssetBundleCatalog>();
            else
                assetBundleCatalogs.Clear();
        }

        private List<AssetBundleHandler> GetDependentHandlers(string assetBundleName)
        {
            List<AssetBundleHandler> result = new List<AssetBundleHandler>();
            if (assetBundleName == null)
                return result;

            List<string> names = GetDependentAssetBundleNames(assetBundleName);
            foreach (var handlerKV in assetBundleHandlers)
            {
                string abName = handlerKV.Key;
                if (names.Contains(abName))
                {
                    AssetBundleHandler handler = handlerKV.Value;
                    if (handler != null &&
                        !result.Contains(handler))
                        result.Add(handler);
                }
            }

            return result;
        }

        private List<string> GetDependentAssetBundleNames(string assetBundleName)
        {
            if (assetBundleCatalogs.ContainsKey(assetBundleName))
                return assetBundleCatalogs[assetBundleName].DependentAssetBundleNames;
            else
                return new List<string>();
        }

        private string GetAssetBundlePath(string assetBundleName)
        {
            if (assetBundleCatalogs.ContainsKey(assetBundleName))
                return assetBundleCatalogs[assetBundleName].Path;
            else
                return string.Empty;
        }

        private void Awake()
        {
            if (instance != null &&
                instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Update()
        {
            foreach (AssetBundleHandler loader in assetBundleHandlers.Values)
            {
                loader.OnUpdate();
            }
        }

        private void OnDestroy()
        {
            foreach (var loader in assetBundleHandlers.Values)
            {
                loader.Clear(true);
            }

            assetBundleHandlers.Clear();
            assetBundleCatalogs.Clear();
        }

        private AssetBundleManager() {}

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static AssetBundleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("_AssetBundleManager");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<AssetBundleManager>();
                }
                return instance;
            }
        }

        private Dictionary<string, AssetBundleHandler> assetBundleHandlers = new Dictionary<string, AssetBundleHandler>();
        private Dictionary<string, AssetBundleCatalog> assetBundleCatalogs = new Dictionary<string, AssetBundleCatalog>();

        private static AssetBundleManager instance;
    }
}