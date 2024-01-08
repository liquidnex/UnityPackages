using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Liquid.ObjectPool
{
    /// <summary>
    /// Object pool provides a scalable dynamic container to spawn and recycle game objects of the same type in real time.
    /// </summary>
    public class ObjectPool
    {
        private enum WaterLevelEnum
        { 
            NONE,
            LOW_LEVEL,
            NORMAL,
            HIGH_LEVEL
        }

        public ObjectPool(int preheatVolume, GameObject initialPrefab)
        {
            prefab = initialPrefab;
            for (int i = 0; i < preheatVolume; ++i)
            {
                Create();
            }
        }

        /// <summary>
        /// Spawn an object templated with the bound prefab.
        /// </summary>
        /// <param name="expireDeltaSec">
        /// Specify an expire time.
        /// When it ends, the created object will be destroyed automatically.
        /// When this value is null, it will not be destroyed automatically.
        /// </param>
        /// <returns>Spawned game object.</returns>
        public ObjectPoolElem Spawn(float? expireDeltaSec = null)
        {
            ObjectPoolElem e = objects.Find(o => !o.IsInUse);
            if (e == null)
            {
                e = Create();
            }

            var spawnMethod = elemType.GetMethod("Spawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            spawnMethod.Invoke(e, new object[] { expireDeltaSec });
            return e;
        }

        /// <summary>
        /// Recycle a specific object from the object pool.
        /// </summary>
        /// <param name="obj">Objects to be recycled.</param>
        public void Recycle(ObjectPoolElem obj)
        {
            if (obj == null)
                return;

            var recycle = elemType.GetMethod("Recycle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            recycle.Invoke(obj, new object[] {});
        }

        /// <summary>
        /// Expand object pool.
        /// </summary>
        /// <param name="amount">Amount to expand.</param>
        public void Expand(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                Create();
            }
        }

        /// <summary>
        /// Shrink object pool.
        /// </summary>
        /// <param name="amount">Amount to shrink.</param>
        /// <returns>Failed to shrink the number of elements because of usage restrictions.</returns>
        public int Shrink(int amount)
        {
            if (objects.Count - amount < ObjectPoolManager.Instance.MinimumVolume)
                return 0;

            for (int i = 0; i < amount; ++i)
            {
                ObjectPoolElem e = objects.Find(o => !o.IsInUse);
                if (e != null)
                {
                    var destroy = elemType.GetMethod("Destroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    destroy.Invoke(e, new object[] {});
                    objects.Remove(e);
                }
                else
                {
                    return amount - i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Destroy all pattern records and pool objects.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                ObjectPoolElem e = objects[i];
                if (e != null)
                {
                    var destroy = elemType.GetMethod("Destroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    destroy.Invoke(e, new object[] { });
                    objects.Remove(e);
                }
            }

            objects.Clear();
        }

        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            ClearExpire(deltaTime);

            if (!ObjectPoolManager.Instance.IsPredictionEnabled)
                return;

            PatternRecord(unscaledDeltaTime);

            PoolChange(unscaledDeltaTime);

            ChangeAndDischange();
        }

        private void ClearExpire(float deltaTime)
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                ObjectPoolElem e = objects[i];
                if (e != null)
                {
                    if (e.ExpireDeltaSecond == null)
                        continue;

                    e.ExpireDeltaSecond -= deltaTime;
                    if (e.ExpireDeltaSecond <= 0)
                    {
                        var recycle = elemType.GetMethod("Recycle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        recycle.Invoke(e, new object[] { });
                    }
                }
            }
        }

        private void PatternRecord(float unscaledDeltaTime)
        {
            partternCountSec += unscaledDeltaTime;
            if (partternCountSec >= ObjectPoolManager.Instance.PartternRecordIntervalSenconds)
            {
                partternCountSec = 0;
                usedPoolCountRecord.Add(UseCount);
                if (CheckCurrentRecordEnd())
                {
                    int? firstBigJitterIdx = CheckFirstBigJitterIdx(0, usedPoolCountRecord.Count - 1);
                    if (firstBigJitterIdx != null)
                    {
                        int count = usedPoolCountRecord.Count - ObjectPoolManager.Instance.MinimumStableCountOfEndPattern + 1 - firstBigJitterIdx.Value;
                        if (count > 0)
                        {
                            List<int> pattern = usedPoolCountRecord.GetRange(firstBigJitterIdx.Value, count);
                            patternManager.AddPattern(pattern);
                        }
                    }

                    usedPoolCountRecord = new List<int>();
                }
            }
        }

        private void PoolChange(float unscaledDeltaTime)
        {
            if (newPoolProtectCountSec > 0)
                newPoolProtectCountSec -= unscaledDeltaTime;

            predictionCountSec += unscaledDeltaTime;
            if (predictionCountSec >= ObjectPoolManager.Instance.PredictionIntervalSeconds)
            {
                predictionCountSec = 0;
                int change = Prediction(CheckWaterLevel());

                if (newPoolProtectCountSec > 0 &&
                    change < 0)
                    return;

                needCreateCount += change;
            }
        }

        private void ChangeAndDischange()
        {
            int abs = Mathf.Abs(needCreateCount);
            int amount =
                abs > ObjectPoolManager.Instance.MaximumChangePerUpdate ?
                    ObjectPoolManager.Instance.MaximumChangePerUpdate : abs;

            if (needCreateCount < 0)
            {
                needCreateCount += abs;
                int surplus = Shrink(amount);
                if(surplus > 0)
                    needCreateCount -= surplus;
            }
            else if (needCreateCount > 0)
            {
                needCreateCount -= abs;
                Expand(amount);
            }
        }

        private WaterLevelEnum CheckWaterLevel()
        {
            int all = objects.Count;
            if (all == 0)
                return WaterLevelEnum.LOW_LEVEL;

            double useRate = (float)UseCount / all;

            if (useRate >= ObjectPoolManager.Instance.HighWatermark)
                return WaterLevelEnum.HIGH_LEVEL;

            if (useRate <= ObjectPoolManager.Instance.LowWatermark)
                return WaterLevelEnum.LOW_LEVEL;

            return WaterLevelEnum.NORMAL;
        }

        private int Prediction(WaterLevelEnum waterLevelState)
        {
            if (waterLevelState == WaterLevelEnum.NORMAL)
                return 0;

            List<int> effectiveRecord;
            int? idx = CheckFirstBigJitterIdx(0, usedPoolCountRecord.Count - 1);
            if (idx == null)
            {
                effectiveRecord = usedPoolCountRecord;
            }
            else
            {
                effectiveRecord = usedPoolCountRecord.GetRange(idx.Value, usedPoolCountRecord.Count - idx.Value);
            }

            FluctuationPattern p = patternManager.GetFirstMatch(effectiveRecord, FluctuationPatternManager.MatchResultEnum.MATCH);
            if (p != null)
            {
                int? peak = p.PeakValue(effectiveRecord.Count);
                if(peak != null)
                    return peak.Value - objects.Count;
            }

            if (waterLevelState == WaterLevelEnum.LOW_LEVEL)
                return -ObjectPoolManager.Instance.MaximumChangePerUpdate;
            else if (waterLevelState == WaterLevelEnum.HIGH_LEVEL)
                return ObjectPoolManager.Instance.MaximumChangePerUpdate;
            
            return 0;
        }

        private bool CheckCurrentRecordEnd()
        {
            int poolCount = usedPoolCountRecord.Count;
            int minStableCount = ObjectPoolManager.Instance.MinimumStableCountOfEndPattern;
            if (minStableCount < 2)
                minStableCount = 2;
            int minLen = ObjectPoolManager.Instance.MinimumHistoryCount + minStableCount;

            if (poolCount >= ObjectPoolManager.Instance.MaximumHistoryCount)
                return true;

            if (poolCount < minStableCount ||
                poolCount < minLen)
                return false;

            int? result = CheckFirstBigJitterIdx(poolCount-minStableCount, poolCount-1);
            if (result == null)
                return true;
            return false;
        }

        private int? CheckFirstBigJitterIdx(int minIdx, int maxIdx)
        {
            int poolCount = usedPoolCountRecord.Count;
            if (poolCount < 2)
                return null;

            if (minIdx < 0 ||
                maxIdx >= poolCount)
                return null;

            int sum = 0;
            for (int i = minIdx; i <= maxIdx-1; ++i)
            {
                int nextIdx = i + 1;
                sum += Mathf.Abs(usedPoolCountRecord[nextIdx] - usedPoolCountRecord[i]);
                if (sum > ObjectPoolManager.Instance.MaximumJitterOfPattern)
                {
                    return i;
                }
            }

            return null;
        }

        private ObjectPoolElem Create()
        {
            var create = elemType.GetMethod("Create", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            ObjectPoolElem e = create.Invoke(null, new object[] { this, prefab }) as ObjectPoolElem;
            objects.Add(e);
            return e;
        }

        /// <summary>
        /// Associated prefab of this pool.
        /// </summary>
        public GameObject Prefab
        {
            get => prefab;
        }

        /// <summary>
        /// Number of all pool elements.
        /// </summary>
        public int Count
        {
            get => objects.Count;
        }

        /// <summary>
        /// Count number of elements used.
        /// </summary>
        public int UseCount
        {
            get => objects.FindAll(o => o.IsInUse).Count;
        }

        private float newPoolProtectCountSec = ObjectPoolManager.Instance.NewPoolProtectSecond; 
        private float predictionCountSec = 0;
        private float partternCountSec = 0;
        private int needCreateCount = 0;
        private readonly GameObject prefab;
        private List<int> usedPoolCountRecord = new List<int>{0};
        private List<ObjectPoolElem> objects = new List<ObjectPoolElem>();
        private FluctuationPatternManager patternManager = new FluctuationPatternManager();
        private readonly Type elemType = typeof(ObjectPoolElem);
    }
}