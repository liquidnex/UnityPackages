using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Liquid.EasyInput
{
    /// <summary>
    /// Mouse controller.
    /// </summary>
    public class MouseInputer : IInputer
    {
        private class ButtonTrackerData
        {
            public bool IsMoving = false;
        }

        /// <summary>
        /// Callback when the mouse button is pressed.
        /// </summary>
        /// <param name="pos">Mouse position.</param>
        /// <param name="isPointerOverUI">Whether the pointer is over ui game object.</param>
        /// <param name="button">Button id.</param>
        public delegate void OnClickBegan(Vector2 pos, bool isPointerOverUI, int button);

        /// <summary>
        /// Callback when the mouse button is pressed and moved.
        /// </summary>
        /// <param name="pos">Mouse position.</param>
        /// <param name="deltaPerSec">
        /// The average moving distance per second,
        /// which is used to reflect the variable of moving speed.
        /// </param>
        /// <param name="isPointerOverUI">Whether the pointer is over ui game object.</param>
        /// <param name="button">Button id.</param>
        public delegate void OnClickMoved(Vector2 pos, float deltaPerSec, bool isPointerOverUI, int button);

        /// <summary>
        /// Callback when the mouse button is released.
        /// </summary>
        /// <param name="pos">Mouse position.</param>
        /// <param name="isPointerOverUI">Whether the pointer is over ui game object.</param>
        /// <param name="button">Button id.</param>
        public delegate void OnClickEnded(Vector2 pos, bool isPointerOverUI, int button);

        public void OnUpdate()
        {
            bool isPointerOverUI =
                EventSystem.current == null ? 
                    false : EventSystem.current.IsPointerOverGameObject();

            Vector2 pos = Input.mousePosition;

            foreach (int button in buttonTrackers.Keys)
            {
                if (buttonTrackers[button] == null)
                    buttonTrackers[button] = new ButtonTrackerData();
                ButtonTrackerData trackerData = buttonTrackers[button];

                if (Input.GetMouseButtonUp(button))
                {
                    trackerData.IsMoving = false;

                    if (onClickEndedCallback.TryGetValue(button, out OnClickEnded func) &&
                        func != null)
                    {
                        func(pos, isPointerOverUI, button);
                    }
                }
                else if (trackerData.IsMoving)
                {
                    Vector2 deltaPos = Vector2.zero;
                    if (lastPos != null)
                        deltaPos = pos - lastPos.Value;

                    if (onClickMovedCallback.TryGetValue(button, out OnClickMoved func) &&
                        func != null)
                    {
                        func(pos, deltaPos.magnitude/Time.deltaTime, isPointerOverUI, button);
                    }
                }
                else if (Input.GetMouseButtonDown(button))
                {
                    trackerData.IsMoving = true;

                    if (onClickBeganCallback.TryGetValue(button, out OnClickBegan func) &&
                        func != null)
                    {
                        func(pos, isPointerOverUI, button);
                    }
                }
            }

            lastPos = pos;
        }

        /// <summary>
        /// Add callback when mouse button is pressed.
        /// </summary>
        /// <param name="buttonID">ID of the mouse button to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnClickBegan(int buttonID, OnClickBegan func)
        {
            if (func == null)
                return;

            if (onClickBeganCallback.ContainsKey(buttonID))
            {
                onClickBeganCallback[buttonID] += func;
            }
            else
            {
                onClickBeganCallback.Add(buttonID, func);
                AddTracker(buttonID);
            }
        }

        /// <summary>
        /// Add callback when mouse button is pressed and moved.
        /// </summary>
        /// <param name="buttonID">ID of the mouse button to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnClickMoved(int buttonID, OnClickMoved func)
        {
            if (func == null)
                return;

            if (onClickMovedCallback.ContainsKey(buttonID))
            {
                onClickMovedCallback[buttonID] += func;
            }
            else
            {
                onClickMovedCallback.Add(buttonID, func);
                AddTracker(buttonID);
            }
        }

        /// <summary>
        /// Add callback when mouse button is released.
        /// </summary>
        /// <param name="buttonID">ID of the mouse button to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnClickEnded(int buttonID, OnClickEnded func)
        {
            if (func == null)
                return;

            if (onClickEndedCallback.ContainsKey(buttonID))
            {
                onClickEndedCallback[buttonID] += func;
            }
            else
            {
                onClickEndedCallback.Add(buttonID, func);
                AddTracker(buttonID);
            }
        }

        /// <summary>
        /// Remove callback when mouse button is pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnClickBegan(OnClickBegan func)
        {
            if (func == null)
                return;

            List<int> keys = new List<int>(onClickBeganCallback.Keys);
            foreach (int k in keys)
            {
                onClickBeganCallback[k] -= func;
                if (onClickBeganCallback[k] == null)
                {
                    onClickBeganCallback.Remove(k);
                    RemoveTracker(k);
                }
            }
        }

        /// <summary>
        /// Remove callback when mouse button is pressed and moved.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnClickMoved(OnClickMoved func)
        {
            if (func == null)
                return;

            var keys = onClickMovedCallback.Keys;
            foreach (int k in keys)
            {
                onClickMovedCallback[k] -= func;
                if (onClickMovedCallback[k] == null)
                {
                    onClickMovedCallback.Remove(k);
                    RemoveTracker(k);
                }
            }
        }

        /// <summary>
        /// Remove callback when mouse button is released.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnClickEnded(OnClickEnded func)
        {
            if (func == null)
                return;

            var keys = onClickEndedCallback.Keys;
            foreach (int k in keys)
            {
                onClickEndedCallback[k] -= func;
                if (onClickEndedCallback[k] == null)
                {
                    onClickEndedCallback.Remove(k);
                    RemoveTracker(k);
                }
            }
        }

        /// <summary>
        /// Remove all callbacks on mouse button press.
        /// </summary>
        public void ClearOnClickBegan()
        {
            List<int> keys = new List<int>(onClickBeganCallback.Keys);

            onClickBeganCallback.Clear();

            foreach (int k in keys)
            {
                RemoveTracker(k);
            }
        }

        /// <summary>
        /// Remove all callbacks on mouse button press and move.
        /// </summary>
        public void ClearOnClickMoved()
        {
            List<int> keys = new List<int>(onClickMovedCallback.Keys);

            onClickMovedCallback.Clear();

            foreach (int k in keys)
            {
                RemoveTracker(k);
            }
        }

        /// <summary>
        /// Remove all callbacks on mouse button release.
        /// </summary>
        public void ClearOnClickEnded()
        {
            List<int> keys = new List<int>(onClickEndedCallback.Keys);

            onClickEndedCallback.Clear();

            foreach (int k in keys)
            {
                RemoveTracker(k);
            }
        }

        /// <summary>
        /// Remove all callbacks.
        /// </summary>
        public void Clear()
        {
            onClickBeganCallback.Clear();
            onClickMovedCallback.Clear();
            onClickEndedCallback.Clear();
            buttonTrackers.Clear();
            lastPos = null;
        }

        private int GetMethodButtonID(MethodInfo i)
        {
            if (i == null)
                return -1;

            ParameterInfo[] paramInfos = i.GetParameters();
            if (paramInfos.Length >= 1 &&
                paramInfos[paramInfos.Length - 1].HasDefaultValue)
            {
                object o = paramInfos[paramInfos.Length - 1].DefaultValue;
                if (o is int v)
                {
                    return v;
                }
            }

            return -1;
        }

        private void AddTracker(int button)
        {
            if (!buttonTrackers.ContainsKey(button))
                buttonTrackers.Add(button, new ButtonTrackerData());
        }

        private void RemoveTracker(int button)
        {
            if (!onClickBeganCallback.ContainsKey(button) &&
                !onClickMovedCallback.ContainsKey(button) &&
                !onClickEndedCallback.ContainsKey(button) &&
                buttonTrackers.ContainsKey(button))
            {
                buttonTrackers.Remove(button);
            }
        }

        /// <summary>
        /// Callback of OnClickBegan.
        /// When adding a callback, you need to provide a function with a default button parameter to indicate the button id that needs to be monitored.
        /// </summary>
        public event OnClickBegan OnClickBeganCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                int idx = GetMethodButtonID(i);
                if (idx == -1)
                {
                    Debug.LogWarning("The function for OnClickBeganCallBack MUST has a default int value of its last parameter.");
                    return;
                }

                AddOnClickBegan(idx, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnClickBegan(value);
            }
        }

        /// <summary>
        /// Callback of OnClickMoved.
        /// When adding a callback, you need to provide a function with a default button parameter to indicate the button id that needs to be monitored.
        /// </summary>
        public event OnClickMoved OnClickMovedCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                int idx = GetMethodButtonID(i);
                if (idx == -1)
                {
                    Debug.LogWarning("The function for OnClickMovedCallback MUST has a default int value of its last parameter.");
                    return;
                }

                AddOnClickMoved(idx, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnClickMoved(value);
            }
        }

        /// <summary>
        /// Callback of OnClickEnded.
        /// When adding a callback, you need to provide a function with a default button parameter to indicate the button id that needs to be monitored.
        /// </summary>
        public event OnClickEnded OnClickEndedCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                int idx = GetMethodButtonID(i);
                if (idx == -1)
                {
                    Debug.LogWarning("The function for OnClickEndedCallback MUST has a default int value of its last parameter.");
                    return;
                }

                AddOnClickEnded(idx, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnClickEnded(value);
            }
        }

        private Dictionary<int, OnClickBegan> onClickBeganCallback = new Dictionary<int, OnClickBegan>();
        private Dictionary<int, OnClickMoved> onClickMovedCallback = new Dictionary<int, OnClickMoved>();
        private Dictionary<int, OnClickEnded> onClickEndedCallback = new Dictionary<int, OnClickEnded>();
        private Dictionary<int, ButtonTrackerData> buttonTrackers = new Dictionary<int, ButtonTrackerData>();

        private Vector2? lastPos = null;
    }
}