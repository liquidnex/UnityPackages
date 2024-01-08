using UnityEngine;

namespace Liquid.CommonUtils
{
    public static class EnvironmentUtil
    {
        public static bool SetTag(GameObject go, string tag, bool recursion)
        {
            if (go == null)
                return false;

            if (recursion)
            {
                Transform[] tsList = go.GetComponentsInChildren<Transform>();
                if (tsList == null)
                    return false;
                for (int i = 0; i < tsList.Length; ++i)
                {
                    Transform ts = tsList[i];
                    if (ts == null || ts.gameObject == null)
                        continue;

                    ts.gameObject.tag = tag;
                }
            }
            else
            {
                go.tag = tag;
            }

            return true;
        }

        public static bool SetLayer(GameObject go, int layerIndex, bool recursion)
        {
            if (go == null)
                return false;

            if (recursion)
            {
                Transform[] tsList = go.GetComponentsInChildren<Transform>();
                if (tsList == null)
                    return false;
                for (int i = 0; i < tsList.Length; ++i)
                {
                    Transform ts = tsList[i];
                    if (ts == null || ts.gameObject == null)
                        continue;

                    ts.gameObject.layer = layerIndex;
                }
            }
            else
            {
                go.layer = layerIndex;
            }

            return true;
        }
    }
}