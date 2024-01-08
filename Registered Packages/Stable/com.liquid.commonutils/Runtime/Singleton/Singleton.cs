using System;
using System.Runtime.Serialization;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// A generic class that provides a singleton pattern.
    /// </summary>
    /// <typeparam name="T">A derived type, which is also used as a generic parameter.</typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>, new()
    {
        private static T instance;
        private static readonly object syslock = new object();

        /// <summary>
        /// Get singleton of object.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = (T)FormatterServices.GetUninitializedObject(typeof(T));
                            instance.Init();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// The constructor of the singleton version. The fields in class need to be initialized in this function.
        /// </summary>
        protected virtual void Init() {}

        /// <summary>
        /// To protect the uniqueness of the singleton pattern, using the constructor directly will throw an exception.
        /// </summary>
        protected Singleton()
        {
            throw new InvalidOperationException("Direct use of the constructor is forbidden. Use Instance instead.");
        }
    }
}