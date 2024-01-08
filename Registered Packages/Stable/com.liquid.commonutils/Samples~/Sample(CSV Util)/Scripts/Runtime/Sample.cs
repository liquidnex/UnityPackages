using Liquid.CommonUtils;
using UnityEngine;

namespace Liquid.Samples.CSVUtilSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            string csvStr = "[string]Name,[int]ID\n"
                          + "Alice,0\n"
                          + "Bob,1";

            CSVTable t = new CSVTable("Main", csvStr);
            Debug.Log(t.Serialize());
        }
    }
}