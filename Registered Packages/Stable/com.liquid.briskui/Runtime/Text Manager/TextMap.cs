using System.Collections.Generic;

namespace Liquid.BriskUI
{
    /// <summary>
    /// A text map contains the relationship between multiple pairs of text id and text content.
    /// </summary>
    public class TextMap
    {
        private Dictionary<string, string> texts = new Dictionary<string, string>();

        /// <summary>
        /// Access raw dictionary data of text map.
        /// </summary>
        public Dictionary<string, string> Texts
        {
            get => texts;
        }

        /// <summary>
        /// Initialize text map with data from a converter.
        /// </summary>
        /// <param name="textMap">Initialize text map.</param>
        public TextMap(Dictionary<string, string> textMap)
        {
            AddText(textMap);
        }

        /// <summary>
        /// Merge all the data of another text map,
        /// the data with the same text id will be overwritten.
        /// </summary>
        /// <param name="m">Text map to merge.</param>
        public void Merge(TextMap m)
        {
            AddText(m.Texts);
        }

        /// <summary>
        /// Add several sets of data to text map.
        /// </summary>
        /// <param name="d">Dictionary to add.</param>
        private void AddText(Dictionary<string, string> d)
        {
            foreach (var kv in d)
            {
                if (kv.Key == null ||
                    kv.Value == null)
                    continue;

                string key = kv.Key;
                string textContent = kv.Value;
                if (!texts.ContainsKey(key))
                    texts.Add(key, textContent);
                else
                    texts[key] = textContent;
            }
        }
    }
}