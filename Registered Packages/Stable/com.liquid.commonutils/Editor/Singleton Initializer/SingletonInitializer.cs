using System.IO;
using UnityEditor;

namespace Liquid.CommonUtils.Editor
{
    public class SingletonInitializer
    {
        private static string packagePath;

        [InitializeOnLoadMethod]
        private static void SingletonInit()
        {
            bool needInitPackage = true;
            string projectPath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(projectPath))
            {
                projectPath = projectPath.ToUnityPath();
                string tt1 = projectPath + "/Assets/Tools/T4/Singleton.tt";
                string tt2 = projectPath + "/Assets/Tools/T4/SingletonMonoBehaviour.tt";

                if (File.Exists(tt1) &&
                    File.Exists(tt2))
                    needInitPackage = false;
            }

            if (needInitPackage)
            {
                AssetDatabase.ImportPackage(PackagePath + "/Package Resources/Singleton T4 Source Code.unitypackage", false);
            }
        }

        private static string PackagePath
        {
            get
            {
                if (!string.IsNullOrEmpty(packagePath))
                    return packagePath;

                packagePath = FolderUtil.GetPackageFullPath("com.liquid.commonutils");
                return packagePath;
            }
        }
    }
}