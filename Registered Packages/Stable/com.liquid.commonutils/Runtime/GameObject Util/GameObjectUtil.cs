using UnityEngine;
using UnityEngine.SceneManagement;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Util of the UnityEngine.GameObject.
    /// </summary>
    public static class GameObjectUtil
    {
        /// <summary>
        /// Find a game object in all active scenes by path string.
        /// </summary>
        /// <param name="pathString">
        /// Describes the path of the game object to find.
        /// A path string can be any relative path to the target game object.
        /// </param>
        /// <param name="delimiter">Separator in the path string.</param>
        /// <returns>Search result.</returns>
        public static GameObject FindGameObject(string pathString, char delimiter = '/')
        {
            foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                GameObject result = rootObj.FindGameObject(pathString, delimiter);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}