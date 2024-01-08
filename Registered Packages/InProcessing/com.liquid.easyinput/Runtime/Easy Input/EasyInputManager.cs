using UnityEngine;

namespace Liquid.EasyInput
{
    /// <summary>
    /// Provide integration and management tools for input events.
    /// </summary>
    public class EasyInputManager : MonoBehaviour
    {
        private void Awake()
        {
            if (instance != null &&
                instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Update()
        {
            Touchpad.OnUpdate();
            Mouse.OnUpdate();
            Keyboard.OnUpdate();
        }

        /// <summary>
        /// Access mouse controller.
        /// </summary>
        public MouseInputer Mouse
        {
            get => mouse;
        }

        /// <summary>
        /// Access touchpad controller.
        /// </summary>
        public TouchpadInputer Touchpad
        {
            get => touchpad;
        }

        /// <summary>
        /// Access keyboard controller.
        /// </summary>
        public KeyboardInputer Keyboard
        {
            get => keyboard;
        }

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static EasyInputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("_EasyInput");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<EasyInputManager>();
                }
                return instance;
            }
        }

        private MouseInputer mouse = new MouseInputer();
        private TouchpadInputer touchpad = new TouchpadInputer();
        private KeyboardInputer keyboard = new KeyboardInputer();

        private static EasyInputManager instance;
    }
}