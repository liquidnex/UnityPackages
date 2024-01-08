using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Liquid.CommonUtils
{
    public class JobManager : MonoBehaviour
    {
        /// <summary>
        /// Use ITransitiveJob to schedule a single job that runs in parallel to other jobs and the main thread.
        /// Objects that inherit ITransitiveJob are used through the ScheduleJob function.
        /// </summary>
        /// <typeparam name="T">Original type of job data.</typeparam>
        public interface ITransitiveJob<T> : IJob
            where T : struct
        {
            /// <summary>
            /// Result is used to access the calculation results.
            /// </summary>
            public NativeArray<T> Result
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Parallel-for jobs allow you to perform the same independent operation for each element of a native container or for a fixed number of iterations.
        /// Objects that inherit ITransitiveJobParallelFor are used through the ScheduleJobParallelFor function.
        /// </summary>
        /// <typeparam name="T">Original type of job data.</typeparam>
        public interface ITransitiveJobParallelFor<T> : IJobParallelFor
            where T : struct
        {
            /// <summary>
            /// Result is used to access the calculation results.
            /// </summary>
            public NativeArray<T> Result
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Provides multithreaded computing for the Transform.
        /// Objects that inherit ITransitiveJobParallelFor are used through the ScheduleJobParallelForTransform function.
        /// </summary>
        public interface ITransitiveJobParallelForTransform : IJobParallelForTransform
        {
            /// <summary>
            /// Result is used to access the calculation results.
            /// </summary>
            public TransformAccessArray Result
            {
                get;
                set;
            }
        }

        private static JobManager instance;

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static JobManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("_JobManager");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<JobManager>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Schedule a job for execution on a worker thread.
        /// </summary>
        /// <typeparam name="R">Type of result.</typeparam>
        /// <param name="jobData">The data to be calculated, and inherits the interface of the ITransitiveJob.</param>
        /// <param name="resultListLen">Length of calculation result list.</param>
        /// <param name="waitHandle">Dependencies are used to ensure that a job executes on workerthreads after the dependency has completed execution. Making sure that two jobs reading or writing to same data do not run in parallel.</param>
        /// <param name="jobType">Used to specify allocation type for NativeArray.</param>
        /// <returns>A handle for current job.</returns>
        public JobHandle ScheduleJob<J, R>
        (
            ref J jobData,
            int resultListLen,
            JobHandle? waitHandle = null,
            Allocator jobType = Allocator.TempJob
        )
            where J : struct, ITransitiveJob<R>
            where R : struct
        {
            NativeArray<R> transitiveResult = new NativeArray<R>(resultListLen, jobType);
            jobData.Result = transitiveResult;

            JobHandle handle;
            if (waitHandle == null)
                handle = jobData.Schedule();
            else
                handle = jobData.Schedule(waitHandle.Value);
            return handle;
        }

        /// <summary>
        /// Schedule a parallel-for job for execution on a worker thread.
        /// </summary>
        /// <typeparam name="R">Type of result.</typeparam>
        /// <param name="jobData">The data to be calculated, and inherits the interface of the ITransitiveJobParallelFor.</param>
        /// <param name="batchCount">Granularity in which workstealing is performed. A value of 32, means the job queue will steal 32 iterations and then perform them in an efficient inner loop.</param>
        /// <param name="resultListLen">Length of calculation result list.</param>
        /// <param name="waitHandle">Dependencies are used to ensure that a job executes on workerthreads after the dependency has completed execution. Making sure that two jobs reading or writing to same data do not run in parallel.</param>
        /// <param name="jobType">Used to specify allocation type for NativeArray.</param>
        /// <returns>A handle for current job.</returns>
        public JobHandle ScheduleJobParallelFor<J, R>
        (
            ref J jobData,
            int batchCount,
            int resultListLen,
            JobHandle? waitHandle = null,
            Allocator jobType = Allocator.TempJob
        )
            where J : struct, ITransitiveJobParallelFor<R>
            where R : struct
        {
            NativeArray<R> transitiveResult = new NativeArray<R>(resultListLen, jobType);
            jobData.Result = transitiveResult;

            JobHandle handle;
            if (waitHandle == null)
                handle = jobData.Schedule(resultListLen, batchCount);
            else
                handle = jobData.Schedule(resultListLen, batchCount, waitHandle.Value);
            return handle;
        }

        /// <summary>
        /// Schedule a Transform parallel-for job for execution on a worker thread.
        /// </summary>
        /// <param name="jobData">The data to be calculated, and inherits the interface of the ITransitiveJobParallelForTransform.</param>
        /// <param name="trans">A list of Transform.</param>
        /// <param name="waitHandle">Dependencies are used to ensure that a job executes on workerthreads after the dependency has completed execution. Making sure that two jobs reading or writing to same data do not run in parallel.</param>
        /// <returns>A handle for current job.</returns>
        public JobHandle ScheduleJobParallelForTransform<J>
        (
            ref J jobData,
            Transform[] trans,
            JobHandle? waitHandle = null
        )
            where J : struct, ITransitiveJobParallelForTransform
        {
            TransformAccessArray transitiveResult = new TransformAccessArray(trans);
            jobData.Result = transitiveResult;

            JobHandle handle;
            if (waitHandle == null)
                handle = jobData.Schedule(jobData.Result);
            else
                handle = jobData.Schedule(jobData.Result, waitHandle.Value);
            return handle;
        }

        /// <summary>
        /// Ensures that the job has completed.
        /// </summary>
        /// <typeparam name="R">Type of result.</typeparam>
        /// <param name="jobData">Job data.</param>
        /// <param name="handle">Job handle.</param>
        /// <returns>The calculation results.</returns>
        public List<R> CompleteJob<J, R>(ref J jobData, ref JobHandle handle)
            where J : struct, ITransitiveJob<R>
            where R : struct
        {
            if (!handle.IsCompleted)
                handle.Complete();

            NativeArray<R> nativeResult = jobData.Result;
            List<R> result = new List<R>(nativeResult.ToArray());

            ClearJobData(jobData);
            nativeResult.Dispose();
            return result;
        }

        /// <summary>
        /// Ensures that the job has completed.
        /// </summary>
        /// <typeparam name="R">Type of result.</typeparam>
        /// <param name="jobData">Job data.</param>
        /// <param name="handle">Job handle.</param>
        /// <returns>The calculation results.</returns>
        public List<R> CompleteJobParallelFor<J, R>(ref J jobData, ref JobHandle handle)
            where J : struct, ITransitiveJobParallelFor<R>
            where R : struct
        {
            if (!handle.IsCompleted)
                handle.Complete();

            NativeArray<R> nativeResult = jobData.Result;
            List<R> result = new List<R>(nativeResult.ToArray());

            ClearJobData(jobData);
            return result;
        }

        /// <summary>
        /// Ensures that the job has completed.
        /// </summary>
        /// <param name="jobData">Job data.</param>
        /// <param name="handle">Job handle.</param>
        /// <returns>The calculation results.</returns>
        public Transform[] CompleteJobParallelForTransform<J>(ref J jobData, ref JobHandle handle)
            where J : struct, ITransitiveJobParallelForTransform
        {
            if (!handle.IsCompleted)
                handle.Complete();

            TransformAccessArray nativeResult = jobData.Result;
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < nativeResult.length; ++i)
            {
                result.Add(nativeResult[i]);
            }

            ClearJobData(jobData);
            return result.ToArray();
        }

        private void ClearJobData<J>(J jobData)
            where J : struct
        {
            FieldInfo[] fields = typeof(J).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (FieldInfo f in fields)
            {
                if (f.FieldType.IsSubClassOfRawGeneric(typeof(NativeArray<>)))
                {
                    object v = f.GetValue(jobData);
                    if (v == null)
                        continue;

                    Type vt = v.GetType();
                    if (vt == null)
                        continue;

                    MethodInfo mi = vt.GetMethod(
                        "Dispose",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                        null,
                        CallingConventions.Any,
                        new Type[] { },
                        null
                    );
                    if (mi == null)
                        continue;

                    mi.Invoke(v, new object[] { });
                }
            }
        }

        private void Awake()
        {
            if (instance != null &&
                instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}