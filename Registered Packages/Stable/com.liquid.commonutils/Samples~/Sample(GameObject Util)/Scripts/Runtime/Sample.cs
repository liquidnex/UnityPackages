using Liquid.CommonUtils;
using System;
using UnityEngine;

namespace Liquid.Samples.GameObjectUtilSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            // Find by Global Search
            GameObject root = GameObjectUtil.FindGameObject("Root");

            // Find by Full Name
            GameObject b0 = root.FindGameObject("A/B");

            // Find by Matching Name Mode
            GameObject c0 = root.FindGameObject("B/C");

            // Find without Path
            GameObject b1 = root.FindGameObject("B");

            // Find Outer A
            GameObject outerA = root.FindGameObject("A");

            // Find Inner A
            GameObject innerA = b1.FindGameObject("A");

            Debug.Log(string.Format("GameObject root name is :{0}", root.name));
            Debug.Log(string.Format("GameObject b0 name is :{0}", b0.name));
            Debug.Log(string.Format("GameObject c1 name is :{0}", c0.name));

            Debug.Log(string.Format("GameObject b1 name is :{0}", b1.name));
            Debug.Log(string.Format("GameObject outerA name is :{0}, tag is {1}", outerA.name, outerA.tag));
            Debug.Log(string.Format("GameObject innerA name is :{0}, tag is {1}", innerA.name, innerA.tag));
        }
    }
}