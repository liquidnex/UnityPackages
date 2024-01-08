using UnityEngine;

namespace Liquid.Samples.SingletonSample
{
    public class S : Singleton<S>
    {
        // Initialize all fields here.
        protected override void Init()
        {
            Debug.Log("S Initialized.");
            InitializedStr = "GOOD USAGE";
        }

        public string InitializedStr;
        public string UninitializedStr = "BAD USAGE";
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            S s1 = S.Instance;
            S s2 = S.Instance;

            Debug.Log(s1.InitializedStr);
            Debug.Log(s1.UninitializedStr);
            if (s1 == s2)
                Debug.Log("s1 == s2.");
        }
    }
}