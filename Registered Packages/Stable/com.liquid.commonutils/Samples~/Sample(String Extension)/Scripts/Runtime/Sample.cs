using System.IO;
using UnityEngine;

namespace Liquid.Samples.StringExtensionSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            string projectPath = Path.GetFullPath("Assets/..");
            Debug.Log(string.Format($"Project path is {projectPath}"));
            Debug.Log(string.Format("Unity path for project is {0}", projectPath.ToUnityPath()));
        }
    }
}