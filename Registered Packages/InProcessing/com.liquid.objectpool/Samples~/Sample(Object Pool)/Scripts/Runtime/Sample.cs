using System.Collections;
using UnityEngine;

namespace Liquid.ObjectPool.Samples.ObjectPoolSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            ObjectPoolManager.Instance.Preheat(PrefabObject, 50);
        }

        private void Update()
        {
            cd -= Time.deltaTime;
            if (cd <= 0f)
            {
                StartCoroutine(CreateFX());
                cd = Random.Range(12, 14);
            }
        }

        private IEnumerator CreateFX()
        {
            CreateFX(3, 4);
            yield return new WaitForSeconds(1);
            CreateFX(3, 4);
            yield return new WaitForSeconds(1);
            CreateFX(3, 4);
            yield return new WaitForSeconds(1);
            CreateFX(3, 4);
        }

        private void CreateFX(int count, float? expire = null)
        {
            for (int i = 0; i < count; ++i)
            {
                float x = Random.Range(0f, 1f);
                float y = Random.Range(0f, 1f);
                ObjectPoolElem e = ObjectPoolManager.Instance.Spawn(PrefabObject, expire);
                e.Target.transform.localPosition = new Vector3(x, y, 0);
            }
        }

        public GameObject PrefabObject;
        private float cd = 0f;
    }
}