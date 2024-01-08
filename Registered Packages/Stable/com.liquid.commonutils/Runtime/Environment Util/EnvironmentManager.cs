using System;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// A game environment manager.
    /// </summary>
    public class EnvironmentManager
    {
        /// <summary>
        /// Enumeration of all platforms.
        /// </summary>
        [Flags]
        public enum PlatformType
        {
            NONE = 1,
            EDITOR = 1 << 1,
            WIN32 = 1 << 2,
            WIN64 = 1 << 3,
            MAC = 1 << 4,
            ANDROID = 1 << 5,
            IOS = 1 << 6,
            WEBGL = 1 << 7
        }

        /// <summary>
        /// Enumeration of all deliverys.
        /// </summary>
        public enum DeliveryType
        {
            NONE,
            DEV,
            QA,
            STAGE,
            PROD
        }

        private PlatformType platformEnvir;
        private DeliveryType deliveryEnvir;

        private static EnvironmentManager instance;
        private static readonly object syslock = new object();

        private void Init()
        {
#if UNITY_STANDALONE_WIN && UNITY_64
            platformEnvir = PlatformType.WIN64;
#elif UNITY_STANDALONE_WIN
            platformEnvir = PlatformType.WIN32;
#elif UNITY_STANDALONE_OSX
            platformEnvir = PlatformType.MAC;
#elif UNITY_ANDROID
            platformEnvir = PlatformType.ANDROID;
#elif UNITY_IOS
            platformEnvir = PlatformType.IOS;
#elif UNITY_WEBGL
            platformEnvir = PlatformType.WEBGL;
#endif

#if UNITY_EDITOR
            platformEnvir |= PlatformType.EDITOR;
#endif

            // Read the delivery dcripting define symbols in player settings
#if DEV
            deliveryEnvir = DeliveryType.DEV;
#elif QA
            deliveryEnvir = DeliveryType.QA;
#elif STAGE
            deliveryEnvir = DeliveryType.STAGE;
#elif PROD
            deliveryEnvir = DeliveryType.PROD;
#else
            deliveryEnvir = DeliveryType.DEV;
#endif
        }

        private EnvironmentManager() {}

        /// <summary>
        /// Gets the current runtime platform type.
        /// </summary>
        public PlatformType PlatformEnvir
        {
            get => platformEnvir;
        }

        /// <summary>
        /// Gets the current delivery type.
        /// </summary>
        public DeliveryType DeliveryEnvir
        {
            get => deliveryEnvir;
        }

        /// <summary>
        /// Access interface of singleton design pattern object.
        /// </summary>
        public static EnvironmentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new EnvironmentManager();
                            instance.Init();
                        }
                    }
                }
                return instance;
            }
        }
    }
}