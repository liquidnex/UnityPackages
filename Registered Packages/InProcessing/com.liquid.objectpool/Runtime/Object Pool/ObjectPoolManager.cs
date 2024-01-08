using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Liquid.ObjectPool
{
    /// <summary>
    /// A dynamic object life cycle manager with the function of historical pattern matching prediction based on statistics.
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        /// <summary>
        /// Preheat an object pool.
        /// </summary>
        /// <param name="prefab">Prefab game object for preheat.</param>
        /// <param name="amount">
        /// The size of the pool to be preheated.
        /// If the value is less than or equal to 0, the pool will be preheated to the default size.
        /// </param>
        public void Preheat(GameObject prefab, int amount = 0)
        {
            if (amount <= 0)
                amount = DefaultPreheatVolume;

            ObjectPool targetPool = GetPool(prefab);
            if (targetPool != null)
            {
                int remainCount = targetPool.Count - targetPool.UseCount;
                if (remainCount <= 0)
                    remainCount = 0;

                int needAddAmount = amount - remainCount;
                if (needAddAmount > 0)
                {
                    targetPool.Expand(needAddAmount);
                }
            }
            else
            {
                ObjectPool newOne = new ObjectPool(amount, prefab);
                poolList.Add(newOne);
            }
        }

        /// <summary>
        /// Spawn an object templated with a specific prefab.
        /// If there is no object pool for this specific prefab yet, create one.
        /// </summary>
        /// <param name="prefab">Prefab game object for spawn.</param>
        /// <param name="expireDeltaSec">
        /// Specify an expire time.
        /// When it ends, the created object will be destroyed automatically.
        /// When this value is null, it will not be destroyed automatically.
        /// </param>
        /// <returns>Spawned game object.</returns>
        public ObjectPoolElem Spawn(GameObject prefab, float? expireDeltaSec = null)
        {
            if (prefab == null)
                return null;

            ObjectPool targetPool = GetPool(prefab);
            if (targetPool != null)
            {
                return targetPool.Spawn(expireDeltaSec);
            }
            else
            {
                ObjectPool newOne = new ObjectPool(DefaultPreheatVolume, prefab);
                poolList.Add(newOne);
                return newOne.Spawn(expireDeltaSec);
            }
        }

        /// <summary>
        /// Recycle a specific object from the object pool.
        /// </summary>
        /// <param name="obj">Objects to be recycled.</param>
        public void Recycle(ObjectPoolElem obj)
        {
            if (obj == null ||
                obj.MasterPool == null)
                return;

            obj.MasterPool.Recycle(obj);
        }

        /// <summary>
        /// Destroy all object pools and their associated pattern records and objects.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < poolList.Count; ++i)
            {
                poolList[i].Clear();
            }
            poolList.Clear();
        }

        /// <summary>
        /// Get an object pool according to prefab.
        /// </summary>
        /// <param name="prefab">Specific prefab.</param>
        /// <returns>Pool found.</returns>
        public ObjectPool GetPool(GameObject prefab)
        {
            if (prefab == null)
                return null;

            foreach (ObjectPool p in poolList)
            {
                if (p.Prefab == prefab)
                {
                    return p;
                }
            }

            return null;
        }

        private void OnSceneUnload(Scene current)
        {
            Clear();
        }

        private void Init()
        {
            SceneManager.sceneUnloaded += instance.OnSceneUnload;
        }

        private void Awake()
        {
            if (instance != null &&
                instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Update()
        {
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            float deltaTime = Time.deltaTime;
            foreach (ObjectPool op in poolList)
            {
                op.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static ObjectPoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("_ObjectPoolManager");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<ObjectPoolManager>();
                    instance.Init();
                }
                return instance;
            }
        }

        /// <summary>
        /// Whether to enable automatic pool scaling prediction.
        /// When true, enables automatic pattern capture and pattern matching.
        /// When false, will not actively operate on the size of the pool.
        /// </summary>
        public bool IsPredictionEnabled = true;

        /// <summary>
        /// The duration of the protection mechanism of the new object pool.
        /// During this time, the pool will not shrink due to predictions.
        /// </summary>
        public float NewPoolProtectSecond = 5;

        /// <summary>
        /// Interval seconds of predicted behavior.
        /// Longer intervals represent lazier prediction behavior.
        /// </summary>
        public float PredictionIntervalSeconds = 2;

        /// <summary>
        /// Pool minimum size.
        /// The pool size cannot be smaller than this value, either automatically or manually.
        /// </summary>
        public int MinimumVolume = 10;

        /// <summary>
        /// High water mark of object pool.
        /// If the occupancy rate of the object pool is higher than this value,
        /// it is considered to need automatic expansion.
        /// Pool capacity does not change when between high and low watermarks.
        /// </summary>
        public double HighWatermark = 0.85;

        /// <summary>
        /// Low water mark of object pool.
        /// If the occupancy rate of the object pool is lower than this value,
        /// it is considered to need automatic shrinking.
        /// Pool capacity does not change when between high and low watermarks.
        /// </summary>
        public double LowWatermark = 0.5;

        /// <summary>
        /// Maximum number of game objects that can be created/destroyed per second.
        /// </summary>
        public int MaximumChangePerUpdate = 10;

        /// <summary>
        /// Interval seconds of pattern recording.
        /// The smaller the value, the more precise the record.
        /// </summary>
        public float PartternRecordIntervalSenconds = 1;

        /// <summary>
        /// The maximum amount of pattern records.
        /// </summary>
        public int MaximumPatternCount = 20;

        /// <summary>
        /// Number of tail values of minimum stable sequence check.
        /// </summary>
        public int MinimumStableCountOfEndPattern = 3;

        /// <summary>
        /// Maximum jitter value allowed in stable detection.
        /// </summary>
        public int MaximumJitterOfPattern = 2;

        /// <summary>
        /// The minimum amount of data in a pattern record.
        /// </summary>
        public int MinimumHistoryCount = 6;

        /// <summary>
        /// The maximum amount of data in a pattern record.
        /// </summary>
        public int MaximumHistoryCount = 20;

        /// <summary>
        /// Goodness of fit in statistical method.
        /// A goodness of fit above or equal to this value will be considered pattern matched.
        /// </summary>
        public float FitGoodnees = 0.8f;

        /// <summary>
        /// The default size of the object pool.
        /// </summary>
        public int DefaultPreheatVolume = 40;

        private List<ObjectPool> poolList = new List<ObjectPool>();
        private static ObjectPoolManager instance;
    }
}