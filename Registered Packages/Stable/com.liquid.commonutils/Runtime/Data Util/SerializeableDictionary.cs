using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid.CommonUtils
{
    [Serializable]
    public class SerializeableDictionary<K, V> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<K> keys = new List<K>();
        [SerializeField]
        private List<V> values = new List<V>();
        private Dictionary<K, V> dictionary = new Dictionary<K, V>();

        public Dictionary<K, V> Dictionary => dictionary;

        public static implicit operator SerializeableDictionary<K, V>(Dictionary<K, V> dic)
        {
            var sdic = new SerializeableDictionary<K, V>();
            sdic.dictionary = dic;
            return sdic;
        }

        public static implicit operator Dictionary<K, V>(SerializeableDictionary<K, V> sdic)
        {
            return sdic.dictionary;
        }

        public void OnAfterDeserialize()
        {
            int len = keys.Count;
            dictionary = new Dictionary<K, V>();
            for (int i = 0; i < len; ++i)
            {
                dictionary[keys[i]] = values[i];
            }
            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<K>();
            values = new List<V>();

            foreach (var kv in dictionary)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value);
            }
        }
    }
}