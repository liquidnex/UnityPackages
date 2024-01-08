using Liquid.CommonUtils;
using Liquid.CommonUtils.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Liquid.AssetBundleToolkit.Editor
{
    public class AssetBundleBuilder
    {
        public enum FolderMarkMode
        {
            NONE,
            CLEAR_AB_TAG,
            NO_RECURSION,
            RECURSION,
            RECURSION_AND_UNIFIED_MARK
        }

        public static bool BuildAssetBundle(
            BuildTarget target,
            out string assetBundleBuildPath,
            out List<AssetBundleCatalog> allAssetBundleCatalogs)
        {
            allAssetBundleCatalogs = null;

            string assetBundleBuildLocation = GetAssetBundleBuildLocation(target);
            assetBundleBuildPath = AssetBundleCatalog.TransformLocationToPath(assetBundleBuildLocation);
            if (string.IsNullOrEmpty(assetBundleBuildPath))
                return false;

            bool b = CleanObsoleteDirectory(assetBundleBuildPath);
            if (!b)
                return false;

            BuildPipeline.BuildAssetBundles(
                assetBundleBuildPath,
                BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle,
                target);

            allAssetBundleCatalogs = GenerateAssetBundleCatalogs(target);
            return true;
        }

        public static bool SetFolderAsAssetBundle(string path, FolderMarkMode mode = FolderMarkMode.NONE, string topLevelABName = null)
        {
            if (!Directory.Exists(path))
                return false;

            path = FolderUtil.GetAssetPath(path);
            string abName = Path.GetFileName(path);
            if (topLevelABName == null)
                topLevelABName = abName;

            DirectoryInfo dic = new DirectoryInfo(path);
            FileInfo[] files = dic.GetFiles();
            foreach (FileInfo f in files)
            {
                if (!f.Name.StartsWith(".") &&
                    !f.Name.EndsWith("~") &&
                    !f.Name.EndsWith(".meta"))
                {
                    string filePath = FolderUtil.GetAssetPath(f.FullName);
                    AssetImporter importer = AssetImporter.GetAtPath(filePath);
                    if (importer != null)
                    {
                        if (mode == FolderMarkMode.CLEAR_AB_TAG)
                            importer.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
                        else if (mode == FolderMarkMode.RECURSION_AND_UNIFIED_MARK)
                            importer.SetAssetBundleNameAndVariant(topLevelABName, variantName);
                        else
                            importer.SetAssetBundleNameAndVariant(abName, variantName);
                    }
                }
            }

            if (mode == FolderMarkMode.CLEAR_AB_TAG ||
                mode == FolderMarkMode.RECURSION ||
                mode == FolderMarkMode.RECURSION_AND_UNIFIED_MARK)
            {
                DirectoryInfo[] dics = dic.GetDirectories();
                foreach (DirectoryInfo d in dics)
                {
                    if (!d.Name.StartsWith(".") &&
                        !d.Name.EndsWith("~") &&
                        !d.Name.EndsWith(".meta"))
                    {
                        SetFolderAsAssetBundle(d.FullName, mode, topLevelABName);
                    }
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return true;
        }

        private static bool CleanObsoleteDirectory(string assetBundleBuildPath)
        {
            if (string.IsNullOrEmpty(assetBundleBuildPath))
                return true;

            try
            {
                if (Directory.Exists(assetBundleBuildPath))
                {
                    Directory.Delete(assetBundleBuildPath, true);
                }
                Directory.CreateDirectory(assetBundleBuildPath);
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to delete or create file path {0}. Exception: {1}", assetBundleBuildPath, e.Message);
                Debug.LogError(message);
                return false;
            }
            return true;
        }

        private static List<AssetBundleCatalog> GenerateAssetBundleCatalogs(BuildTarget target)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            string assetBundleLocationStr = GetAssetBundleBuildLocation(target) + "/%ASSETBUNDLE_NAME%";
            List<AssetBundleCatalog> result = new List<AssetBundleCatalog>();
            string[] abNamesWithVariant = AssetDatabase.GetAllAssetBundleNames();
            foreach (string abNameWithVariant in abNamesWithVariant)
            {
                string assetBundleName = abNameWithVariant;
                string assetBundleLocation = assetBundleLocationStr.Replace("%ASSETBUNDLE_NAME%", abNameWithVariant);
                List<string> assetBundleDependentAssetBundleNames =
                    new List<string>(
                        AssetDatabase.GetAssetBundleDependencies(abNameWithVariant, true)
                    );

                AssetBundleCatalog catalog = new AssetBundleCatalog(assetBundleName, assetBundleLocation, assetBundleDependentAssetBundleNames);
                if (catalog.IsLegal)
                    result.Add(catalog);
            }
            return result;
        }

        private static string GetAssetBundleBuildLocation(BuildTarget target)
        {
            string platformFolderName = string.Empty;
            if (target == BuildTarget.StandaloneWindows)
            {
                platformFolderName = "Win32";
            }
            else if (target == BuildTarget.StandaloneWindows64)
            {
                platformFolderName = "Win64";
            }
            else if (target == BuildTarget.StandaloneOSX)
            {
                platformFolderName = "Mac";
            }
            else if (target == BuildTarget.Android)
            {
                platformFolderName = "Android";
            }
            else if (target == BuildTarget.iOS)
            {
                platformFolderName = "iOS";
            }
            else if (target == BuildTarget.WebGL)
            {
                platformFolderName = "WebGL";
            }

            return Path.Combine("%STREAMING_ASSETS_PATH%/Data/AssetBundle", platformFolderName).ToUnityPath();
        }

        [MenuItem("Assets/Set Folder As AssetBundle/Clear")]
        private static void ClearAssetBundleTag()
        {
            SetFolderAsAssetBundle(FolderUtil.GetAssetPathForSelectFolder(), FolderMarkMode.CLEAR_AB_TAG);
        }

        [MenuItem("Assets/Set Folder As AssetBundle/No Recursion")]
        private static void SetFolderAsAssetBundle()
        {
            SetFolderAsAssetBundle(FolderUtil.GetAssetPathForSelectFolder(), FolderMarkMode.NO_RECURSION);
        }

        [MenuItem("Assets/Set Folder As AssetBundle/Recursion")]
        private static void SetFolderAsAssetBundleWithRecursion()
        {
            SetFolderAsAssetBundle(FolderUtil.GetAssetPathForSelectFolder(), FolderMarkMode.RECURSION);
        }

        [MenuItem("Assets/Set Folder As AssetBundle/Recursion and Unified Mark")]
        private static void SetFolderAsAssetBundleWithRecursionAndUnifiedMark()
        {
            SetFolderAsAssetBundle(FolderUtil.GetAssetPathForSelectFolder(), FolderMarkMode.RECURSION_AND_UNIFIED_MARK);
        }

        [MenuItem("Edit/Build AssetBundle/Win32")]
        private static void BuildAssetBundleForWin32()
        {
            BuildAssetBundle(
                BuildTarget.StandaloneWindows,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        [MenuItem("Edit/Build AssetBundle/Win64")]
        private static void BuildAssetBundleForWin64()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows64,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        [MenuItem("Edit/Build AssetBundle/Mac")]
        private static void BuildAssetBundleForMac()
        {
            BuildAssetBundle(
                BuildTarget.StandaloneOSX,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        [MenuItem("Edit/Build AssetBundle/Android")]
        private static void BuildAssetBundleForAndroid()
        {
            BuildAssetBundle(
                BuildTarget.Android,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        [MenuItem("Edit/Build AssetBundle/iOS")]
        private static void BuildAssetBundleForiOS()
        {
            BuildAssetBundle(BuildTarget.iOS,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        [MenuItem("Edit/Build AssetBundle/WebGL")]
        private static void BuildAssetBundleForWebGL()
        {
            BuildAssetBundle(
                BuildTarget.WebGL,
                out string assetBundleBuildPath,
                out List<AssetBundleCatalog> allAssetBundleCatalogs);
        }

        internal const string AssetBundleBuildConfigPath = "Assets/Res/Data/AssetBundle Toolkit/AssetBundle Build Config.asset";

        private const string variantName = "unitydata";
    }
}