using Liquid.CommonUtils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Liquid.AssetBundleToolkit
{
    [Serializable]
    public struct AssetBundleCatalog
    {
        public AssetBundleCatalog(string name, string location, List<string> dependentABNames)
        {
            Name = name;
            Location = location;
            DependentAssetBundleNames = dependentABNames;
        }

        public static string TransformLocationToPath(string location)
        {
            if (location == null)
                return null;

            string str = location.ToUnityPath();
            Regex rgxSA = new Regex(@"^%STREAMING_ASSETS_PATH%[\\/]");
            str = rgxSA.Replace(str, Application.streamingAssetsPath + "/");
            Regex rgxPD = new Regex(@"^%PERSISTENT_DATA_PATH%[\\/]");
            str = rgxPD.Replace(str, Application.persistentDataPath + "/");
            return str;
        }

        public bool IsLegal
        {
            get
            {
                if (string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(Location) ||
                    DependentAssetBundleNames == null)
                    return false;
                return true;
            }
        }

        public string Path
        {
            get => TransformLocationToPath(Location);
        }

        public string Name;
        public string Location;
        public List<string> DependentAssetBundleNames;
    }
}