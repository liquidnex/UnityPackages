using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Extension of the UnityEngine.GameObject.
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Find game object by path string.
        /// </summary>
        /// <param name="root">Current game object.</param>
        /// <param name="pathString">
        /// Describes the path of the game object to find based on the current object.
        /// A path string can be any relative path from root to the target game object.
        /// </param>
        /// <param name="delimiter">Separator in the path string.</param>
        /// <returns>Search result.</returns>
        public static GameObject FindGameObject(this GameObject root, string pathString, char delimiter = '/')
        {
            if (string.IsNullOrEmpty(pathString))
                return null;

            List<string> nameInHierarchy = new List<string>(pathString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));

            if (nameInHierarchy.Count == 0)
                return null;

            string searchRootName = nameInHierarchy[0];
            List<GameObject> gameObjects = new List<GameObject>();
            root.FindAllGameObject(searchRootName, ref gameObjects);

            foreach (GameObject obj in gameObjects)
            {
                GameObject result = obj.FindGameObject(nameInHierarchy);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Find component by path string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">Current game object.</param>
        /// <param name="pathString">
        /// Describes the path of the component to find based on the current object.
        /// A path string can be any relative path from root to the target game object.
        /// </param>
        /// <param name="delimiter">Separator in the path string.</param>
        /// <returns>Search result.</returns>
        public static T FindComponent<T>(this GameObject root, string pathString, char delimiter = '/')
            where T : Component
        {
            GameObject obj = FindGameObject(root, pathString, delimiter);
            if (obj != null)
                return obj.GetComponentInChildren<T>(true);
            return null;
        }

        /// <summary>
        /// Remove all child game objects directly under the specified parent game object.
        /// </summary>
        /// <param name="root">Specified parent game object.</param>
        public static void RemoveAllChilds(this GameObject root)
        {
            if (root == null)
                return;

            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform transform = root.transform.GetChild(i);
                GameObject.Destroy(transform.gameObject);
            }
        }

        /// <summary>
        /// Remove all child game objects directly under the specified parent game object.
        /// </summary>
        /// <param name="root">Specified parent game object.</param>
        public static void RemoveAllChilds(this Transform root)
        {
            if (root == null)
                return;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform transform = root.GetChild(i);
                GameObject.Destroy(transform.gameObject);
            }
        }

        private static GameObject FindGameObject(this GameObject root, string name)
        {
            if (root == null)
                return null;
            if (name == null)
                return null;

            foreach (Transform child in root.transform)
            {
                if (child.gameObject.name == name)
                {
                    return child.gameObject;
                }
            }

            if (root.name == name)
                return root;

            return null;
        }

        private static GameObject FindGameObject(this GameObject root, List<string> nameInHierarchy)
        {
            string[] tmpArracy = new string[nameInHierarchy.Count];
            nameInHierarchy.CopyTo(tmpArracy);
            List<string> copyNameInHierarchy = new List<string>(tmpArracy);

            if (root == null)
                return null;
            if (copyNameInHierarchy.Count == 0)
                return null;

            string name = copyNameInHierarchy[0];
            if (name == null)
                return null;
            copyNameInHierarchy.RemoveAt(0);

            GameObject result = root.FindGameObject(name);
            if (result == null)
                return null;

            if (copyNameInHierarchy.Count == 0)
                return result;
            else
                return result.FindGameObject(copyNameInHierarchy);
        }

        private static void FindAllGameObject(this GameObject root, string name, ref List<GameObject> gameObjects)
        {
            if (gameObjects == null)
                gameObjects = new List<GameObject>();

            if (root == null)
                return;
            if (name == null)
                return;

            if (root.name == name && 
                !gameObjects.Contains(root))
            {
                gameObjects.Add(root);
            }

            foreach (Transform child in root.transform)
            {
                if (child.gameObject.name == name &&
                    !gameObjects.Contains(root))
                {
                    gameObjects.Add(child.gameObject);
                }

                child.gameObject.FindAllGameObject(name, ref gameObjects);
            }

            return;
        }

        public static string GetSiblingID(this GameObject go)
        {
            if(go == null)
                return null;

            StringBuilder sb = new StringBuilder();

            GameObject current = go;
            while(true)
            {
                bool noParent = current?.transform?.parent?.gameObject == null;

                string insertStr;
                if (noParent)
                {
                    insertStr = "0-";
                }
                else
                {
                    string sid = current.transform.GetSiblingIndex().ToString();
                    insertStr = sid + "-";
                }
                sb.Insert(0, insertStr);

                if (noParent)
                    break;
                current = current.transform.parent.gameObject;
            }
            sb = sb.Remove(sb.Length-1, 1);
            return sb.ToString();
        }

        public static bool IsMesh(this GameObject go)
        {
            if (go == null)
                return false;

            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null ||
                mf?.sharedMesh == null)
                return false;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr == null)
                return false;
            return true;
        }
    }
}