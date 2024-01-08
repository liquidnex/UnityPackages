using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid.BriskUI
{
    /// <summary>
    /// A catalog for ui.
    /// </summary>
    [CreateAssetMenu(fileName = "UI Catalog", menuName = "BriskUI/UI Catalog")]
    public class UICatalog : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Serializable UI information dictionary.
        /// </summary>
        [NonSerialized]
        public Dictionary<string, UICatalogData> Data = new Dictionary<string, UICatalogData>();

        [SerializeField]
        private List<string> UITypeNames = new List<string>();
        [SerializeField]
        private List<string> UILabels = new List<string>();
        [SerializeField]
        private List<string> UIAddresses = new List<string>();


        public void OnBeforeSerialize()
        {
            CheckDicToNotNull();
            UITypeNames.Clear();
            UILabels.Clear();
            UIAddresses.Clear();

            foreach (var data in Data)
            {
                if (data.Key == null ||
                    !data.Value.IsLegal)
                    continue;

                UITypeNames.Add(data.Key);
                UILabels.Add(data.Value.UILabel);
                UIAddresses.Add(data.Value.UIAddress);
            }
        }

        public void OnAfterDeserialize()
        {
            CheckDicToNotNull();
            Data.Clear();

            if (UITypeNames == null ||
                UILabels == null ||
                UIAddresses == null)
                return;

            if (UITypeNames.Count != UILabels.Count ||
                UILabels.Count != UIAddresses.Count)
                return;

            int len = UITypeNames.Count;
            for (int i = 0; i < len; ++i)
            {
                string name = UITypeNames[i];
                string label = UILabels[i];
                string address = UIAddresses[i];

                if (!Data.TryAdd(
                    name, new UICatalogData(label, address)))
                    Debug.LogWarning($"Duplicate type name: {name}, this item will be discarded. Item UILabel: {label}, Item UIAddress: {address}.");
            }
        }

        private void CheckDicToNotNull()
        {
            if (Data == null)
                Data = new Dictionary<string, UICatalogData>();
        }
    }
}