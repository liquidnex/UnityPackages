using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Liquid.EasyInput
{
    /// <summary>
    /// Keyboard controller.
    /// </summary>
    public class KeyboardInputer : IInputer
    {
        /// <summary>
        /// Callback when any key is pressed.
        /// </summary>
        /// <param name="keys">List of pressed key codes.</param>
        public delegate void OnAnyKeyDown(List<KeyCode> keys);

        /// <summary>
        /// Callback when any key is being pressed.
        /// </summary>
        /// <param name="keys">List of key codes being pressed.</param>
        public delegate void OnAnyKey(List<KeyCode> keys);

        /// <summary>
        /// Callback when any key is released.
        /// </summary>
        /// <param name="keys">List of released key codes.</param>
        public delegate void OnAnyKeyUp(List<KeyCode> keys);

        /// <summary>
        /// Callback when the specific key is pressed.
        /// </summary>
        /// <param name="k">Key code.</param>
        public delegate void OnKeyDown(KeyCode k);

        /// <summary>
        /// Callback when the specific key is being pressed.
        /// </summary>
        /// <param name="k">Key code.</param>
        public delegate void OnKey(KeyCode k);

        /// <summary>
        /// Callback when the specific key is released.
        /// </summary>
        /// <param name="k">Key code.</param>
        public delegate void OnKeyUp(KeyCode k);

        public void OnUpdate()
        {
            if (Input.anyKeyDown)
            {
                List<KeyCode> keys = new List<KeyCode>();
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    bool isKeyGotten = Input.GetKeyDown(k);
                    if (isKeyGotten)
                    {
                        keys.Add(k);
                    }

                    if (isKeyGotten &&
                        onKeyDownCallback.TryGetValue(k, out OnKeyDown func) &&
                        func != null)
                    {
                        func(k);
                    }
                }

                if (keys.Count > 0 &&
                    OnAnyKeyDownCallback != null)
                {
                    OnAnyKeyDownCallback(keys);
                }
            }

            if (Input.anyKey)
            {
                List<KeyCode> keys = new List<KeyCode>();
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    bool isKeyGotten = Input.GetKey(k);
                    if (isKeyGotten)
                    {
                        keys.Add(k);
                    }

                    if (isKeyGotten &&
                        onKeyCallback.TryGetValue(k, out OnKey func) &&
                        func != null)
                    {
                        func(k);
                    }
                }

                if (keys.Count > 0 &&
                    OnAnyKeyCallback != null)
                {
                    OnAnyKeyCallback(keys);
                }
            }

            List<KeyCode> releasedkeys = new List<KeyCode>();
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                bool isKeyGotten = Input.GetKeyUp(k);
                if (isKeyGotten)
                {
                    releasedkeys.Add(k);
                }

                if (isKeyGotten &&
                    onKeyUpCallback.TryGetValue(k, out OnKeyUp func) &&
                    func != null)
                {
                    func(k);
                }
            }

            if (releasedkeys.Count > 0 &&
                OnAnyKeyUpCallback != null)
            {
                OnAnyKeyUpCallback(releasedkeys);
            }
        }

        /// <summary>
        /// Add callback when any key is pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnAnyKeyDown(OnAnyKeyDown func)
        {
            if (func == null)
                return;

            OnAnyKeyDownCallback += func;
        }

        /// <summary>
        /// Add callback when any key is being pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnAnyKey(OnAnyKey func)
        {
            if (func == null)
                return;

            OnAnyKeyCallback += func;
        }

        /// <summary>
        /// Add callback when any key is released.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void AddOnAnyKeyUp(OnAnyKeyUp func)
        {
            if (func == null)
                return;

            OnAnyKeyUpCallback += func;
        }

        /// <summary>
        /// Remove callback when and key is pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnAnyKeyDown(OnAnyKeyDown func)
        {
            if (func == null)
                return;

            OnAnyKeyDownCallback -= func;
        }

        /// <summary>
        /// Remove callback when any key is being pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnAnyKey(OnAnyKey func)
        {
            if (func == null)
                return;

            OnAnyKeyCallback -= func;
        }

        /// <summary>
        /// Remove callback when any key is released.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnAnyKeyUp(OnAnyKeyUp func)
        {
            if (func == null)
                return;

            OnAnyKeyUpCallback -= func;
        }

        /// <summary>
        /// Add callback when a specific key is pressed.
        /// </summary>
        /// <param name="key">Keyboard key to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnKeyDown(KeyCode key, OnKeyDown func)
        {
            if (key == KeyCode.None)
                return;
            if (func == null)
                return;

            if (onKeyDownCallback.ContainsKey(key))
            {
                onKeyDownCallback[key] += func;
            }
            else
            {
                onKeyDownCallback.Add(key, func);
            }
        }

        /// <summary>
        /// Add callback when a specific key is being pressed.
        /// </summary>
        /// <param name="key">Keyboard key to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnKey(KeyCode key, OnKey func)
        {
            if (key == KeyCode.None)
                return;
            if (func == null)
                return;

            if (onKeyCallback.ContainsKey(key))
            {
                onKeyCallback[key] += func;
            }
            else
            {
                onKeyCallback.Add(key, func);
            }
        }

        /// <summary>
        /// Add callback when a specific key is released.
        /// </summary>
        /// <param name="key">Keyboard key to listen.</param>
        /// <param name="func">Callback function.</param>
        public void AddOnKeyUp(KeyCode key, OnKeyUp func)
        {
            if (key == KeyCode.None)
                return;
            if (func == null)
                return;

            if (onKeyUpCallback.ContainsKey(key))
            {
                onKeyUpCallback[key] += func;
            }
            else
            {
                onKeyUpCallback.Add(key, func);
            }
        }

        /// <summary>
        /// Remove callback when the specific key is pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnKeyDown(OnKeyDown func)
        {
            if (func == null)
                return;

            List<KeyCode> keys = new List<KeyCode>(onKeyDownCallback.Keys);
            foreach (KeyCode k in keys)
            {
                onKeyDownCallback[k] -= func;
                if (onKeyDownCallback[k] == null)
                    onKeyDownCallback.Remove(k);
            }
        }

        /// <summary>
        /// Remove callback when the specific key is being pressed.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnKey(OnKey func)
        {
            if (func == null)
                return;

            List<KeyCode> keys = new List<KeyCode>(onKeyCallback.Keys);
            foreach (KeyCode k in keys)
            {
                onKeyCallback[k] -= func;
                if (onKeyCallback[k] == null)
                    onKeyCallback.Remove(k);
            }
        }

        /// <summary>
        /// Remove callback when the specific key is released.
        /// </summary>
        /// <param name="func">Callback function.</param>
        public void RemoveOnKeyUp(OnKeyUp func)
        {
            if (func == null)
                return;

            List<KeyCode> keys = new List<KeyCode>(onKeyUpCallback.Keys);
            foreach (KeyCode k in keys)
            {
                onKeyUpCallback[k] -= func;
                if (onKeyUpCallback[k] == null)
                    onKeyUpCallback.Remove(k);
            }
        }

        /// <summary>
        /// Clear all callbacks on any key press.
        /// </summary>
        public void ClearOnAnyKeyDown()
        {
            if (OnAnyKeyDownCallback == null)
                return;

            foreach (OnAnyKeyDown d in OnAnyKeyDownCallback.GetInvocationList())
            {
                OnAnyKeyDownCallback -= d;
            }
            OnAnyKeyDownCallback = null;
        }

        /// <summary>
        /// Clear all callbacks on any key being pressed.
        /// </summary>
        public void ClearOnAnyKey()
        {
            if (OnAnyKeyCallback == null)
                return;

            foreach (OnAnyKey d in OnAnyKeyCallback.GetInvocationList())
            {
                OnAnyKeyCallback -= d;
            }
            OnAnyKeyCallback = null;
        }

        /// <summary>
        /// Clear all callbacks on any key release.
        /// </summary>
        public void ClearOnAnyKeyUp()
        {
            if (OnAnyKeyUpCallback == null)
                return;

            foreach (OnAnyKeyUp d in OnAnyKeyUpCallback.GetInvocationList())
            {
                OnAnyKeyUpCallback -= d;
            }
            OnAnyKeyUpCallback = null;
        }

        /// <summary>
        /// Remove all callbacks that listen for special key press.
        /// </summary>
        public void ClearOnKeyDown()
        {
            onKeyDownCallback.Clear();
        }

        /// <summary>
        /// Remove all callbacks that listen for special key being pressed.
        /// </summary>
        public void ClearOnKey()
        {
            onKeyCallback.Clear();
        }

        /// <summary>
        /// Remove all callbacks that listen for special key release.
        /// </summary>
        public void ClearOnKeyUp()
        {
            onKeyUpCallback.Clear();
        }

        /// <summary>
        /// Clear all callbacks.
        /// </summary>
        public void Clear()
        {
            ClearOnAnyKeyDown();
            ClearOnAnyKey();
            ClearOnAnyKeyUp();

            ClearOnKeyDown();
            ClearOnKey();
            ClearOnKeyUp();
        }

        private KeyCode GetMethodKeyCode(MethodInfo i)
        {
            if (i == null)
                return KeyCode.None;

            ParameterInfo[] paramInfos = i.GetParameters();
            if (paramInfos.Length >= 1 &&
                paramInfos[paramInfos.Length-1].HasDefaultValue)
            {
                object o = paramInfos[paramInfos.Length - 1].DefaultValue;
                if (o is KeyCode k)
                {
                    return k;
                }
            }

            return KeyCode.None;
        }

        /// <summary>
        /// Callback of OnKeyDown.
        /// When adding a callback, you need to provide a function with a default key code parameter to indicate the key that needs to be monitored.
        /// </summary>
        public event OnKeyDown OnKeyDownCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                KeyCode c = GetMethodKeyCode(i);
                if (c == KeyCode.None)
                {
                    Debug.LogWarning("The function for OnKeyDownCallback MUST has a default KeyCode value of its first parameter.");
                    return;
                }

                AddOnKeyDown(c, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnKeyDown(value);
            }
        }

        /// <summary>
        /// Callback of OnKey.
        /// When adding a callback, you need to provide a function with a default key code parameter to indicate the key that needs to be monitored.
        /// </summary>
        public event OnKey OnKeyCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                KeyCode c = GetMethodKeyCode(i);
                if (c == KeyCode.None)
                {
                    Debug.LogWarning("The function for OnKeyCallback MUST has a default KeyCode value of its first parameter.");
                    return;
                }

                AddOnKey(c, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnKey(value);
            }
        }

        /// <summary>
        /// Callback of OnKeyUp.
        /// When adding a callback, you need to provide a function with a default key code parameter to indicate the key that needs to be monitored.
        /// </summary>
        public event OnKeyUp OnKeyUpCallback
        {
            add
            {
                if (value == null)
                    return;

                MethodInfo i = value.Method;
                KeyCode c = GetMethodKeyCode(i);
                if (c == KeyCode.None)
                {
                    Debug.LogWarning("The function for OnKeyUpCallback MUST has a default KeyCode value of its first parameter.");
                    return;
                }

                AddOnKeyUp(c, value);
            }
            remove
            {
                if (value == null)
                    return;

                RemoveOnKeyUp(value);
            }
        }

        /// <summary>
        /// Callback of OnAnyKeyDown.
        /// </summary>
        public event OnAnyKeyDown OnAnyKeyDownCallback;

        /// <summary>
        /// Callback of OnAnyKey.
        /// </summary>
        public event OnAnyKey OnAnyKeyCallback;

        /// <summary>
        /// Callback of OnAnyKeyUp.
        /// </summary>
        public event OnAnyKeyUp OnAnyKeyUpCallback;

        private Dictionary<KeyCode, OnKeyDown> onKeyDownCallback = new Dictionary<KeyCode, OnKeyDown>();
        private Dictionary<KeyCode, OnKey> onKeyCallback = new Dictionary<KeyCode, OnKey>();
        private Dictionary<KeyCode, OnKeyUp> onKeyUpCallback = new Dictionary<KeyCode, OnKeyUp>();
    }
}