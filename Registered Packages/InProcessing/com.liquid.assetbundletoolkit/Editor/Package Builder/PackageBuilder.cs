using Liquid.CommonUtils;
using Liquid.CommonUtils.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Liquid.AssetBundleToolkit.Editor
{
    public class PackageBuilder
    {
        private struct PackageArgs
        {
            public enum PlatformType
            {
                NONE,
                WIN32,
                WIN64,
                MAC,
                ANDROID,
                IOS,
                WEBGL
            }

            public PackageArgs(
                string argStrBundleVersion,
                string argStrDeliveryChannel,
                string argStrLocalOutPath,
                string argStrPlatform,
                string argStrDevelopmentBuild,
                string argAndroidKeystoreName,
                string argAndroidKeystorePass,
                string argAndroidKeyaliasName,
                string argAndroidKeyaliasPass)
            {
                BundleVersion = argStrBundleVersion;
                DeliveryChannel = argStrDeliveryChannel;
                LocalOutPath = argStrLocalOutPath.ToUnityPath();

                Platform = ParseStringToPlatformType(argStrPlatform);
                DevelopmentBuild = argStrDevelopmentBuild;

                AndroidKeystoreName = argAndroidKeystoreName;
                AndroidKeystorePass = argAndroidKeystorePass;
                AndroidKeyaliasName = argAndroidKeyaliasName;
                AndroidKeyaliasPass = argAndroidKeyaliasPass;
            }

            public bool Legal
            {
                get
                {
                    if (DeliveryChannel == null)
                        return false;
                    if (LocalOutPath == null)
                        return false;
                    if (Platform == PlatformType.NONE)
                        return false;
                    if (Platform == PlatformType.ANDROID &&
                       (string.IsNullOrEmpty(AndroidKeystoreName) ||
                        string.IsNullOrEmpty(AndroidKeystorePass) ||
                        string.IsNullOrEmpty(AndroidKeyaliasName) ||
                        string.IsNullOrEmpty(AndroidKeyaliasPass)))
                        return false;

                    return true;
                }
            }

            public static PlatformType ParseStringToPlatformType(string str)
            {
                PlatformType pt = PlatformType.NONE;
                if (string.IsNullOrEmpty(str))
                    return pt;

                if (str == "Win32")
                {
                    pt = PlatformType.WIN32;
                }
                else if (str == "Win64")
                {
                    pt = PlatformType.WIN64;
                }
                else if (str == "Mac")
                {
                    pt = PlatformType.MAC;
                }
                else if (str == "Android")
                {
                    pt = PlatformType.ANDROID;
                }
                else if (str == "iOS")
                {
                    pt = PlatformType.IOS;
                }
                else if (str == "WebGL")
                {
                    pt = PlatformType.WEBGL;
                }
                return pt;
            }

            public readonly string BundleVersion;
            public readonly string DeliveryChannel;
            public readonly string LocalOutPath;
            public readonly PlatformType Platform;
            public readonly string DevelopmentBuild;

            public readonly string AndroidKeystoreName;
            public readonly string AndroidKeystorePass;
            public readonly string AndroidKeyaliasName;
            public readonly string AndroidKeyaliasPass;
        }

        private static void Build()
        {
            PackageArgs args = GetArgs("Liquid.AssetBundleToolkit.Editor.PackageBuilder.Build");
            if (!args.Legal)
            {
                Debug.LogError("Illegal Arguments! Check it for package.");
                return;
            }

            string localOutFolderFullPath = Path.GetDirectoryName(args.LocalOutPath);
            if (!Directory.Exists(localOutFolderFullPath))
                Directory.CreateDirectory(localOutFolderFullPath);

            BuildTarget buildTarget = BuildTarget.NoTarget;
            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
            if (args.Platform == PackageArgs.PlatformType.WIN32)
            {
                buildTarget = BuildTarget.StandaloneWindows;
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            else if (args.Platform == PackageArgs.PlatformType.WIN64)
            {
                buildTarget = BuildTarget.StandaloneWindows64;
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            else if (args.Platform == PackageArgs.PlatformType.MAC)
            {
                buildTarget = BuildTarget.StandaloneOSX;
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            else if (args.Platform == PackageArgs.PlatformType.ANDROID)
            {
                buildTarget = BuildTarget.Android;
                buildTargetGroup = BuildTargetGroup.Android;
            }
            else if (args.Platform == PackageArgs.PlatformType.IOS)
            {
                buildTarget = BuildTarget.iOS;
                buildTargetGroup = BuildTargetGroup.iOS;
            }
            else if (args.Platform == PackageArgs.PlatformType.WEBGL)
            {
                buildTarget = BuildTarget.WebGL;
                buildTargetGroup = BuildTargetGroup.WebGL;
            }

            bool switchSuccess = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
            if (!switchSuccess)
            {
                Debug.LogError("Switch platform faild.");
                return;
            }

            if (args.Platform == PackageArgs.PlatformType.ANDROID)
            {
                string keystoreFileName = args.AndroidKeystoreName;
                if (string.IsNullOrEmpty(keystoreFileName))
                {
                    Debug.LogError("String AndroidKeystoreName is Empty.");
                    return;
                }

                string[] filePaths = Directory.GetFiles(Application.dataPath, keystoreFileName, SearchOption.AllDirectories);
                if (filePaths.Length == 0)
                {
                    Debug.LogError("Keystore file cannot found.");
                    return;
                }

                string keystoreFilefullPath = filePaths[0].ToUnityPath();
                string keystoreFileAssetPath = FolderUtil.GetAssetPath(keystoreFilefullPath);
                if (string.IsNullOrEmpty(keystoreFileAssetPath))
                {
                    Debug.LogError("Keystore file cannot found.");
                    return;
                }

                PlayerSettings.Android.keystoreName = keystoreFileAssetPath;
                PlayerSettings.Android.keystorePass = args.AndroidKeystorePass;
                PlayerSettings.Android.keyaliasName = args.AndroidKeyaliasName;
                PlayerSettings.Android.keyaliasPass = args.AndroidKeyaliasPass;
            }

            if (args.BundleVersion != null)
            {
                PlayerSettings.bundleVersion = args.BundleVersion;
            }

            Symbols symbols = new Symbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));
            foreach (string dtName in Enum.GetNames(typeof(EnvironmentManager.DeliveryType)))
            {
                if (symbols.Contains(dtName))
                    symbols.Remove(dtName);
            }
            symbols.Add(args.DeliveryChannel);
            symbols.Save();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetBuildScenes();
            buildPlayerOptions.locationPathName = args.LocalOutPath;
            buildPlayerOptions.target = buildTarget;

            if (args.DevelopmentBuild != null &&
                args.DevelopmentBuild == "On")
            {
                buildPlayerOptions.options = BuildOptions.Development | BuildOptions.StrictMode;
            }
            else if (args.DevelopmentBuild == null ||
                     args.DevelopmentBuild == "Off")
            {
                buildPlayerOptions.options = BuildOptions.StrictMode;
            }

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + report.summary.totalSize + " bytes.");
            }
            if (report.summary.result == BuildResult.Failed)
            {
                Debug.LogError("Build failed.");
            }
        }

        private static PackageArgs GetArgs(string methodName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            Regex argsRgx = new Regex(@"\-\-(\S+)\=(\S+)");

            bool beginParse = false;
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (beginParse)
                {
                    Match matches = argsRgx.Match(arg);
                    if (matches.Success &&
                        matches.Groups.Count == 3)
                    {
                        string k = matches.Groups[1].Value;
                        string v = matches.Groups[2].Value;
                        args.Add(k, v);
                    }
                }
                else if (arg == methodName)
                {
                    beginParse = true;
                }
            }

            return ParseArgs(args);
        }

        private static PackageArgs ParseArgs(Dictionary<string, string> args)
        {
            // Common.
            string bundleVersion;
            string deliveryChannel;
            string localOutPath;
            string platform;
            string developmentBuild;

            // For android only.
            string androidKeystoreName = null;
            string androidKeystorePass = null;
            string androidKeyaliasName = null;
            string androidKeyaliasPass = null;

            if (!args.TryGetValue("BundleVersion", out bundleVersion))
            {
                Debug.LogWarning("Warning: missing configuration BundleVersion, the version configured in the project settings will be used.");
            }
            if (!args.TryGetValue("DeliveryChannel", out deliveryChannel))
            {
                Debug.LogWarning("Fatal Error: missing configuration Server. This build will be stopped.");
            }
            if (!args.TryGetValue("LocalOutPath", out localOutPath))
            {
                Debug.LogWarning("Fatal Error: missing configuration LocalOutPath. This build will be stopped.");
            }
            if (!args.TryGetValue("Platform", out platform))
            {
                Debug.LogWarning("Fatal Error: missing configuration Platform. This build will be stopped.");
            }
            if (!args.TryGetValue("DevelopmentBuild", out developmentBuild))
            {
                Debug.LogWarning("Fatal Error: missing configuration DevelopmentBuild. This build will be stopped.");
            }

            PackageArgs.PlatformType pt = PackageArgs.ParseStringToPlatformType(platform);
            if (pt == PackageArgs.PlatformType.ANDROID)
            {
                if (!args.TryGetValue("AndroidKeystoreName", out androidKeystoreName))
                {
                    Debug.LogWarning("Fatal Error: missing configuration AndroidKeystoreName. This build will be stopped.");
                }
                if (!args.TryGetValue("AndroidKeystorePass", out androidKeystorePass))
                {
                    Debug.LogWarning("Fatal Error: missing configuration AndroidKeystorePass. This build will be stopped.");
                }
                if (!args.TryGetValue("AndroidKeyaliasName", out androidKeyaliasName))
                {
                    Debug.LogWarning("Fatal Error: missing configuration AndroidKeyaliasName. This build will be stopped.");
                }
                if (!args.TryGetValue("AndroidKeyaliasPass", out androidKeyaliasPass))
                {
                    Debug.LogWarning("Fatal Error: missing configuration AndroidKeyaliasPass. This build will be stopped.");
                }
            }

            PackageArgs result = new PackageArgs(
                bundleVersion,
                deliveryChannel,
                localOutPath,
                platform,
                developmentBuild,
                androidKeystoreName,
                androidKeystorePass,
                androidKeyaliasName,
                androidKeyaliasPass);
            return result;
        }

        private static string[] GetBuildScenes()
        {
            List<string> sceneNames = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene != null &&
                    scene.enabled
                )
                {
                    sceneNames.Add(scene.path);
                }
            }

            return sceneNames.ToArray();
        }
    }
}