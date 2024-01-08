using UnityEngine;

namespace Liquid.Samples.TimerSample
{
    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Timer t = Timer.CreateTimer();
            t.StartTimer(
                120,
                f => {
                    Debug.Log(
                        string.Format("Now is {0}", f)
                    );
                }
            );
        }
    }
}