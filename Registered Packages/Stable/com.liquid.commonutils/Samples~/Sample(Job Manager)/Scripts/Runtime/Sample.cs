using Liquid.CommonUtils;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Liquid.Samples.JobManagerSample
{
    public struct MyJob : JobManager.ITransitiveJobParallelFor<float>
    {
        public void Init()
        {
            a1 = new NativeArray<float>(2, Allocator.TempJob);
            a2 = new NativeArray<float>(2, Allocator.TempJob);
            a1[0] = 0.1f;
            a1[1] = 0.3f;
            a2[0] = 0.2f;
            a2[1] = 0.4f;
        }

        public NativeArray<float> Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        public void Execute(int i)
        {
            result[i] = a1[i] + a2[i];
        }

        private NativeArray<float> a1;
        private NativeArray<float> a2;
        private NativeArray<float> result;
    }

    public class Sample : MonoBehaviour
    {
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            MyJob j = new MyJob();
            j.Init();

            JobHandle jobHandle = JobManager.Instance.ScheduleJobParallelFor<MyJob, float>(ref j, 1, 2);
            List<float> result = JobManager.Instance.CompleteJobParallelFor<MyJob, float>(ref j, ref jobHandle);

            foreach (var r in result)
            {
                Debug.Log(string.Format("{0} ", r));
            }
        }
    }
}