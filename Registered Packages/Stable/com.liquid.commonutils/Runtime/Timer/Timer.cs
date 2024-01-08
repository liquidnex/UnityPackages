using System;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// A timer.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        private float tmpElapsedTime;
        private float tmpRemainingTime;
        private float tmpStartPauseTime;
        private float tmpPauseDurationTime;

        private float startTime;
        private float targetTime;

        private bool isIgnoreTimeScale = false;
        private bool isCountDown = false;
        private bool isRepeatWhenStopped = false;
        private bool isDestroyWhenStopped = true;
        private bool isTiming = false;

        private Action<float> onUpdateCallback;
        private Action onCompletedCallback;

        /// <summary>
        /// Get elapsed seconds for timing.
        /// </summary>
        /// <returns>Remain seconds.</returns>
        public float ElapsedTime
        {
            get => Mathf.Clamp(tmpElapsedTime, 0, targetTime);
        }

        /// <summary>
        /// Get remaining seconds for timing.
        /// </summary>
        public float RemainingTime
        {
            get => Mathf.Clamp(tmpRemainingTime, 0, targetTime);
        }

        private float TimeNow
        {
            get => isIgnoreTimeScale ? Time.realtimeSinceStartup : Time.time;
        }

        /// <summary>
        /// Create a timer.
        /// </summary>
        /// <param name="objName">Unity object name for timer.</param>
        /// <returns>Created timer.</returns>    
        public static Timer CreateTimer(string objName = "Timer")
        {
            GameObject g = new GameObject(objName);
            Timer timer = g.AddComponent<Timer>();
            return timer;
        }

        /// <summary>
        /// Start current timer.
        /// </summary>
        /// <param name="targetTimeSec">Target timing seconds.</param>
        /// <param name="isCountDown">Countdown or not.</param>
        /// <param name="isIgnoreTimeScale">Whether to ignore time scale in Unity.</param>
        /// <param name="isRepeatWhenStopped">Whether to repeat when stopped.</param>
        /// <param name="isDestroyWhenStopped">Whether to destroy when stopped.</param>
        /// <param name="onUpdate">On update callback.</param>
        /// <param name="onCompleted">On completed callback.</param>
        public void Launch(
            float targetTimeSec,
            Action<float> onUpdate = null,
            Action onCompleted = null,
            bool isIgnoreTimeScale = false,
            bool isCountDown = false,
            bool isRepeatWhenStopped = false,
            bool isDestroyWhenStopped = true)
        {
            targetTime = targetTimeSec;
            this.isIgnoreTimeScale = isIgnoreTimeScale;
            this.isCountDown = isCountDown;
            this.isRepeatWhenStopped = isRepeatWhenStopped;
            this.isDestroyWhenStopped = isDestroyWhenStopped;

            if (onUpdate != null)
                onUpdateCallback = onUpdate;
            if (onCompleted != null)
                onCompletedCallback = onCompleted;

            tmpStartPauseTime = 0f;
            tmpPauseDurationTime = 0;
            startTime = TimeNow;
            isTiming = true;
        }

        /// <summary>
        /// Stop current timer.
        /// </summary>
        public void Stop()
        {
            isTiming = false;
            tmpPauseDurationTime = 0;
            if (isDestroyWhenStopped)
                Destroy(gameObject);
        }

        /// <summary>
        /// Restart current timer.
        /// </summary>
        public void Restart()
        {
            tmpStartPauseTime = 0f;
            tmpPauseDurationTime = 0f;
            startTime = TimeNow;
        }

        /// <summary>
        /// Pause current timer.
        /// </summary>
        public void Pause()
        {
            if (!isTiming)
                return;

            isTiming = false;
            tmpStartPauseTime = TimeNow;
        }

        /// <summary>
        /// Resume current timer.
        /// </summary>
        public void Resume()
        {
            if (isTiming)
                return;

            tmpPauseDurationTime += (TimeNow - tmpStartPauseTime);
            tmpStartPauseTime = 0f;
            isTiming = true;
        }

        /// <summary>
        /// Change target time.
        /// </summary>
        /// <param name="time">Target time.</param>
        public void ChangeTargetTime(float time)
        {
            isTiming = false;
            tmpStartPauseTime = 0f;
            tmpPauseDurationTime = 0f;
            targetTime = time;
            startTime = TimeNow;
            isTiming = true;
        }

        /// <summary>
        /// Format seconds as a time string.
        /// </summary>
        /// <param name="seconds">Seconds.</param>
        /// <returns>Time string.</returns>
        public static string FormatSeconds(float seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = string.Empty;
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:" + ts.Seconds.ToString("00");
            }
            return str;
        }

        private void Update()
        {
            if (!isTiming)
                return;

            tmpElapsedTime = TimeNow - tmpPauseDurationTime - startTime;
            tmpRemainingTime = targetTime - tmpElapsedTime;

            if (onUpdateCallback != null)
            {
                if (isCountDown)
                    onUpdateCallback(tmpRemainingTime);
                else
                    onUpdateCallback(tmpElapsedTime);
            }

            if (tmpElapsedTime > targetTime)
            {
                if (onCompletedCallback != null)
                    onCompletedCallback();

                if (isRepeatWhenStopped)
                    Restart();
                else
                    Stop();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                Pause();
            else
                Resume();
        }
    }
}