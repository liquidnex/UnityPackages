using Liquid.CommonUtils;
using Liquid.CommonUtils.Editor;
using System.IO;
using UnityEditor;

namespace Liquid.BriskUI.Editor
{
    public class BriskUIInitializer
    {
        private static string packagePath;

        private static string PackagePath
        {
            get
            {
                if (!string.IsNullOrEmpty(packagePath))
                    return packagePath;

                packagePath = FolderUtil.GetPackageFullPath("com.liquid.briskui");
                return packagePath;
            }
        }

        [InitializeOnLoadMethod]
        private static void BriskUIInit()
        {
            bool needInitPackage = true;
            string projectPath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(projectPath))
            {
                projectPath = projectPath.ToUnityPath();
                string catalogPath = projectPath + "/Assets/Resources/Data/UI/UI Catalog.asset";
                string uiRootPath = projectPath + "/Assets/Resources/Prefab/UI/Root UI.prefab";

                if (File.Exists(catalogPath) &&
                    File.Exists(uiRootPath))
                    needInitPackage = false;
            }

            if (needInitPackage)
            {
                AssetDatabase.ImportPackage(PackagePath + "/Package Resources/Brisk UI Resources.unitypackage", false);
            }
        }

        [MenuItem("Window/Text Manager/Import Text Manager Examples")]
        private static void ImportExamplesContentMenu()
        {
            AssetDatabase.ImportPackage(PackagePath + "/Package Resources/Text Manager Sample.unitypackage", true);
        }

        [MenuItem("Window/Brisk UI/Import Brisk UI Examples")]
        private static void ImportExamplesContentMenu1()
        {
            AssetDatabase.ImportPackage(PackagePath + "/Package Resources/Brisk UI Sample.unitypackage", true);
        }
    }
}