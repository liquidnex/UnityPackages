using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Util of the 3D graphic.
    /// </summary>
    public static class Graphic3DUtil
    {
        public static float FOVV2H(this float d, Camera camera = null)
        {
            float scale;
            if (camera == null)
                scale = Screen.width / (float)Screen.height;
            else
                scale = camera.aspect;
            return Mathf.Atan(Mathf.Tan(d / 2 * Mathf.Deg2Rad) * scale) * 2 * Mathf.Rad2Deg;
        }

        public static float FOVH2V(this float d, Camera camera = null)
        {
            float scale;
            if (camera == null)
                scale = Screen.height / (float)Screen.width;
            else
                scale = camera.aspect;
            return Mathf.Atan(Mathf.Tan(d / 2 * Mathf.Deg2Rad) * scale) * 2 * Mathf.Rad2Deg;
        }
    }
}