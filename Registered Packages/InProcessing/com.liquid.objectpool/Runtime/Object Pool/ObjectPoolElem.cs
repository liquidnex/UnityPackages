using UnityEngine;

namespace Liquid.ObjectPool
{
    /// <summary>
    /// A ObjectPoolElem describes the game objects allocated in the object pool.
    /// </summary>
    public class ObjectPoolElem
    {
        private static ObjectPoolElem Create(ObjectPool ownerPool, GameObject prefab = null)
        {
            ObjectPoolElem creation = new ObjectPoolElem(ownerPool, prefab);
            return creation;
        }

        private void Destroy()
        {
            isInUse = false;
            masterPool = null;
            if (target != null)
            {
                GameObject.Destroy(target);
                target = null;
            }
            ExpireDeltaSecond = null;
        }

        private void Spawn(float? expireDeltaSec = null)
        {
            ExpireDeltaSecond = expireDeltaSec;
            isInUse = true;
            if (target != null)
                target.SetActive(true);
        }

        private void Recycle()
        {
            isInUse = false;
            ExpireDeltaSecond = -1f;
            if (target != null)
                target.SetActive(false);
        }

        private ObjectPoolElem(ObjectPool ownerPool, GameObject prefab = null)
        {
            masterPool = ownerPool;

            if (prefab == null)
                target = new GameObject();
            else
            {
                target = GameObject.Instantiate(prefab);
                target.name = prefab.name;
            }

            if (target != null)
                target.SetActive(false);
        }

        /// <summary>
        /// Get the object pool information to which this object belongs.
        /// </summary>
        public ObjectPool MasterPool
        {
            get => masterPool;
        }

        /// <summary>
        /// Get game object inside.
        /// </summary>
        public GameObject Target
        {
            get => target;
        }

        /// <summary>
        /// Detects whether this object is already in use.
        /// Returns true if used, false otherwise.
        /// </summary>
        public bool IsInUse
        {
            get => isInUse;
        }

        /// <summary>
        /// The number of seconds left for the object to be automatically destroyed.
        /// If it is null, it will not be automatically destroyed.
        /// </summary>
        public float? ExpireDeltaSecond;

        private ObjectPool masterPool;
        private GameObject target;
        private bool isInUse = false;
    }
}