using Liquid.CommonUtils;
using UnityEngine;

namespace Liquid.Samples.EnvironmentManagerSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Debug.Log(EnvironmentManager.Instance.PlatformEnvir);
            Debug.Log(EnvironmentManager.Instance.DeliveryEnvir);
        }
    }
}