using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Liquid.CommonUtils.Editor
{
    /// <summary>
    /// Provides tools for common file operations.
    /// </summary>
    public static class FolderUtil
    {
        /// <summary>
        /// Recursively copy all files and folders from fromPath to destPath.
        /// </summary>
        /// <param name="fromPath">Source folder.</param>
        /// <param name="destPath">Destination folder.</param>
        public static void CopyDirectory(string fromPath, string destPath)
        {
            if (!Directory.Exists(fromPath))
                return;

            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            DirectoryInfo i = new DirectoryInfo(fromPath);
            foreach (FileInfo f in i.GetFiles())
            {
                string targetFilePath = Path.Combine(destPath, f.Name);
                if (!File.Exists(targetFilePath))
                    f.CopyTo(targetFilePath);
            }

            foreach (DirectoryInfo d in i.GetDirectories())
            {
                string targetDicPath = Path.Combine(destPath, d.Name);
                if (!Directory.Exists(targetDicPath))
                    Directory.CreateDirectory(targetDicPath);

                CopyDirectory(d.FullName, targetDicPath);
            }
        }

        /// <summary>
        /// Gets the absolute path where the specified unity package is located.
        /// </summary>
        /// <param name="packageName">The specified packge name.</param>
        /// <returns>Absolute found path.</returns>
        public static string GetPackageFullPath(string packageName)
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/" + packageName);
            if (Directory.Exists(packagePath))
            {
                return packagePath.ToUnityPath();
            }

            string projectPath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(projectPath))
            {
                // Search default location for development package
                string defaultLocation = projectPath + "/Assets/Packages/" + packageName;
                if (Directory.Exists(defaultLocation))
                {
                    return defaultLocation.ToUnityPath();
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(projectPath, packageName, SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, projectPath);
                if (path != null)
                    return (projectPath + path).ToUnityPath();
            }

            return null;
        }

        /// <summary>
        /// Get asset file's relative path from the assets folder.
        /// </summary>
        /// <param name="fullPath">Absolute asset file path.</param>
        /// <returns>Asset path relative to the assets folder.</returns>
        public static string GetAssetPath(string fullPath)
        {
            Regex rgx = new Regex(@"Assets[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = string.Empty;
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = assetPath.ToUnityPath();
            return assetPath;
        }

        public static string GetResourcePath(string fullPath)
        {
            if (fullPath == null)
                return null;

            string assetPath = string.Empty;
            Regex rgx = new Regex(@"(Resources[\\/].+)$");
            Match matches = rgx.Match(fullPath);

            if (matches.Success)
            {
                string path = matches.Value;
                Regex rgxSuffix = new Regex(@"\.[^\\/\.]+$");
                string result = rgxSuffix.Replace(path, string.Empty);
                assetPath = result;
            }

            assetPath = assetPath.ToUnityPath();
            return assetPath;
        }

        /// <summary>
        /// Get asset path for selected folder in project window.
        /// </summary>
        /// <returns>Folder's asset path relative to the asset folder.</returns>
        public static string GetAssetPathForSelectFolder()
        {
            Object[] arr = Selection.GetFiltered<Object>(SelectionMode.TopLevel);
            if (arr.Length > 0)
            {
                return AssetDatabase.GetAssetPath(arr[0]);
            }
            return string.Empty;
        }

        private static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (!Directory.Exists(paths[i]))
                    continue;

                // Check if any of the matching directories contain a package.json file.
                if (File.Exists(paths[i] + "/package.json"))
                {
                    string folderPath = paths[i].Replace(projectPath, string.Empty);
                    folderPath = folderPath.TrimStart('\\', '/');
                    return folderPath;
                }
            }

            return null;
        }

        [MenuItem("Edit/Open Persistent Data Path")]
        private static void Open()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}