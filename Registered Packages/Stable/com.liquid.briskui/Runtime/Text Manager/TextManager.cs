using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Liquid.BriskUI
{
    /// <summary>
    /// Manage text maps in relation to the local language.
    /// </summary>
    public class TextManager
    {
        private Dictionary<SystemLanguage, TextMap> texts = new Dictionary<SystemLanguage, TextMap>();
        private static TextManager instance;
        private static readonly object syslock = new object();

        public static TextManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new TextManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Add a text map into texts pool.
        /// </summary>
        /// <param name="langType">Text map environment.</param>
        /// <param name="map">Text map.</param>
        public void AddTextMap(SystemLanguage langType, TextMap map)
        {
            if (texts.ContainsKey(langType))
                texts[langType].Merge(map);
            else
                texts.Add(langType, map);
        }

        /// <summary>
        /// Remove a text map from texts pool.
        /// </summary>
        /// <param name="langType">Text map environment.</param>
        /// <returns>
        /// Returns true when the removal is successful, otherwise returns false.
        /// </returns>
        public bool RemoveTextMap(SystemLanguage langType)
        {
            return texts.Remove(langType);
        }

        /// <summary>
        /// Get the text map in the current locale.
        /// </summary>
        /// <returns>Text map.</returns>
        public TextMap GetTextMap()
        {
            if (texts.TryGetValue(Application.systemLanguage, out TextMap map))
                return map;
            else
                return null;
        }

        /// <summary>
        /// Get a text based on text id.
        /// </summary>
        /// <param name="textID">Text id.</param>
        /// <returns>Matching text.</returns>
        public string GetText(string textID)
        {
            if(textID == null)
               return "";

            TextMap m = GetTextMap();
            if (m == null)
                return textID;

            if (m.Texts.TryGetValue(textID, out string v))
                return v;
            else
                return textID;
        }

        /// <summary>
        /// Get a text based on a text id and a text parameter.
        /// </summary>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        /// <returns>Matching text.</returns>
        public string GetText(string textID, params string[] textParam)
        {
            if(textID == null)
                return "";

            string textContent = GetText(textID);
            Regex rgx = new Regex(@"(?<=\\)%[^%]*(?<=\\)%");

            foreach (var t in textParam)
            {
                textContent = rgx.Replace(textContent, t, 1);
            }
            textContent = textContent.Replace("\\%", "%");
            return textContent;
        }

        /// <summary>
        /// Get a text based on a text id and a text parameter.
        /// </summary>
        /// <param name="textID">Text id.</param>
        /// <param name="content">Text parameter.</param>
        /// <returns>Matching text.</returns>
        public string GetText(string textID, params TextContent[] content)
        {
            if (textID == null)
                return "";

            string textContent = GetText(textID);
            Regex rgx = new Regex(@"(?<=\\)%[^%]*(?<=\\)%");

            foreach (var t in content)
            {
                textContent = rgx.Replace(textContent, t, 1);
            }
            textContent = textContent.Replace("\\%", "%");
            return textContent;
        }

        /// <summary>
        /// Get a text based on a text id and a text parameter.
        /// </summary>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        /// <returns>Matching text.</returns>
        public string GetText(string textID, Dictionary<string, string> textParam)
        {
            if(textID == null)
                return "";
            
            string textContent = GetText(textID);
            foreach (var tElem in textParam)
            {
                Regex rgx = new Regex(@"(?<=\\)%" + tElem.Key + @"(?<=\\)%");
                textContent = rgx.Replace(textContent, tElem.Value, 1);
            }
            textContent = textContent.Replace("\\%", "%");
            return textContent;
        }

        /// <summary>
        /// Get a text based on a text id and a text parameter.
        /// </summary>
        /// <param name="textID">Text id.</param>
        /// <param name="textParam">Text parameter.</param>
        /// <returns>Matching text.</returns>
        public string GetText(string textID, Dictionary<string, TextContent> textParam)
        {
            if (textID == null)
                return "";

            string textContent = GetText(textID);
            foreach (var tElem in textParam)
            {
                Regex rgx = new Regex(@"(?<=\\)%" + tElem.Key + @"(?<=\\)%");
                textContent = rgx.Replace(textContent, tElem.Value, 1);
            }
            textContent = textContent.Replace("\\%", "%");
            return textContent;
        }

        private TextManager() {}
    }
}